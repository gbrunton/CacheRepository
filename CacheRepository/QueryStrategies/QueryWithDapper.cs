using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using Dapper;

namespace CacheRepository.QueryStrategies
{
	public class QueryWithDapper : IQueryStrategy
	{
		private readonly SqlConnectionResolver sqlConnectionResolver;

		public QueryWithDapper(SqlConnectionResolver sqlConnectionResolver)
		{
			if (sqlConnectionResolver == null) throw new ArgumentNullException("sqlConnectionResolver");
			this.sqlConnectionResolver = sqlConnectionResolver;
		}

		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class
		{
			return this.sqlConnectionResolver
			           .GetConnection()
			           .Query<TEntity>(query, parameters, this.sqlConnectionResolver.GetTransaction(), true, 0);
		}
	}
}