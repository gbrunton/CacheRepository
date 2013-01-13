using System.Collections.Generic;
using CacheRepository.Configuration.Configs;
using Dapper;

namespace CacheRepository.Repositories
{
	public class SqlRepository : Repository
	{
		private readonly SqlRepositoryConfig sqlRepositoryConfig;

		public SqlRepository(SqlRepositoryConfig sqlRepositoryConfig) : base(sqlRepositoryConfig)
		{
			this.sqlRepositoryConfig = sqlRepositoryConfig;
		}

		public void ExecuteSql(string sql, object parameters = null)
		{
			this.sqlRepositoryConfig.SqlConnectionResolver.GetConnection()
				.Execute(
				sql,
				parameters,
				this.sqlRepositoryConfig.SqlConnectionResolver.GetTransaction(),
				0);
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null)
		{
			return this.sqlRepositoryConfig
				.SqlConnectionResolver
				.GetConnection()
				.Query<TEntity>
					(query, parameters, this.sqlRepositoryConfig.SqlConnectionResolver.GetTransaction(), true, 0);
		}
	}
}