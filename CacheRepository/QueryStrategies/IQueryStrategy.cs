using System.Collections.Generic;

namespace CacheRepository.QueryStrategies
{
	public interface IQueryStrategy
	{
		IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class;
	}
}