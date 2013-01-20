using System;
using System.IO;
using FubuCore.Util;

namespace CacheRepository.ConnectionResolvers
{
	public class FileConnectionResolver : IConnectionResolver, IFileConnectionResolver
	{
		private readonly Cache<string, StreamReader> streamReaderCache;
		private readonly Cache<string, StreamWriter> streamWriterCache;
		private readonly Cache<string, bool> doesFileExistCache;

		public FileConnectionResolver(FilePathResolver filePathResolver)
		{
			if (filePathResolver == null) throw new ArgumentNullException("filePathResolver");
			this.doesFileExistCache = new Cache<string, bool>
				{
					OnMissing = name => File.Exists(filePathResolver.GetPathToFile(name))
				};

			this.streamReaderCache = new Cache<string, StreamReader>
				{
					OnMissing = name => File.OpenText(filePathResolver.GetPathToFile(name))
				};

			this.streamWriterCache = new Cache<string, StreamWriter>
			{
				OnMissing = name => File.CreateText(filePathResolver.GetPathToFile(name))
			};
		}

		public string GetLine(string entityName)
		{
			return !this.doesFileExistCache[entityName] 
				? null 
				: this.streamReaderCache[entityName].ReadLine();
		}

		public void WriteLine<TEntity>(string line)
		{
			var entityName = typeof (TEntity).Name;
			this.doesFileExistCache[entityName] = true;
			this.streamWriterCache[entityName].WriteLine(line);
		}

		public void Dispose()
		{
			this.streamReaderCache.Each(x => x.Dispose());
			this.streamWriterCache.Each(x => x.Dispose());
		}
	}
}