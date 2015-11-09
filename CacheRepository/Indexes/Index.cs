using System;

namespace CacheRepository.Indexes
{
	public abstract class Index<TEntity, TKey> : IIndex
	{
		public abstract void Add(object entity);

		public Type GetEntityType()
		{
			return typeof(TEntity);
		}

		public bool HasBeenHydrated { get; set; }

	    public abstract dynamic Cache { get; set; }

	    protected abstract TKey GetKey(TEntity entity);
	}
}