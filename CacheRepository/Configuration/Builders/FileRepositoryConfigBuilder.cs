using System;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.Repositories;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Builders
{
	public class FileRepositoryConfigBuilder 
		: RepositoryConfigBuilder<FileRepositoryConfigBuilder, FileRepositoryConfig, FileConnectionResolver, FileRepository>
	{
		private readonly string rootPathFolder;
		private string fileExtension;
		private IFileEntityFactoryStrategy fileEntityFactoryStrategy;

		public FileRepositoryConfigBuilder(string rootPathFolder) 
		{
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.rootPathFolder = rootPathFolder;
			this.fileExtension = ".txt";
			this.fileEntityFactoryStrategy = new ConstructorContainsLine();
			base.WithNextIdStrategy(new IdDoesNotExist());
			base.WithSetIdStrategy(new EntityHasNoIdSetter());
		}

		protected override IBulkInsertStrategy GetBulkInsertStrategy(FileRepositoryConfig repositoryConfig)
		{
			return null;
		}

		protected override IInsertStrategy GetInsertStrategy(FileRepositoryConfig repositoryConfig)
		{
			return null;
		}

		protected override IUpdateStrategy GetUpdateStrategy(FileRepositoryConfig repositoryConfig)
		{
			return null;
		}

		protected override IEntityRetrieverStrategy GetEntityRetrieverStrategy(FileRepositoryConfig repositoryConfig)
		{
			return new FileEntityRetrieverStrategy(repositoryConfig);
		}

		protected override FileConnectionResolver GetConnectionResolver(FileRepositoryConfig repositoryConfig)
		{
			return new FileConnectionResolver(this.rootPathFolder, repositoryConfig);
		}

		public override FileRepositoryConfig Build()
		{
			var repositoryConfig = new FileRepositoryConfig
				{
					FileExtension = this.fileExtension,
					FileEntityFactoryStrategy = this.fileEntityFactoryStrategy
				};
			base.BuildUp(repositoryConfig);
			return repositoryConfig;
		}

		protected override FileRepositoryConfigBuilder GetThis()
		{
			return this;
		}

		public FileRepositoryConfigBuilder WithFileExtension(string newValue)
		{
			this.fileExtension = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithFileEntityFactory(IFileEntityFactoryStrategy newValue)
		{
			this.fileEntityFactoryStrategy = newValue;
			return this;
		}
	}
}