using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CacheRepository.Behaviours;
using CacheRepository.Configuration.Configs;
using CacheRepository.Indexes;
using CacheRepository.Utils;

namespace CacheRepository.Repositories
{
	public class Repository 
		: IDisposable, ICanInsert, ICanBuilkInsert, ICanSqlQuery, ICanExecuteSql, ICanUpdate, ICanCommit, ICanGet
	{
		private readonly IRepositoryConfig repositoryConfig;
		private readonly Cache<Type, List<dynamic>> entitiesCachedForBulkInsert;
		private readonly Cache<Type, dynamic> entityIds;
		private readonly Cache<Type, string> entitySqlCached;
	    private readonly RepositoryData repositoryData;

		public Repository(IRepositoryConfig repositoryConfig)
		{
			if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
			this.repositoryConfig = repositoryConfig;
			this.entitySqlCached = new Cache<Type, string> {OnMissing = key => null};
			this.entityIds = new Cache<Type, dynamic> { OnMissing = key => this.repositoryConfig.NextIdStrategy.GetNextId(key, () => this.GetAll(key).Max(x => x.Id)) };
			this.entitiesCachedForBulkInsert = new Cache<Type, List<dynamic>> { OnMissing = typeOfEntity => new List<dynamic>() };
			this.repositoryConfig.CustomEntitySql.Each(x => entitySqlCached[x.Item1] = x.Item2);
		    this.repositoryData = new RepositoryData(repositoryConfig);
		}

		public void BuilkInsertNonGeneric(object entity)
		{
			var typeOfEntity = entity.GetType();
			var methodInfo = typeof(Repository).GetMethods().First(x => x.Name == "BulkInsert");
			var generic = methodInfo.MakeGenericMethod(typeOfEntity);
			var parameter = Array.CreateInstance(typeOfEntity, 1);
			parameter.SetValue(entity, 0);
			generic.Invoke(this, new object[] { parameter });
		}

		public void BulkInsert<TEntity>(params TEntity[] entities) where TEntity : class
		{
			this.BulkInsert((IEnumerable<TEntity>)entities);
		}

		public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			var type = typeof(TEntity);
			this.insert(entities, entity => this.entitiesCachedForBulkInsert[type].Add(entity));
		}

		public void BulkInsertFlush(IEnumerable<Type> flushOrder = null)
		{
			if (flushOrder != null)
			{
				flushOrder.Each(type =>
				{
					this.repositoryConfig.BulkInsertStrategy.Insert(type, this.entitiesCachedForBulkInsert[type]);
					this.entitiesCachedForBulkInsert.Remove(type);
				});
			}

			this.entitiesCachedForBulkInsert
				.Each((type, entityList) =>
					  this.repositoryConfig.BulkInsertStrategy.Insert(type, this.entitiesCachedForBulkInsert[type]));
			this.entitiesCachedForBulkInsert.ClearAll();
		}

		public void Insert<TEntity>(params TEntity[] entities) where TEntity : class
		{
			this.Insert((IEnumerable<TEntity>)entities);
		}

		public void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			insert(entities, entity =>
							 this.repositoryConfig.InsertStrategy.Insert(entity));
		}

		private void insert<TEntity>(IEnumerable<TEntity> entities, Action<TEntity> insertFunction) where TEntity : class
		{
			var type = typeof(TEntity);
			var existingEntities = this.getAllWithGeneric<TEntity>();
			var typeIndexes = this.repositoryData.IndexesCached.Value.Where(x => type == x.GetEntityType());
			entities.Each(entity =>
			{
				var id = this.entityIds[type];
				this.repositoryConfig.SetIdStrategy.SetId(type, id, entity);
				this.entityIds[type] = this.repositoryConfig.NextIdStrategy.GetNextId(type, () => id);
				existingEntities.Add(entity);
				insertFunction(entity);
				typeIndexes.Each(index => index.Add(entity));
			});
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			this.repositoryConfig.UpdateStrategy.Update(entity);
		}

		public void ExecuteSql(string sql, object parameters = null)
		{
			this.repositoryConfig.ExecuteSqlStrategy.Execute(sql, parameters);
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class 
		{
			return this.repositoryConfig.QueryStrategy.Query<TEntity>(query, parameters);
		}

		public void Commit()
		{
			this.repositoryConfig.CommitStrategy.Commit();
		}

		public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class 
		{
			return getAllWithGeneric<TEntity>().Cast<TEntity>();
		}

		public TIndex GetIndex<TIndex>() where TIndex : IIndex
		{
            var index = (TIndex)this.repositoryData.IndexesCached.Value[typeof(TIndex)];
			if (!index.HasBeenHydrated)
			{
				GetAll(index.GetEntityType());
				index.HasBeenHydrated = true;
			}
			return index;
		}

		public Cache<Type, dynamic> GetHashOfIdsByEntityType()
		{
			return this.entityIds;
		}

		public void Dispose()
		{
			this.repositoryConfig.DisposeStrategy.Dispose();
		    this.repositoryData.Dispose();
		}

		private List<dynamic> getAllWithGeneric<TEntity>() where TEntity : class 
		{
			var type = typeof(TEntity);
            this.repositoryData.EntitiesCached.Value.OnMissing = key =>
			{
				var all = this.repositoryConfig
					.EntityRetrieverStrategy
					.GetAll<TEntity>(this.entitySqlCached[type]);
                var typeIndexes = this.repositoryData.IndexesCached.Value.Where(x => type == x.GetEntityType());
				var returnList = new List<dynamic>();
				all.Each(entity =>
					{
						typeIndexes.Each(index => index.Add(entity));
						returnList.Add(entity);
					});
				return returnList;
			};
            return this.repositoryData.EntitiesCached.Value[type];
		}

		public List<dynamic> GetAll(Type type)
		{
			var methodInfo = typeof(Repository)
				.GetMethod("getAllWithGeneric", BindingFlags.NonPublic | BindingFlags.Instance);
			var generic = methodInfo.MakeGenericMethod(type);
			return (List<dynamic>)generic.Invoke(this, null);
		}
	}
}