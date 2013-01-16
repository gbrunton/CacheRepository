namespace CacheRepository.Behaviours
{
	public interface ICanUpdate
	{
		void Update<TEntity>(TEntity entity) where TEntity : class;
	}
}