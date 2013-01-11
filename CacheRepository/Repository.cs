using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using CacheRepository.Configuration;
using CacheRepository.Indexes;
using Dapper;
using DapperExtensions;
using FubuCore;
using FubuCore.Util;

namespace CacheRepository
{
	public class Repository
	{
		private readonly RepositoryConfig repositoryConfig;
		private readonly Cache<Type, IIndex> indexesCached;
		private readonly Cache<Type, List<dynamic>> entitiesCached;
		private readonly Cache<Type, List<dynamic>> entitiesCachedForBulkInsert;
		private readonly Cache<Type, dynamic> entityIds;
		private readonly Cache<Type, string> entitySqlCached;

		public Repository(RepositoryConfigBuilder repositoryConfigBuilder)
		{
			if (repositoryConfigBuilder == null) throw new ArgumentNullException("repositoryConfigBuilder");
			this.repositoryConfig = repositoryConfigBuilder.Build();
			this.indexesCached = new Cache<Type, IIndex>(repositoryConfig.Indexes.ToDictionary(index => index.GetType(), index => index));
			this.entitiesCached = new Cache<Type, List<dynamic>>();
			this.entitySqlCached = new Cache<Type, string>();
			this.entityIds = new Cache<Type, dynamic> { OnMissing = key => repositoryConfig.NextIdCommand.GetNextId(this.getAll(key).Max(x => x.Id)) };
			this.entitiesCachedForBulkInsert = new Cache<Type, List<dynamic>> { OnMissing = typeOfEntity => new List<dynamic>() };
			repositoryConfig.CustomEntitySql.Each(x => entitySqlCached[x.Item1] = x.Item2);
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
			var connection = (SqlConnection) this.repositoryConfig.ConnectionResolver.GetConnection();
			var tranaction = (SqlTransaction) this.repositoryConfig.ConnectionResolver.GetTransaction();
			if (flushOrder != null)
			{
				flushOrder.Each(type =>
				                	{
										BulkInsert(type, connection, tranaction, type.Name,this.entitiesCachedForBulkInsert[type]);
				                		this.entitiesCachedForBulkInsert.Remove(type);
				                	});
			}

			this.entitiesCachedForBulkInsert
			    .Each((type, entityList) =>
			          BulkInsert(type, connection, tranaction, type.Name, entityList));
			this.entitiesCachedForBulkInsert.ClearAll();
		}

		public void Insert<TEntity>(params TEntity[] entities) where TEntity : class, IEntityWithId
		{
			this.Insert((IEnumerable<TEntity>)entities);
		}

		public void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntityWithId
		{
			insert(entities, entity => this.repositoryConfig.ConnectionResolver.GetConnection().Insert(entity, this.repositoryConfig.ConnectionResolver.GetTransaction(), 0));
		}

		private void insert<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> insertFunction) where TEntity : class, IEntityWithId
		{
			var type = typeof(TEntity);
			var existingEntities = this.getAllWithGeneric<TEntity>();
			var typeIndexes = this.indexesCached.Where(x => type == x.GetEntityType());
			entities.Each(entity =>
			{
				entity.Id = this.entityIds[type];
				this.entityIds[type] = this.repositoryConfig.NextIdCommand.GetNextId(this.entityIds[type]);
				existingEntities.Add(entity);
				insertFunction(entity);
				typeIndexes.Each(index => index.Add(entity));
			});			
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			// what happens if an indexed property gets updated???
			// See Issue #2
			this.repositoryConfig.ConnectionResolver.GetConnection().Update(entity, this.repositoryConfig.ConnectionResolver.GetTransaction());
		}

		public Cache<Type, dynamic> GetHashOfIdsByEntityType()
		{
			return this.entityIds;
		}

		public void ExecuteSql(string sql, object parameters = null)
		{
			this.repositoryConfig.ConnectionResolver.GetConnection()
				.Execute(
				sql, 
				parameters,
				this.repositoryConfig.ConnectionResolver.GetTransaction(),
				0);
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null)
		{
			return this.repositoryConfig.ConnectionResolver.GetConnection().Query<TEntity>(query, parameters, this.repositoryConfig.ConnectionResolver.GetTransaction(), true, 0);
		}

		private List<dynamic> getAllWithGeneric<TEntity>()
		{
			var type = typeof(TEntity);
			this.entitiesCached.OnMissing = key =>
			{
				var connection = this.repositoryConfig.ConnectionResolver.GetConnection();
				var transaction = this.repositoryConfig.ConnectionResolver.GetTransaction();
				this.entitySqlCached.OnMissing = entityType => "Select * From [{0}]".ToFormat(type.Name);
				var all = connection.Query<TEntity>(this.entitySqlCached[type], null, transaction, true, 0);
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