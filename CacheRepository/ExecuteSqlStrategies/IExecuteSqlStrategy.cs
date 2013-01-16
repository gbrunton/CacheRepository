namespace CacheRepository.ExecuteSqlStrategies
{
	public interface IExecuteSqlStrategy
	{
		void Execute(string sql, object parameters);
	}
}