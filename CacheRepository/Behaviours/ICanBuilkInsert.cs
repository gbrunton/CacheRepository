using System;
using System.Collections.Generic;

namespace CacheRepository.Behaviours
{
	public interface ICanBuilkInsert
	{
		void BuilkInsertNonGeneric(object entity);
		void BulkInsert<TEntity>(params TEntity[] entities) where TEntity : class;
		void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
		void BulkInsertFlush(IEnumerable<Type> flushOrder = null);
	}
}