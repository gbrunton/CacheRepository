namespace CacheRepository.InsertStrategies
{
	public interface IInsertStrategy
	{
		void Insert<TEntity>(TEntity entity) where TEntity : class;
	}
}