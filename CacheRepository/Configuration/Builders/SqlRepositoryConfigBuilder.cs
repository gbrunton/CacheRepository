using System;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.InsertStrategies;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Builders
{
	public class SqlRepositoryConfigBuilder : RepositoryConfigBuilder<SqlRepositoryConfigBuilder, SqlRepositoryConfig>
	{
		private readonly SqlConnectionResolver sqlConnectionResolver;

		public SqlRepositoryConfigBuilder(SqlConnectionResolver sqlConnectionResolver)
		{
			if (sqlConnectionResolver == null) throw new ArgumentNullException("sqlConnectionResolver");
			this.sqlConnectionResolver = sqlConnectionResolver;
		}

		protected override IBulkInsertStrategy GetBulkInsertStrategy(SqlRepositoryConfig repositoryConfig)
		{
			return new SqlServerBulkInsert(repositoryConfig);
		}

		protected override IInsertStrategy GetInsertStrategy(SqlRepositoryConfig repositoryConfig)
		{
			return new SqlInsert(repositoryConfig);
		}

		protected override IUpdateStrategy GetUpdateStrategy(SqlRepositoryConfig repositoryConfig)
		{
			return new SqlUpdate(repositoryConfig);
		}

		protected override IEntityRetrieverStrategy GetEntityRetrieverStrategy(SqlRepositoryConfig repositoryConfig)
		{
			return new SqlEntityRetrieverStrategy(repositoryConfig);
		}

		public override SqlRepositoryConfig Build()
		{
			var sqlRepositoryConfig = new SqlRepositoryConfig
				{
					SqlConnectionResolver = this.sqlConnectionResolver
				};
			base.BuildUp(sqlRepositoryConfig);
			return sqlRepositoryConfig;
		}

		public SqlRepositoryConfigBuilder AddCustomEntitySql<TEntity>(string sql)
		{
			this.CustomEntitySql.Add(new Tuple<Type, string>(typeof(TEntity), sql));
			return this;
		}

		protected override SqlRepositoryConfigBuilder GetThis()
		{
			return this;
		}
	}
}