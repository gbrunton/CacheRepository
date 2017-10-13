using System;
using System.Collections.Generic;
using System.Linq;
using CacheRepository.Utils;
using ServiceStack.Text;

namespace CacheRepository.Indexes
{
	public abstract class Index<TEntity, TKey> : IIndex
	{
	    protected Cache<TKey, List<TEntity>> Cache;

	    protected Index()
	    {
	        this.Cache = new Cache<TKey, List<TEntity>>
	        {
	            OnMissing = key => new List<TEntity>()
	        };
	    }

	    public IDictionary<string, List<TEntity>> CacheForSerialization
	    {
	        get
	        {
                return this.Cache.ToDictionary().ToDictionary(entry => JsonSerializer.SerializeToString(entry.Key), entry => entry.Value);
            }
	        set
	        {
	            this.Cache = new Cache<TKey, List<TEntity>>(value.ToDictionary(entry => JsonSerializer.DeserializeFromString<TKey>(entry.Key), entry => entry.Value))
                {
                    OnMissing = key => new List<TEntity>()
                };
            }
	    }

        public abstract void Add(object entity);
	    protected abstract TKey GetKey(TEntity entity);
	    public bool HasBeenHydrated { get; set; }
        public Type GetEntityType()
		{
			return typeof(TEntity);
		}
	}
}