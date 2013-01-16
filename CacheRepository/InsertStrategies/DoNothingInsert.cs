namespace CacheRepository.InsertStrategies
{
	public class DoNothingInsert : IInsertStrategy
	{
		public void Insert<TEntity>(TEntity entity) where TEntity : class
		{
			
		}
	}
}