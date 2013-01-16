using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using Dapper;
using FubuCore;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class SqlEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		private readonly SqlConnectionResolver connectionResolver;

		public SqlEntityRetrieverStrategy(SqlConnectionResolver connectionResolver)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
			this.connectionResolver = connectionResolver;
		}

		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
			queryString = string.IsNullOrEmpty(queryString)
							  ? "Select * From [{0}]".ToFormat(typeof(TEntity).Name)
				              : queryString;
			return this.connectionResolver
				.GetConnection()
				.Query<TEntity>(queryString, null, this.connectionResolver.GetTransaction(), true, 0);
		}
	}
}