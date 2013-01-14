using System;
using System.Collections.Generic;
using System.Linq;
using CacheRepository.Configuration.Configs;
using Dapper;

namespace CacheRepository.Repositories
{
	public class SqlRepository : Repository
	{
		private readonly SqlRepositoryConfig repositoryConfig;

		public SqlRepository(SqlRepositoryConfig repositoryConfig) : base(repositoryConfig)
		{
			this.repositoryConfig = repositoryConfig;
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
			this.insert(entities, entity => this.EntitiesCachedForBulkInsert[type].Add(entity));
		}

		public void Flush(IEnumerable<Type> flushOrder = null)
		{
			if (flushOrder != null)
			{
				flushOrder.Each(type =>
				{
					this.repositoryConfig.BulkInsertStrategy.Insert(type, this.EntitiesCachedForBulkInsert[type]);
					this.EntitiesCachedForBulkInsert.Remove(type);
				});
			}

			this.EntitiesCachedForBulkInsert
				.Each((type, entityList) =>
					  this.repositoryConfig.BulkInsertStrategy.Insert(type, this.EntitiesCachedForBulkInsert[type]));
			this.EntitiesCachedForBulkInsert.ClearAll();
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
			var existingEntities = this.GetAllWithGeneric<TEntity>();
			var typeIndexes = this.IndexesCached.Where(x => type == x.GetEntityType());
			entities.Each(entity =>
			{
				var id = this.EntityIds[type];
				this.repositoryConfig.SetIdStrategy.SetId(id, entity);
				this.EntityIds[type] = this.repositoryConfig.NextIdStrategy.GetNextId(id);
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
			this.repositoryConfig.SqlConnectionResolver.GetConnection()
				.Execute(
				sql,
				parameters,
				this.repositoryConfig.SqlConnectionResolver.GetTransaction(),
				0);
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null)
		{
			return this.repositoryConfig
				.SqlConnectionResolver
				.GetConnection()
				.Query<TEntity>
					(query, parameters, this.repositoryConfig.SqlConnectionResolver.GetTransaction(), true, 0);
		}
	}
}