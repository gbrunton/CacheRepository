namespace CacheRepository.ConnectionResolvers
{
	public interface IFileConnectionResolver
	{
		void WriteLine<TEntity>(string line);
	}
}