using CacheRepository.ConnectionResolvers;
using CacheRepository.Repositories;

namespace CacheRepository.Configuration.Configs
{
	public class SqlRepositoryConfig : RepositoryConfig<SqlRepository>
	{
		internal SqlRepositoryConfig() {}
		public SqlConnectionResolver SqlConnectionResolver { get; set; }
		public override SqlRepository BuildRepository()
		{
			return new SqlRepository(this);
		}
	}
}