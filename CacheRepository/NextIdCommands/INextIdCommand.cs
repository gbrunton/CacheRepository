namespace CacheRepository.NextIdCommands
{
	public interface INextIdCommand
	{
		dynamic GetNextId(dynamic currentMaxId);
	}
}