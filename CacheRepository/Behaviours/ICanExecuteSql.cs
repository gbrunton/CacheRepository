namespace CacheRepository.Behaviours
{
	public interface ICanExecuteSql
	{
		void ExecuteSql(string sql, object parameters = null);
	}
}