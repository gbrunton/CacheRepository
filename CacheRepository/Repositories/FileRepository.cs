﻿using System;
using System.Collections.Generic;
using CacheRepository.Behaviours;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.CommitStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.ExecuteSqlStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.QueryStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;
using FubuCore.Util;

namespace CacheRepository.Repositories
{
	public class FileRepository
		: IDisposable, ICanGet
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
					InsertStrategy = new DoNothingInsert(),
					NextIdStrategy = new IdDoesNotExist(),
					SetIdStrategy = new EntityHasNoIdSetter(),
					QueryStrategy = new DoNothingQuery(),
					UpdateStrategy = new DoNothingUpdate()
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