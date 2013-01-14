using CacheRepository.Configuration.Configs;

namespace CacheRepository.Repositories
{
	public class FileRepository : Repository
	{
		private readonly FileRepositoryConfig repositoryConfig;

		public FileRepository(FileRepositoryConfig repositoryConfig) : base(repositoryConfig)
		{
			this.repositoryConfig = repositoryConfig;
		}
	}
}