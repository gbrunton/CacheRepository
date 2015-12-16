using System;
using System.Collections.Generic;
using CacheRepository.Behaviours;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.CommitStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.ExecuteSqlStrategies;
using CacheRepository.Indexes;
using CacheRepository.QueryStrategies;
using CacheRepository.UpdateStrategies;
using CacheRepository.Utils;

namespace CacheRepository.Repositories
{
	public class FileRepository
		: IDisposable, ICanGet, ICanInsert
	{
		private readonly Repository repository;

		public FileRepository(FileRepositoryConfig repositoryConfig)
		{
			var config = new RepositoryConfig
				{
					DisposeStrategy = repositoryConfig.DisposeStrategy,
					EntityRetrieverStrategy = new FileEntityRetrieverStrategy(repositoryConfig.EntityFactoryStrategy, repositoryConfig.ConnectionResolver),
					Indexes = repositoryConfig.Indexes,
					BulkInsertStrategy = new DoNothingBulkInsert(),
					CommitStrategy = new DoNothingCommit(),
					CustomEntitySql = new List<Tuple<Type, string>>(),
					ExecuteSqlStrategy = new DoNothingExecuteSql(),
					InsertStrategy = repositoryConfig.InsertStrategy,
					NextIdStrategy = repositoryConfig.NextIdStrategy,
					SetIdStrategy = repositoryConfig.SetIdStrategy,
					QueryStrategy = new DoNothingQuery(),
					UpdateStrategy = new DoNothingUpdate(),
                    PersistedDataPath = repositoryConfig.PersistedDataPath
				};
			this.repository = new Repository(config);
		}

		public void Dispose()
		{
			this.repository.Dispose();
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

		public void Insert<TEntity>(params TEntity[] entities) where TEntity : class
		{
			this.repository.Insert(entities);
		}

		public void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
		{
			this.repository.Insert(entities);
		}
	}
}