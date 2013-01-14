using CacheRepository.ConnectionResolvers;
using CacheRepository.Repositories;

namespace CacheRepository.Configuration.Configs
{
	public class SqlRepositoryConfig : RepositoryConfig<SqlRepository, SqlConnectionResolver>
	{
		internal SqlRepositoryConfig() {}

		public override SqlRepository BuildRepository()
		{
			return new SqlRepository(this);
		}
	}
}