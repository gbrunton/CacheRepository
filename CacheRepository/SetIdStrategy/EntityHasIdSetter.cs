using System;

namespace CacheRepository.SetIdStrategy
{
	public class EntityHasIdSetter : ISetIdStrategy
	{
		public void SetId(Type type, dynamic entityId, dynamic entity)
		{
			entity.Id = entityId;
		}
	}
}