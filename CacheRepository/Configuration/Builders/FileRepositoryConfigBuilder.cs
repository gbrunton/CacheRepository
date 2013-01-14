using System;
using CacheRepository.BulkInsertStrategies;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.EntityRetrieverStrategies;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.SetIdStrategy;
using CacheRepository.UpdateStrategies;

namespace CacheRepository.Configuration.Builders
{
	public class FileRepositoryConfigBuilder : RepositoryConfigBuilder<FileRepositoryConfigBuilder, FileRepositoryConfig>
	{
		private readonly FileConnectionResolver fileConnectionResolver;
		private string fileExtension;
		private ConstructorContainsLine fileEntityFactoryStrategy;

		public FileRepositoryConfigBuilder(FileConnectionResolver fileConnectionResolver) 
		{
			if (fileConnectionResolver == null) throw new ArgumentNullException("fileConnectionResolver");
			this.fileConnectionResolver = fileConnectionResolver;
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

		public override FileRepositoryConfig Build()
		{
			var repositoryConfig = new FileRepositoryConfig
				{
					FileConnectionResolver = this.fileConnectionResolver,
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
	}
}