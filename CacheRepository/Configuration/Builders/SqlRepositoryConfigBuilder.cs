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
		public SqlRepositoryConfigBuilder(SqlConnectionResolver sqlConnectionResolver)
		{
			SqlConnectionResolver = sqlConnectionResolver;
			this.BulkInsertStratgy = new SqlServerBulkInsert(this);
			this.InsertStrategy = new SqlInsert(this);
			this.UpdateStrategy = new SqlUpdate(this);
			this.EntityRetrieverStrategy = new SqlEntityRetrieverStrategy(this);
		}

		public SqlConnectionResolver SqlConnectionResolver { get; private set; }

		public override SqlRepositoryConfig Build()
		{
			var sqlRepositoryConfig = new SqlRepositoryConfig
				{
					SqlConnectionResolver = this.SqlConnectionResolver
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