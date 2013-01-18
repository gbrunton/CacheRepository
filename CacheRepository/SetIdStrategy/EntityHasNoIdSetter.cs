using System;

namespace CacheRepository.SetIdStrategy
{
	public class EntityHasNoIdSetter : ISetIdStrategy
	{
		public void SetId(Type type, dynamic entityId, dynamic entity)
		{
			
		}
	}
}