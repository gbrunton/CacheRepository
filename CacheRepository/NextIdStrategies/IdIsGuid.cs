using System;

namespace CacheRepository.NextIdStrategies
{
	public class IdIsGuid : INextIdStrategy
	{
		public dynamic GetNextId(dynamic currentMaxId)
		{
			return Guid.NewGuid();
		}
	}
}