using CacheRepository.ConnectionResolvers;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.Repositories;

namespace CacheRepository.Configuration.Configs
{
	public class FileRepositoryConfig : RepositoryConfig<FileRepository>
	{
		internal FileRepositoryConfig() {}

		public FileConnectionResolver FileConnectionResolver { get; set; }
		public string FileExtension { get; set; }
		public IFileEntityFactoryStrategy FileEntityFactoryStrategy { get; set; }

		public override FileRepository BuildRepository()
		{
			return new FileRepository(this);
		}
	}
}