using System;
using System.IO;
using FubuCore.Util;

namespace CacheRepository.ConnectionResolvers
{
	public class FileConnectionResolver : IConnectionResolver, IFileConnectionResolver
	{
		private readonly Cache<Type, StreamReader> streamReaderCache;
		private readonly Cache<Type, StreamWriter> streamWriterCache;
		private readonly Cache<Type, bool> doesFileExistCache;

		public FileConnectionResolver(FilePathResolver filePathResolver)
		{
			if (filePathResolver == null) throw new ArgumentNullException("filePathResolver");
			this.doesFileExistCache = new Cache<Type, bool>
				{
					OnMissing = name => File.Exists(filePathResolver.GetPathToFile(name))
				};

			this.streamReaderCache = new Cache<Type, StreamReader>
				{
					OnMissing = name => File.OpenText(filePathResolver.GetPathToFile(name))
				};

			this.streamWriterCache = new Cache<Type, StreamWriter>
			{
				OnMissing = name => File.CreateText(filePathResolver.GetPathToFile(name))
			};
		}

		public string GetLine(Type entityType)
		{
			return !this.doesFileExistCache[entityType] 
				? null 
				: this.streamReaderCache[entityType].ReadLine();
		}

		public void WriteLine<TEntity>(string line)
		{
			var entityType = typeof (TEntity);
			this.doesFileExistCache[entityType] = true;
			this.streamWriterCache[entityType].WriteLine(line);
		}

		public void Dispose()
		{
			this.streamReaderCache.Each(x => x.Dispose());
			this.streamWriterCache.Each(x => x.Dispose());
		}
	}
}