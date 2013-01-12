using System;
using System.Linq;
using DapperExtensions.Mapper;

namespace CacheRepository.Configuration
{
	public class EntityIdIsAssigned<TEntityWithId> : AutoClassMapper<TEntityWithId> where TEntityWithId : class
	{
		protected override void AutoMap()
		{
			var type = typeof(TEntityWithId);
			var keyFound = Properties.Any(p => p.KeyType != KeyType.NotAKey);
			foreach (var propertyInfo in type.GetProperties())
			{
				if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
				{
					continue;
				}

				var map = Map(propertyInfo);

				if (!keyFound && string.Equals(map.PropertyInfo.Name, "Id", StringComparison.InvariantCultureIgnoreCase))
				{
					map.Key(KeyType.Assigned);
					keyFound = true;
				}
			}
		}
	}
}