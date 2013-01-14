using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.Repositories;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Builders
{
	public abstract class RepositoryConfigBuilder<TRepositoryBuilder, TRepositoryConfig, TConnectionResolver, TRepository> 
		where TRepositoryConfig : RepositoryConfig<TRepository, TConnectionResolver> 
		where TConnectionResolver : IConnectionResolver
		where TRepository : Repository
	{
		protected readonly List<Tuple<Type, string>> CustomEntitySql;
		private IEnumerable<IIndex> indexes;
		private INextIdStrategy nextIdStrategy;
		private ISetIdStrategy setIdStrategy;

		protected RepositoryConfigBuilder()
		{
			this.CustomEntitySql = new List<Tuple<Type, string>>();
			this.indexes = new List<IIndex>();
			this.nextIdStrategy = new IdDoesNotExist();
			this.setIdStrategy = new EntityHasNoIdSetter();
		}

		protected abstract TConnectionResolver GetConnectionResolver(TRepositoryConfig repositoryConfig);
		protected abstract IBulkInsertStrategy GetBulkInsertStrategy(TRepositoryConfig repositoryConfig);
		protected abstract IInsertStrategy GetInsertStrategy(TRepositoryConfig repositoryConfig);
		protected abstract IUpdateStrategy GetUpdateStrategy(TRepositoryConfig repositoryConfig);
		protected abstract IEntityRetrieverStrategy GetEntityRetrieverStrategy(TRepositoryConfig repositoryConfig);

		protected void BuildUp(TRepositoryConfig repositoryConfig)
		{
			repositoryConfig.Indexes = this.indexes;
			repositoryConfig.NextIdStrategy = this.nextIdStrategy;
			repositoryConfig.CustomEntitySql = this.CustomEntitySql;
			repositoryConfig.SetIdStrategy = this.setIdStrategy;

			repositoryConfig.ConnectionResolver = this.GetConnectionResolver(repositoryConfig);
			repositoryConfig.BulkInsertStrategy = this.GetBulkInsertStrategy(repositoryConfig);
			repositoryConfig.InsertStrategy = this.GetInsertStrategy(repositoryConfig);
			repositoryConfig.UpdateStrategy = this.GetUpdateStrategy(repositoryConfig);
			repositoryConfig.EntityRetrieverStrategy = this.GetEntityRetrieverStrategy(repositoryConfig);
		}

		public abstract TRepositoryConfig Build();

		public TRepositoryBuilder WithIndexes(IEnumerable<IIndex> newValue)
		{
			this.indexes = newValue;
			return this.GetThis();
		}

		public TRepositoryBuilder WithNextIdStrategy(INextIdStrategy newValue)
		{
			this.nextIdStrategy = newValue;
			return this.GetThis();
		}

		public TRepositoryBuilder WithSetIdStrategy(ISetIdStrategy newValue)
		{
			this.setIdStrategy = newValue;
			return this.GetThis();
		}

		protected abstract TRepositoryBuilder GetThis();
	}
}