using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Builders
{
	public abstract class RepositoryConfigBuilder<TRepositoryBuilder, TRepositoryConfig> where TRepositoryConfig : RepositoryConfig
	{
		protected readonly List<Tuple<Type, string>> CustomEntitySql;
		private IEnumerable<IIndex> indexes;
		private INextIdStrategy nextIdStrategy;
		private ISetIdStrategy setIdStrategy;

		protected IBulkInsertStrategy BulkInsertStratgy;
		protected IInsertStrategy InsertStrategy;
		protected IUpdateStrategy UpdateStrategy;
		protected IEntityRetrieverStrategy EntityRetrieverStrategy;

		protected RepositoryConfigBuilder()
		{
			this.CustomEntitySql = new List<Tuple<Type, string>>();
			this.indexes = new List<IIndex>();
			this.nextIdStrategy = new IdDoesNotExist();
			this.setIdStrategy = new EntityHasNoIdSetter();
		}

		protected void BuildUp(RepositoryConfig repositoryConfig)
		{
			repositoryConfig.Indexes = this.indexes;
			repositoryConfig.NextIdStrategy = this.nextIdStrategy;
			repositoryConfig.CustomEntitySql = this.CustomEntitySql;
			repositoryConfig.SetIdStrategy = this.setIdStrategy;

			repositoryConfig.BulkInsertStrategy = this.BulkInsertStratgy;
			repositoryConfig.InsertStrategy = this.InsertStrategy;
			repositoryConfig.UpdateStrategy = this.UpdateStrategy;
			repositoryConfig.EntityRetrieverStrategy = this.EntityRetrieverStrategy;
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