using System;

namespace CacheRepository.SetIdStrategy
{
	public interface ISetIdStrategy
	{
		void SetId(Type type, dynamic entityId, dynamic entity);
	}
}