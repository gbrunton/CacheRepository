using System;
using System.Collections.Generic;
using System.Linq;
using CacheRepository.Configuration.Configs;
using CacheRepository.ConnectionResolvers;
using CacheRepository.DisposeStrategies;
using CacheRepository.FileEntityFactoryStrategies;
using CacheRepository.Indexes;
using CacheRepository.InsertStrategies;
using CacheRepository.NextIdStrategies;
using CacheRepository.OutputConventions;
using CacheRepository.SetIdStrategy;
using FubuCore.Util;

namespace CacheRepository.Configuration.Builders
{
	public class FileRepositoryConfigBuilder
	{
		private readonly string rootPathFolder;
		private string fileExtension;
		private IEnumerable<IIndex> indexes;
		private IFileEntityFactoryStrategy fileEntityFactoryStrategy;
		private IDisposeStrategy disposeStrategy;
		private IInsertStrategy insertStrategy;
		private INextIdStrategy nextIdStrategy;
		private ISetIdStrategy setIdStrategy;
		private IEnumerable<IOutputConvention> outputConventions;
		private string delimitor;
		private string fieldQualifier;
		private Cache<Type, EntityPropertiesForFile> entityPropertiesForFiles;

		public FileRepositoryConfigBuilder(string rootPathFolder)
		{
			if (rootPathFolder == null) throw new ArgumentNullException("rootPathFolder");
			this.rootPathFolder = rootPathFolder;
			this.fileExtension = ".txt";
			this.delimitor = string.Empty;
			this.fieldQualifier = string.Empty;
			this.indexes = new List<IIndex>();
			this.nextIdStrategy = new SmartNextIdRetreiver();
			this.setIdStrategy = new SmartEntityIdSetter();
			this.outputConventions = new List<IOutputConvention>();
			this.entityPropertiesForFiles = new Cache<Type, EntityPropertiesForFile>();
		}

		public FileRepositoryConfig Build()
		{
			this.entityPropertiesForFiles.OnMissing = entityType => new EntityPropertiesForFile(entityType);
			var filePathResolver = new FilePathResolver(this.entityPropertiesForFiles, this.rootPathFolder, this.fileExtension);
			var connectionResolver = new FileConnectionResolver(filePathResolver);
			this.disposeStrategy = this.disposeStrategy ?? new DisposeConnectionResolver(connectionResolver);
			this.insertStrategy = this.insertStrategy ?? new FileInsert(connectionResolver, this.outputConventions, this.delimitor, this.fieldQualifier);
			this.fileEntityFactoryStrategy = this.fileEntityFactoryStrategy ?? 
												(string.IsNullOrEmpty(this.delimitor)
												 ? (IFileEntityFactoryStrategy) new ConstructorContainsLine()
				                                 : new ConstructByEvaluatingDelimitedFile(this.delimitor, this.fieldQualifier));
			return new FileRepositoryConfig
				{
					ConnectionResolver = connectionResolver,
					Indexes = this.indexes,
					EntityFactoryStrategy = this.fileEntityFactoryStrategy,
					DisposeStrategy = this.disposeStrategy,
					InsertStrategy = this.insertStrategy,
					NextIdStrategy = this.nextIdStrategy,
					SetIdStrategy = this.setIdStrategy,
				};
		}

		public FileRepositoryConfigBuilder WithFileExtension(string newValue)
		{
			this.fileExtension = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithFileDelimitor(string newValue)
		{
			this.delimitor = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithFieldQualifier(string newValue)
		{
			this.fieldQualifier = newValue;
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

		public FileRepositoryConfigBuilder WithOutputConventions(IEnumerable<IOutputConvention> newValue)
		{
			this.outputConventions = newValue ?? new List<IOutputConvention>();
			return this;
		}

		public FileRepositoryConfigBuilder WithOutputConventions(params IOutputConvention[] newValue)
		{
			this.outputConventions = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithFileEntityFactoryStrategy(IFileEntityFactoryStrategy newValue)
		{
			this.fileEntityFactoryStrategy = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithNextIdStrategy(INextIdStrategy newValue)
		{
			this.nextIdStrategy = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithSetIdStrategy(ISetIdStrategy newValue)
		{
			this.setIdStrategy = newValue;
			return this;
		}

		public FileRepositoryConfigBuilder WithEntityPropertiesForFiles(IEnumerable<EntityPropertiesForFile> newValue)
		{
			if (newValue == null) throw new ArgumentNullException("newValue");
			this.entityPropertiesForFiles = new Cache<Type, EntityPropertiesForFile>
				(
					newValue
					.ToDictionary(entityPropertiesForFiles => entityPropertiesForFiles.EntityType, entityPropertiesForFiles => entityPropertiesForFiles)
				);
			return this;
		}
	}
}