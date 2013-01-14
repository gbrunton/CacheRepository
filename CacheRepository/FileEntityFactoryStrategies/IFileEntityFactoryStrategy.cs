namespace CacheRepository.FileEntityFactoryStrategies
{
	public interface IFileEntityFactoryStrategy
	{
		dynamic Create<TEntity>(string line) where TEntity : class;
	}
}