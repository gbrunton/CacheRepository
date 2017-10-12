using System.Collections.Generic;

namespace CacheRepository.Indexes
{
	public abstract class NonUniqueIndex<TEntity, TKey> : Index<TEntity, TKey>
	{
		public override void Add(object entityAsObject)
		{
			var entity = (TEntity)entityAsObject;
			var key = GetKey(entity);
			this.Cache[key].Add(entity);
		}

		public IEnumerable<TEntity> Get(TKey key)
		{
			return this.Cache[key];
		}
    }
}