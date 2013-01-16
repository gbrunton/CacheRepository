using System;
using System.Collections.Generic;
using System.Data;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.CommitStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.DisposeStrategies;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.ExecuteSqlStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.QueryStrategies;
using CacheRepository.Repositories;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Builders
{
	public class SqlRepositoryConfigBuilder
	{
		private readonly IDbConnection connection;
		private IEnumerable<IIndex> indexes;
		private IDisposeStrategy disposeStrategy;
		private IBulkInsertStrategy bulkInsertStrategy;
		private ICommitStrategy commitStrategy;
		private readonly List<Tuple<Type, string>> customEntitySql;
		private IEntityRetrieverStrategy entityRetrieverStrategy;
		private IExecuteSqlStrategy executeSqlStrategy;
		private IInsertStrategy insertStrategy;
		private INextIdStrategy nextIdStrategy;
		private IQueryStrategy queryStrategy;
		private ISetIdStrategy setIdStrategy;
		private IUpdateStrategy updateStrategy;

		public SqlRepositoryConfigBuilder(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			this.connection = connection;
			this.indexes = new List<IIndex>();
			this.customEntitySql = new List<Tuple<Type, string>>();
			this.nextIdStrategy = new IdDoesNotExist();
			this.setIdStrategy = new EntityHasNoIdSetter();
		}

		public SqlRepositoryConfig Build()
		{
			var connectionResolver = new SqlConnectionResolver(this.connection);
			this.bulkInsertStrategy = this.bulkInsertStrategy ?? new SqlServerBulkInsert(connectionResolver);
			this.commitStrategy = this.commitStrategy ?? new CommitSqlConnection(connectionResolver);
			this.entityRetrieverStrategy = this.entityRetrieverStrategy ?? new SqlEntityRetrieverStrategy(connectionResolver);
			this.executeSqlStrategy = this.executeSqlStrategy ?? new ExecuteSqlWithDapper(connectionResolver);
			this.insertStrategy = this.insertStrategy ?? new SqlInsert(connectionResolver);
			this.updateStrategy = this.updateStrategy ?? new SqlUpdate(connectionResolver);
			this.queryStrategy = this.queryStrategy ?? new QueryWithDapper(connectionResolver);
			this.disposeStrategy = this.disposeStrategy ?? new DisposeConnectionResolver(connectionResolver);
			return new SqlRepositoryConfig
				{
					Indexes = this.indexes,
					BulkInsertStrategy = this.bulkInsertStrategy,
					CommitStrategy = this.commitStrategy,
					CustomEntitySql = this.customEntitySql,
					DisposeStrategy = this.disposeStrategy,
					EntityRetrieverStrategy = this.entityRetrieverStrategy,
					ExecuteSqlStrategy = this.executeSqlStrategy,
					InsertStrategy = this.insertStrategy,
					NextIdStrategy = this.nextIdStrategy,
					QueryStrategy = this.queryStrategy,
					SetIdStrategy = this.setIdStrategy,
					UpdateStrategy = this.updateStrategy
				};
		}

		public SqlRepositoryConfigBuilder WithNextIdStrategy(INextIdStrategy newValue)
		{
			this.nextIdStrategy = newValue;
			return this;
		}

		public SqlRepositoryConfigBuilder WithSetIdStrategy(ISetIdStrategy newValue)
		{
			this.setIdStrategy = newValue;
			return this;
		}

		public SqlRepositoryConfigBuilder AddCustomEntitySql<TEntity>(string sql)
		{
			this.customEntitySql.Add(new Tuple<Type, string>(typeof(TEntity), sql));
			return this;
		}

		public SqlRepositoryConfigBuilder WithIndexes(IEnumerable<IIndex> newValue)
		{
			this.indexes = newValue ?? new List<IIndex>();
			return this;
		}

		public SqlRepositoryConfigBuilder WithIndexes(params IIndex[] newValue)
		{
			this.indexes = newValue;
			return this;
		}
	}
}