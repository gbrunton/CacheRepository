using System;
using CacheRepository.Utils;

namespace CacheRepository.FileEntityFactoryStrategies
{
	public class ConstructByEvaluatingDelimitedFile : IFileEntityFactoryStrategy
	{
		private readonly string delimitor;
		private readonly string fieldQualifier;

		public ConstructByEvaluatingDelimitedFile(string delimitor, string fieldQualifier)
		{
			if (delimitor == null) throw new ArgumentNullException("delimitor");
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

				var convertedValue = propertyInfo.PropertyType == typeof (Guid) 
					? Guid.Parse(valueAsString)
					: Convert.ChangeType(valueAsString, propertyInfo.PropertyType);
 
				propertyInfo.SetValue(entity, convertedValue, null);
			}

			return entity;
		}
	}
}