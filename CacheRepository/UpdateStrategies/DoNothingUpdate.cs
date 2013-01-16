namespace CacheRepository.UpdateStrategies
{
	public class DoNothingUpdate : IUpdateStrategy
	{
		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			
		}
	}
}