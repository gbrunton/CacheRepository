using System;
using System.Data;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.InsertStrategies;
using CacheRepository.Repositories;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Builders
{
	public class SqlRepositoryConfigBuilder 
		: RepositoryConfigBuilder<SqlRepositoryConfigBuilder, SqlRepositoryConfig, SqlConnectionResolver, SqlRepository>
	{
		private readonly IDbConnection connection;

		public SqlRepositoryConfigBuilder(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			this.connection = connection;
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

		protected override SqlConnectionResolver GetConnectionResolver(SqlRepositoryConfig repositoryConfig)
		{
			return new SqlConnectionResolver(this.connection);
		}

		public override SqlRepositoryConfig Build()
		{
			var sqlRepositoryConfig = new SqlRepositoryConfig();
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