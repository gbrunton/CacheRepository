using CacheRepository.ConnectionResolvers;

namespace CacheRepository.UpdateStrategies
{
	public interface IUpdateStrategy
	{
		void Update<TEntity>(TEntity entity) where TEntity : class;
		ISqlConnectionResolver ConnectionResolver { set; }
	}
}