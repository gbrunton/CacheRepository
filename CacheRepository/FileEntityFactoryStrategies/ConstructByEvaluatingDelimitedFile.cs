using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;
using CacheRepository.StaticUtils;

namespace CacheRepository.FileEntityFactoryStrategies
{
	public class ConstructByEvaluatingDelimitedFile : IFileEntityFactoryStrategy
	{
		private readonly FilePathResolver filePathResolver;
		private readonly string delimitor;
		private readonly string fieldQualifier;

		public ConstructByEvaluatingDelimitedFile(FilePathResolver filePathResolver, string delimitor, string fieldQualifier)
		{
			if (filePathResolver == null) throw new ArgumentNullException("filePathResolver");
			if (delimitor == null) throw new ArgumentNullException("delimitor");
			this.filePathResolver = filePathResolver;
			this.delimitor = delimitor;
			this.fieldQualifier = fieldQualifier;
		}

		public dynamic Create<TEntity>(string line) where TEntity : class
		{
			if (line == null) throw new ArgumentNullException("line");
			var splitLine = line.Split(new[] {this.delimitor}, StringSplitOptions.None);
			var entityType = typeof (TEntity);
			var entity = Activator.CreateInstance(entityType);
			var propertyInfos = entityType.GetProperties();

			for (var i = 0; i < propertyInfos.Length; i++)
			{
				var propertyInfo = propertyInfos[i];
				var valueAsString = splitLine[i].Trim(this.fieldQualifier);
				if (string.IsNullOrEmpty(valueAsString)) continue;
				var value = Convert.ChangeType(valueAsString, propertyInfo.PropertyType);
				propertyInfo.SetValue(entity, value);
			}

			return entity;
		}
	}
}