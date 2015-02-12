using System;
using System.Collections.Generic;
using CacheRepository.Behaviours;
using CacheRepository.Configuration.Configs;
using CacheRepository.Indexes;
using CacheRepository.Utils;

namespace CacheRepository.Repositories
{
	public class SqlRepository
		: IDisposable, ICanInsert, ICanBuilkInsert, ICanSqlQuery, ICanExecuteSql, ICanUpdate, ICanCommit, ICanGet
	{
		private readonly Repository repository;

		public SqlRepository(SqlRepositoryConfig repositoryConfig)
		{
			var config = new RepositoryConfig
				{
					DisposeStrategy = repositoryConfig.DisposeStrategy,
					EntityRetrieverStrategy = repositoryConfig.EntityRetrieverStrategy,
					Indexes = repositoryConfig.Indexes,
					BulkInsertStrategy = repositoryConfig.BulkInsertStrategy,
					CommitStrategy = repositoryConfig.CommitStrategy,
					CustomEntitySql = repositoryConfig.CustomEntitySql,
					ExecuteSqlStrategy = repositoryConfig.ExecuteSqlStrategy,
					InsertStrategy = repositoryConfig.InsertStrategy,
					NextIdStrategy = repositoryConfig.NextIdStrategy,
					SetIdStrategy = repositoryConfig.SetIdStrategy,
					QueryStrategy = repositoryConfig.QueryStrategy,
					UpdateStrategy = repositoryConfig.UpdateStrategy
				};
			this.repository = new Repository(config);			
		}

		public void Dispose()
		{
			this.repository.Dispose();
		}

		public void Insert<TEntity>(params TEntity[] entities) where TEntity : class
		{
			this.repository.Insert(entities);
		}

		public void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			this.repository.Insert(entities);
		}

		public void BuilkInsertNonGeneric(object entity)
		{
			this.repository.BuilkInsertNonGeneric(entity);
		}

		public void BulkInsert<TEntity>(params TEntity[] entities) where TEntity : class
		{
			this.repository.BulkInsert(entities);
		}

		public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			this.repository.BulkInsert(entities);
		}

		public void BulkInsertFlush(IEnumerable<Type> flushOrder = null)
		{
			this.repository.BulkInsertFlush(flushOrder);
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class
		{
			return this.repository.Query<TEntity>(query, parameters);
		}

		public void ExecuteSql(string sql, object parameters = null)
		{
			this.repository.ExecuteSql(sql, parameters);
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			this.repository.Update(entity);
		}

		public void Commit()
		{
			this.repository.Commit();
		}

		public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
		{
			return this.repository.GetAll<TEntity>();
		}

		public IEnumerable<dynamic> GetAll(Type entityType)
		{
			return this.repository.GetAll(entityType);
		}

		public TIndex GetIndex<TIndex>() where TIndex : IIndex
		{
			return this.repository.GetIndex<TIndex>();
		}

		public Cache<Type, dynamic> GetHashOfIdsByEntityType()
		{
			return this.repository.GetHashOfIdsByEntityType();
		}
	}
}