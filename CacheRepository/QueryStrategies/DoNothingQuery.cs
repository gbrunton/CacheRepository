using System.Collections.Generic;

namespace CacheRepository.QueryStrategies
{
	public class DoNothingQuery : IQueryStrategy
	{
		public IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class
		{
			return new List<TEntity>();
		}
	}
}