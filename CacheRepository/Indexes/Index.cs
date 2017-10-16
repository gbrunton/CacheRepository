using System;
using System.Collections.Generic;
using CacheRepository.Utils;

namespace CacheRepository.Indexes
{
	public abstract class Index<TEntity, TKey> : IIndex
	{
	    protected readonly Cache<TKey, List<TEntity>> Cache;

	    protected Index()
	    {
	        this.Cache = new Cache<TKey, List<TEntity>>
	        {
	            OnMissing = key => new List<TEntity>()
	        };
	    }

        public abstract void Add(object entity);
	    protected abstract TKey GetKey(TEntity entity);
	    public bool HasBeenHydrated { get; set; }

	    public void Clear()
	    {
	        this.Cache.ClearAll();
	    }

	    public Type GetEntityType()
		{
			return typeof(TEntity);
		}
	}
}