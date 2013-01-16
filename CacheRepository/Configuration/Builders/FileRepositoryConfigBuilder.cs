using System;
using System.Collections.Generic;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.DisposeStrategies;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.Indexes;

namespace CacheRepository.Configuration.Builders
{
	public class FileRepositoryConfigBuilder
	{
		private readonly string rootPathFolder;
		private string fileExtension;
		private IEnumerable<IIndex> indexes;
		private IFileEntityFactoryStrategy fileEntityFactoryStrategy;
		private IDisposeStrategy disposeStrategy;

		public FileRepositoryConfigBuilder(string rootPathFolder)
		{
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.rootPathFolder = rootPathFolder;
			this.fileExtension = ".txt";
			this.indexes = new List<IIndex>();
			this.fileEntityFactoryStrategy = new ConstructorContainsLine();
		}

		public FileRepositoryConfig Build()
		{
			var connectionResolver = new FileConnectionResolver(this.rootPathFolder, this.fileExtension);
			this.disposeStrategy = this.disposeStrategy ?? new DisposeConnectionResolver(connectionResolver);
			return new FileRepositoryConfig
				{
					ConnectionResolver = connectionResolver,
					Indexes = this.indexes,
					EntityFactoryStrategy = this.fileEntityFactoryStrategy,
					DisposeStrategy = this.disposeStrategy
				};
		}

		public FileRepositoryConfigBuilder WithFileExtension(string newValue)
		{
			this.fileExtension = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithIndexes(IEnumerable<IIndex> newValue)
		{
			this.indexes = newValue ?? new List<IIndex>();
			return this;
		}

		public FileRepositoryConfigBuilder WithIndexes(params IIndex[] newValue)
		{
			this.indexes = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithFileEntityFactoryStrategy(IFileEntityFactoryStrategy newValue)
		{
			this.fileEntityFactoryStrategy = newValue;
			return this;
		}
	}
}