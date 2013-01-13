using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using Dapper;
using FubuCore;

namespace CacheRepository.EntityRetrieverStrategies
{
	public class SqlEntityRetrieverStrategy : IEntityRetrieverStrategy
	{
		public IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class
		{
			queryString = string.IsNullOrEmpty(queryString)
							  ? "Select * From [{0}]".ToFormat(typeof(TEntity).Name)
				              : queryString;
			return ConnectionResolver
				.GetConnection()
				.Query<TEntity>(queryString, null, ConnectionResolver.GetTransaction(), true, 0);
		}

		public ISqlConnectionResolver ConnectionResolver { set; private get; }
	}
}