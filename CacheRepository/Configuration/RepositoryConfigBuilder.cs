using System;
using System.Collections.Generic;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration
{
	public class RepositoryConfigBuilder
	{
		private readonly ISqlConnectionResolver connectionResolver;
		private IEnumerable<IIndex> indexes;
		private readonly List<Tuple<Type, string>> customEntitySql;
		private INextIdStrategy nextIdStrategy;
		private ISetIdStrategy setIdStrategy;
		private IBulkInsertStrategy bulkInsertStratgy;
		private IInsertStrategy insertStrategy;
		private IUpdateStrategy updateStrategy;
		private IEntityRetrieverStrategy entityRetrieverStrategy;

		public RepositoryConfigBuilder(ISqlConnectionResolver connectionResolver)
		{
			this.connectionResolver = connectionResolver;
			this.indexes = new List<IIndex>();
			this.customEntitySql = new List<Tuple<Type, string>>();
			this.nextIdStrategy = new IdDoesNotExist();
			this.setIdStrategy = new EntityHasNoIdSetter();
			this.bulkInsertStratgy = new SqlServerBulkInsert();
			this.insertStrategy = new SqlInsert();
			this.updateStrategy = new SqlUpdate();
			this.entityRetrieverStrategy = new SqlEntityRetrieverStrategy();
		}

		internal RepositoryConfig Build()
		{
			this.bulkInsertStratgy.ConnectionResolver = this.connectionResolver;
			this.insertStrategy.ConnectionResolver = this.connectionResolver;
			this.updateStrategy.ConnectionResolver = this.connectionResolver;
			this.entityRetrieverStrategy.ConnectionResolver = this.connectionResolver;

			return new RepositoryConfig
				(
					this.connectionResolver, 
					this.indexes, 
					this.nextIdStrategy, 
					this.customEntitySql,
					this.setIdStrategy,
					this.bulkInsertStratgy,
					this.insertStrategy,
					this.updateStrategy,
					this.entityRetrieverStrategy
				);
		}

		public RepositoryConfigBuilder WithIndexes(IEnumerable<IIndex> newValue)
		{
			this.indexes = newValue;
			return this;
		}

		public RepositoryConfigBuilder WithNextIdStrategy(INextIdStrategy newValue)
		{
			this.nextIdStrategy = newValue;
			return this;
		}

		public RepositoryConfigBuilder WithSetIdStrategy(ISetIdStrategy newValue)
		{
			this.setIdStrategy = newValue;
			return this;
		}

		public RepositoryConfigBuilder AddCustomEntitySql<TEntity>(string sql)
		{
			this.customEntitySql.Add(new Tuple<Type, string>(typeof(TEntity), sql));
			return this;
		}
	}
}