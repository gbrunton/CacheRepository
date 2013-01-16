using System;
using CacheRepository.ConnectionResolvers;
using Dapper;

namespace CacheRepository.ExecuteSqlStrategies
{
	public class ExecuteSqlWithDapper : IExecuteSqlStrategy
	{
		private readonly SqlConnectionResolver sqlConnectionResolver;

		public ExecuteSqlWithDapper(SqlConnectionResolver sqlConnectionResolver)
		{
			if (sqlConnectionResolver == null) throw new ArgumentNullException("sqlConnectionResolver");
			this.sqlConnectionResolver = sqlConnectionResolver;
		}

		public void Execute(string sql, object parameters)
		{
			this.sqlConnectionResolver.GetConnection()
				.Execute(
				sql,
				parameters,
				this.sqlConnectionResolver.GetTransaction(),
				0);
		}
	}
}