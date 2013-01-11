using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using CacheRepository.Indexes;
using Dapper;
using DapperExtensions;
using FubuCore;
using FubuCore.Util;

namespace CacheRepository
{
	public class Repository
	{
		protected readonly ISqlConnectionResolver ConnectionResolver;
		private readonly Func<dynamic, dynamic> assignNextId;
		private readonly Cache<Type, IIndex> indexesCached;
		private readonly Cache<Type, List<dynamic>> entitiesCached;
		private readonly Cache<Type, List<dynamic>> entitiesCachedForBulkInsert;
		private readonly Cache<Type, dynamic> entityIds;

		protected readonly Cache<Type, string> QueriesCached;

		public Repository(ISqlConnectionResolver connectionResolver, IEnumerable<IIndex> idexes, Func<dynamic, dynamic> assignNextId = null)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
			if (idexes == null) throw new ArgumentNullException("idexes");
			this.ConnectionResolver = connectionResolver;
			this.assignNextId = assignNextId;
			this.indexesCached = new Cache<Type, IIndex>(idexes.ToDictionary(index => index.GetType(), index => index));
			this.entitiesCached = new Cache<Type, List<dynamic>>();
			this.QueriesCached = new Cache<Type, string>();

			if (assignNextId == null)
			{
				this.assignNextId = maxId => maxId + 1 ?? 1;
			}

			this.entityIds = new Cache<Type, dynamic> {OnMissing = key => this.assignNextId(this.getAll(key).Max(x => x.Id))};
			this.entitiesCachedForBulkInsert = new Cache<Type, List<dynamic>> { OnMissing = typeOfEntity => new List<dynamic>() };
		}

		public IEnumerable<TEntity> GetAll<TEntity>()
		{
			return getAllWithGeneric<TEntity>().Cast<TEntity>();
		}

		public TIndex GetIndex<TIndex>() where TIndex : IIndex
		{
			var index = (TIndex)this.indexesCached[typeof(TIndex)];
			if (!index.HasBeenHydrated)
			{
				getAll(index.GetEntityType());
				index.HasBeenHydrated = true;
			}
			return index;
		}

		public void BuilkInsertNonGeneric(IEntityWithId entity)
		{
			var typeOfEntity = entity.GetType();
			var methodInfo = typeof(Repository).GetMethods().First(x => x.Name == "BulkInsert");
			var generic = methodInfo.MakeGenericMethod(typeOfEntity);
			var parameter = Array.CreateInstance(typeOfEntity, 1);
			parameter.SetValue(entity, 0);
			generic.Invoke(this, new object[]{parameter});
		}

		public void BulkInsert<TEntity>(params TEntity[] entities) where TEntity : class, IEntityWithId
		{
			this.BulkInsert((IEnumerable<TEntity>)entities);
		}

		public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntityWithId
		{
			var type = typeof (TEntity);
			this.insert(entities, entity => this.entitiesCachedForBulkInsert[type].Add(entity));
		}

		public void Flush(IEnumerable<Type> flushOrder = null)
		{
			if (flushOrder != null)
			{
				flushOrder.Each(type =>
				                	{
				                		BulkInsert(type, (SqlConnection) this.ConnectionResolver.GetConnection(),
				                		           (SqlTransaction) this.ConnectionResolver.GetTransaction(), type.Name,
				                		           this.entitiesCachedForBulkInsert[type]);
				                		this.entitiesCachedForBulkInsert.Remove(type);
				                	});
			}

			this.entitiesCachedForBulkInsert
				.Each((type, entityList) =>
				BulkInsert(type, (SqlConnection)this.ConnectionResolver.GetConnection(),
						   (SqlTransaction)this.ConnectionResolver.GetTransaction(), type.Name,
						   entityList));
			this.entitiesCachedForBulkInsert.ClearAll();
		}

		public void Insert<TEntity>(params TEntity[] entities) where TEntity : class, IEntityWithId
		{
			this.Insert((IEnumerable<TEntity>)entities);
		}

		public void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntityWithId
		{
			insert(entities, entity => ConnectionResolver.GetConnection().Insert(entity, ConnectionResolver.GetTransaction(), 0));
		}

		private void insert<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> insertFunction) where TEntity : class, IEntityWithId
		{
			var type = typeof(TEntity);
			var existingEntities = this.getAllWithGeneric<TEntity>();
			var typeIndexes = this.indexesCached.Where(x => type == x.GetEntityType());
			entities.Each(entity =>
			{
				entity.Id = this.entityIds[type];
				this.entityIds[type] = this.assignNextId(this.entityIds[type]);
				existingEntities.Add(entity);
				insertFunction(entity);
				typeIndexes.Each(index => index.Add(entity));
			});			
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			// what happens if an indexed property gets updated???
			this.ConnectionResolver.GetConnection().Update(entity, this.ConnectionResolver.GetTransaction());
		}

		public Cache<Type, dynamic> GetHashOfIdsByEntityType()
		{
			return this.entityIds;
		}

		public void ExecuteSql(string sql, object parameters = null)
		{
			this.ConnectionResolver.GetConnection()
				.Execute(
				sql, 
				parameters, 
				this.ConnectionResolver.GetTransaction(),
				0);
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null)
		{
			return this.ConnectionResolver.GetConnection().Query<TEntity>(query, parameters, this.ConnectionResolver.GetTransaction(), true, 0);
		}

		private List<dynamic> getAllWithGeneric<TEntity>()
		{
			var type = typeof(TEntity);
			this.entitiesCached.OnMissing = key =>
			{
				var connection = this.ConnectionResolver.GetConnection();
				var transaction = this.ConnectionResolver.GetTransaction();
				this.QueriesCached.OnMissing = queryKey => "Select * From [{0}]".ToFormat(type.Name);
				var all = connection.Query<TEntity>(this.QueriesCached[type], null, transaction, true, 0);
				var typeIndexes = this.indexesCached.Where(x => type == x.GetEntityType());
				all.Each(entity => typeIndexes.Each(index => index.Add(entity)));
				return all.Cast<dynamic>().ToList();
			};
			return this.entitiesCached[type];
		}

		private List<dynamic> getAll(Type type)
		{
			var methodInfo = typeof(Repository).GetMethod("getAllWithGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
			var generic = methodInfo.MakeGenericMethod(type);
			return (List<dynamic>)generic.Invoke(this, null);
		}

		// I got this from here:
		// http://elegantcode.com/2012/01/26/sqlbulkcopy-for-generic-listt-useful-for-entity-framework-nhibernate/
		private static void BulkInsert(Type typeOfEntity, SqlConnection connection, SqlTransaction transaction, string tableName, IList<dynamic> list)
		{
			using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints, transaction))
			{
				bulkCopy.BulkCopyTimeout = 0;
				bulkCopy.BatchSize = list.Count;
				bulkCopy.DestinationTableName = tableName;

				var table = new DataTable();
				var props = TypeDescriptor.GetProperties(typeOfEntity)
					//Dirty hack to make sure we only have system data types 
					//i.e. filter out the relationships/collections
										   .Cast<PropertyDescriptor>()
										   .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
										   .ToArray();

				foreach (var propertyInfo in props)
				{
					bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
					table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
				}

				var values = new object[props.Length];
				foreach (var item in list)
				{
					for (var i = 0; i < values.Length; i++)
					{
						values[i] = props[i].GetValue(item);
					}

					table.Rows.Add(values);
				}

				bulkCopy.WriteToServer(table);
			}
		}
	}}