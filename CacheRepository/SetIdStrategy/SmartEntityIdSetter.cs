using System;
using CacheRepository.Utils;

namespace CacheRepository.SetIdStrategy
{
	public class SmartEntityIdSetter : ISetIdStrategy
	{
		private readonly Cache<Type, Action<dynamic, dynamic>> entityIdCache;

		public SmartEntityIdSetter() : this("Id")
		{
		}

		public SmartEntityIdSetter(string idPropertyName)
		{
			this.entityIdCache = new Cache<Type, Action<dynamic, dynamic>>
				{
					OnMissing = type => (entityId, entity) =>
						{
							if (type.GetProperty(idPropertyName) != null)
							{
								entity.Id = entityId;
							}
						}
				};
		}

		public void SetId(Type type, dynamic entityId, dynamic entity)
		{
			this.entityIdCache[type](entityId, entity);
		}
	}
}