using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;

namespace CacheRepository.EntityRetrieverStrategies
{
	public interface IEntityRetrieverStrategy
	{
		//IEnumerable<dynamic> GetAll<TEntity>(Action<string> setQueryString, string queryString) where TEntity : class;
		IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class;
		ISqlConnectionResolver ConnectionResolver { set; }
	}
}