namespace CacheRepository.NextIdCommands
{
	public class IdIsInt : INextIdCommand
	{
		public dynamic GetNextId(dynamic currentMaxId)
		{
			return currentMaxId + 1 ?? 1;
		}
	}
}