using System;
using System.Collections.Generic;
using System.Data;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.CommitStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.DisposeStrategies;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.EntityRetrieverStrategies.GetAllQueryStrategies;
using CacheRepository.ExecuteSqlStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.QueryStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.SqlQualifierStrategies;
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
        private readonly Dictionary<Type, IGetAllQuery> overriddenDefaultGetAllQueryStrategy;
		private IEntityRetrieverStrategy entityRetrieverStrategy;
		private IExecuteSqlStrategy executeSqlStrategy;
		private IInsertStrategy insertStrategy;
		private INextIdStrategy nextIdStrategy;
		private IQueryStrategy queryStrategy;
		private ISetIdStrategy setIdStrategy;
		private IUpdateStrategy updateStrategy;
		private ISqlConnectionResolver connectionResolver;
	    private ISqlQualifiers sqlQualifiers;
        private bool persistData;

	    public SqlRepositoryConfigBuilder(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			this.connection = connection;
			this.indexes = new List<IIndex>();
			this.customEntitySql = new List<Tuple<Type, string>>();
            this.overriddenDefaultGetAllQueryStrategy = new Dictionary<Type, IGetAllQuery>();
			this.nextIdStrategy = new SmartNextIdRetreiver();
			this.setIdStrategy = new SmartEntityIdSetter();
	        this.sqlQualifiers = new SqlServerQualifiers();
		}

		public SqlRepositoryConfig Build()
		{
			this.connectionResolver = this.connectionResolver ?? new SqlConnectionResolver(this.connection);
			this.bulkInsertStrategy = this.bulkInsertStrategy ?? new SqlServerBulkInsert(connectionResolver);
			this.commitStrategy = this.commitStrategy ?? new CommitSqlConnection(connectionResolver);
            this.entityRetrieverStrategy = this.entityRetrieverStrategy ?? new SqlEntityRetrieverStrategy(connectionResolver, this.sqlQualifiers, this.overriddenDefaultGetAllQueryStrategy);
			this.executeSqlStrategy = this.executeSqlStrategy ?? new ExecuteSqlWithDapper(connectionResolver);
			this.insertStrategy = this.insertStrategy ?? new SqlInsertWithDapper(connectionResolver);
			this.updateStrategy = this.updateStrategy ?? new SqlUpdateWithDapper(connectionResolver);
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
					UpdateStrategy = this.updateStrategy,
                    PersistData = this.persistData
                };
		}

		public SqlRepositoryConfigBuilder WithConnectionResolver(ISqlConnectionResolver newValue)
		{
			this.connectionResolver = newValue;
			return this;
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

        public SqlRepositoryConfigBuilder WithSqlQualifiers(ISqlQualifiers newValue)
        {
            this.sqlQualifiers = newValue;
            return this;
        }

        public SqlRepositoryConfigBuilder WithGetAllQueryStrategy<TEntity>(IGetAllQuery getAllQuery) where TEntity : class
        {
            this.overriddenDefaultGetAllQueryStrategy.Add(typeof(TEntity), getAllQuery);
            return this;
        }

        public SqlRepositoryConfigBuilder WithPersistData(bool newValue)
        {
            this.persistData = newValue;
            return this;
        }
	}
}