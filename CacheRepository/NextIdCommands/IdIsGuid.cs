using System;

namespace CacheRepository.NextIdCommands
{
	public class IdIsGuid : INextIdCommand
	{
		public dynamic GetNextId(dynamic currentMaxId)
		{
			return Guid.NewGuid();
		}
	}
}