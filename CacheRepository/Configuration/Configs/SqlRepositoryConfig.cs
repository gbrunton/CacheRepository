using CacheRepository.ConnectionResolvers;
using CacheRepository.Repositories;

namespace CacheRepository.Configuration.Configs
{
	public class SqlRepositoryConfig : RepositoryConfig
	{
		internal SqlRepositoryConfig() {}
		public SqlConnectionResolver SqlConnectionResolver { get; set; }
		public override Repository BuildRepository()
		{
			return new SqlRepository(this);
		}
	}
}