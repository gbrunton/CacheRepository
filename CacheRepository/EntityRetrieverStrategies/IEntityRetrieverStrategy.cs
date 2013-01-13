using System.Collections.Generic;

namespace CacheRepository.EntityRetrieverStrategies
{
	public interface IEntityRetrieverStrategy
	{
		IEnumerable<dynamic> GetAll<TEntity>(string queryString) where TEntity : class;
	}
}