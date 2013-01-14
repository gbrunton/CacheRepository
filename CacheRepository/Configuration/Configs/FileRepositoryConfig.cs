using CacheRepository.ConnectionResolvers;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.Repositories;

namespace CacheRepository.Configuration.Configs
{
	public class FileRepositoryConfig : RepositoryConfig<FileRepository, FileConnectionResolver>
	{
		internal FileRepositoryConfig() {}

		public string FileExtension { get; set; }
		public IFileEntityFactoryStrategy FileEntityFactoryStrategy { get; set; }

		public override FileRepository BuildRepository()
		{
			return new FileRepository(this);
		}
	}
}