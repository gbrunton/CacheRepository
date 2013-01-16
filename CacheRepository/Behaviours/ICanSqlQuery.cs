using System.Collections.Generic;

namespace CacheRepository.Behaviours
{
	public interface ICanSqlQuery
	{
		IEnumerable<TEntity> Query<TEntity>(string query, object parameters = null) where TEntity : class;
	}
}