using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.DisposeStrategies;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.Indexes;
using CacheRepository.Repositories;

namespace CacheRepository.Configuration.Configs
{
	public class FileRepositoryConfig
	{
		public FileConnectionResolver ConnectionResolver { get; set; }
		public IEnumerable<IIndex> Indexes { get; set; }
		public IFileEntityFactoryStrategy EntityFactoryStrategy { get; set; }
		public IDisposeStrategy DisposeStrategy { get; set; }

		public FileRepository BuildRepository()
		{
			return new FileRepository(this);
		}
	}
}