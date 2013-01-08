using System;

namespace CacheRepository.Indexes
{
	public interface IIndex
	{
		void Add(object entity);
		Type GetEntityType();
		bool HasBeenHydrated { get; set; }		 
	}
}