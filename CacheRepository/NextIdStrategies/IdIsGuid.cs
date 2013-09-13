using System;

namespace CacheRepository.NextIdStrategies
{
	public class IdIsGuid : INextIdStrategy
	{
		public dynamic GetNextId(Type type, Func<dynamic> getCurrentMaxId)
		{
			return Guid.NewGuid();
		}
	}
}