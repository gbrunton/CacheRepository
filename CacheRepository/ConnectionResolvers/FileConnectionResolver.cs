using System;
using System.IO;
using CacheRepository.Configuration;
using CacheRepository.Utils;

namespace CacheRepository.ConnectionResolvers
{
	public class FileConnectionResolver : IConnectionResolver, IFileConnectionResolver
	{
		private readonly Cache<Type, StreamContainer> streamReaderCache;
		private readonly Cache<Type, StreamContainer> streamWriterCache;
		private readonly Cache<Type, bool> doesFileExistCache;

		public FileConnectionResolver(Cache<Type, EntityPropertiesForFile> entityPropertiesForFiles, FilePathResolver filePathResolver)
		{
			if (entityPropertiesForFiles == null) throw new ArgumentNullException("entityPropertiesForFiles");
			if (filePathResolver == null) throw new ArgumentNullException("filePathResolver");
			this.doesFileExistCache = new Cache<Type, bool>
				{
					OnMissing = entityType => File.Exists(filePathResolver.GetPathToFile(entityType))
				};

			this.streamReaderCache = new Cache<Type, StreamContainer>
			{
				OnMissing = entityType =>
					{
						var fileStream = new FileStream(filePathResolver.GetPathToFile(entityType), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
						return new StreamContainer
							{
								FileStream = fileStream,
								StreamReader = new StreamReader(fileStream)
							};
					}
			};

			this.streamWriterCache = new Cache<Type, StreamContainer>
			{
				OnMissing = entityType =>
					{
						var fileStream = new FileStream
							(
								filePathResolver.GetPathToFile(entityType), 
								entityPropertiesForFiles[entityType].GetFileMode(), 
								FileAccess.Write, 
								FileShare.Read
							);
						return new StreamContainer
							{
								FileStream = fileStream,
								StreamWriter = new StreamWriter(fileStream)
							};
					}
			};
		}

		public string GetLine(Type entityType)
		{
			return !this.doesFileExistCache[entityType] 
				? null 
				: this.streamReaderCache[entityType].StreamReader.ReadLine();
		}

		public void WriteLine<TEntity>(string line)
		{
			var entityType = typeof (TEntity);
			this.streamWriterCache[entityType].StreamWriter.WriteLine(line);
			this.doesFileExistCache[entityType] = true;
		}

		public void Dispose()
		{
			this.streamReaderCache.Each(x => x.Dispose());
			this.streamWriterCache.Each(x => x.Dispose());
		}

		private class StreamContainer : IDisposable
		{
			public FileStream FileStream { private get; set; }
			public StreamReader StreamReader { get; set; }
			public StreamWriter StreamWriter { get; set; }

			public void Dispose()
			{
				if (this.StreamReader != null) this.StreamReader.Dispose();
				if (this.StreamWriter != null) this.StreamWriter.Dispose();
				this.FileStream.Dispose();
			}
		}
	}
}