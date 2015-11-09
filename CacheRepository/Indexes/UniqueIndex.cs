using System;
using CacheRepository.Utils;

namespace CacheRepository.Indexes
{
	public abstract class UniqueIndex<TEntity, TKey> : Index<TEntity, TKey> where TEntity : class
	{
		private Cache<TKey, TEntity> cache;

		public UniqueIndex()
		{
			this.cache = new Cache<TKey, TEntity>
			{
				OnMissing = key => null
			};
		}

        public override dynamic Cache
        {
            get { return cache; }
            set { this.cache = value; }
        }

	    public override void Add(object entityAsObject)
		{
			var entity = (TEntity)entityAsObject;
			var key = GetKey(entity);
			if (this.cache.Has(key) && this.cache[key] != null)
			{
				// we need to see if this entity has been marked as containing dup keys.  If so then check this entity to see if it is the one we want to use. 
				// If it is then replace the existing entity with the same key otherwise do nothing.
				// If this entity has not been marked as containing dup keys then throw.
				var entityToUse = this.FindDuplicateEntityBasedOnKey(key, this.cache[key], entity);
				if (entityToUse == null) throw new Exception("Key '{0}' with value {1} for index '{2} is not unque".ToFormat(key.GetType().Name, key, this.GetType().Name));
				entity = entityToUse;
			}
			this.cache[key] = entity;
		}

		protected virtual TEntity FindDuplicateEntityBasedOnKey(TKey key, TEntity entityAlreadyCached, TEntity entity)
		{
			return null;
		}

		public TEntity Get(TKey key)
		{
			return this.cache[key];
		}
	}
}