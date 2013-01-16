using System.Collections.Generic;

namespace CacheRepository.Behaviours
{
	public interface ICanInsert
	{
		void Insert<TEntity>(params TEntity[] entities) where TEntity : class;
		void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
	}
}