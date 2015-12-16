using System.Collections.Generic;
using CacheRepository.Utils;

namespace CacheRepository.Indexes
{
	public abstract class NonUniqueIndex<TEntity, TKey> : Index<TEntity, TKey>
	{
		private Cache<TKey, List<TEntity>> cache;

		public NonUniqueIndex()
		{
			this.cache = new Cache<TKey, List<TEntity>>
			{
				OnMissing = key => new List<TEntity>()
			};
		}

		public override void Add(object entityAsObject)
		{
			var entity = (TEntity)entityAsObject;
			var key = GetKey(entity);
			this.cache[key].Add(entity);
		}

		public IEnumerable<TEntity> Get(TKey key)
		{
			return this.cache[key];
		}
	}
}