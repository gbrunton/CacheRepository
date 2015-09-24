using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.SqlQualifierStrategies;
using CacheRepository.Utils;
using Dapper;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class SqlEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		private readonly ISqlConnectionResolver connectionResolver;
	    private readonly ISqlQualifiers sqlQualifiers;

	    public SqlEntityRetrieverStrategy(ISqlConnectionResolver connectionResolver, ISqlQualifiers sqlQualifiers)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
	        if (sqlQualifiers == null) throw new ArgumentNullException("sqlQualifiers");
	        this.connectionResolver = connectionResolver;
	        this.sqlQualifiers = sqlQualifiers;
		}

		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
			queryString = string.IsNullOrEmpty(queryString)
                              ? "Select * From {0}{1}{2}".ToFormat(this.sqlQualifiers.BeginDelimitedIdentifier, typeof(TEntity).Name, this.sqlQualifiers.EndDelimitedIdentifier)
				              : queryString;
			return this.connectionResolver
				.GetConnection()
				.Query<TEntity>(queryString, null, this.connectionResolver.GetTransaction(), true, 0);
		}
	}
}