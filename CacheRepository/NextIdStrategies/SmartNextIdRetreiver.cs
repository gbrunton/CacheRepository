using System;
using CacheRepository.Utils;

namespace CacheRepository.NextIdStrategies
{
	public class SmartNextIdRetreiver : INextIdStrategy
	{
		private readonly Cache<Type, Func<dynamic, dynamic>> entityIdCache;
		private readonly Cache<Type, Func<Type, dynamic, dynamic>> propertyTypeGetNextIdFunctionCache;

		public SmartNextIdRetreiver() : this("Id")
		{
		}

		public SmartNextIdRetreiver(string idPropertyName)
		{
			this.entityIdCache = new Cache<Type, Func<dynamic, dynamic>>
				{
					OnMissing = entityType => currentMaxId =>
						{
							var idProperty = entityType.GetProperty(idPropertyName);
							return idProperty == null 
								? null 
								: this.propertyTypeGetNextIdFunctionCache[idProperty.PropertyType](entityType, currentMaxId);
						}
				};

			this.propertyTypeGetNextIdFunctionCache = new Cache<Type, Func<Type, dynamic, dynamic>>()
			{
				OnMissing = propertyType =>
				{
					if (propertyType == typeof(int) || propertyType == typeof(long))
					{
						return (entityType, currentMaxId) => currentMaxId == null ? 1 : currentMaxId + 1;
					}
					if (propertyType == typeof(string))
					{
						return (entityType, currentMaxId) =>
							{
								var entityName = entityType.Name;
								if (string.IsNullOrEmpty(currentMaxId))
								{
									return entityName + "-1";
								}
								var split = currentMaxId.Split('-');
								var nextId = long.Parse(split[1]) + 1;
								return entityName + "-" + nextId;
							};
					}
					if (propertyType == typeof(Guid))
					{
						return (entityType, currentMaxId) => Guid.NewGuid();
					}
					throw new Exception("Next id of type '{0}' has not been defined".ToFormat(propertyType.Name));
				}
			};
		}

		public dynamic GetNextId(Type type, dynamic currentMaxId)
		{
			return this.entityIdCache[type](currentMaxId);
		}
	}
}