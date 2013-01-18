using System;

namespace CacheRepository.NextIdStrategies
{
	public interface INextIdStrategy
	{
		dynamic GetNextId(Type type, dynamic currentMaxId);
	}
}