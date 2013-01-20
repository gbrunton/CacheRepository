using System;
using System.Collections.Generic;
using System.Linq;
using CacheRepository.ConnectionResolvers;
using CacheRepository.OutputConventions;
using CacheRepository.StringFormatterAttributes;

namespace CacheRepository.InsertStrategies
{
	public class FileInsert : IInsertStrategy
	{
		private readonly IFileConnectionResolver connectionResolver;
		private readonly IEnumerable<IOutputConvention> outputConventions;
		private readonly string delimitor;
		private readonly string fieldQualifier;

		public FileInsert(IFileConnectionResolver connectionResolver, IEnumerable<IOutputConvention> outputConventions, string delimitor, string fieldQualifier)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
			if (delimitor == null) throw new ArgumentNullException("delimitor");
			if (fieldQualifier == null) throw new ArgumentNullException("fieldQualifier");
			this.connectionResolver = connectionResolver;
			this.outputConventions = outputConventions;
			this.delimitor = delimitor;
			this.fieldQualifier = fieldQualifier;
		}

		public void Insert<TEntity>(TEntity entity) where TEntity : class
		{
			var entityType = typeof (TEntity);
			var propertyInfos = entityType.GetProperties();
			var values = propertyInfos.Select(property =>
			{
				var valueAsObj = property.GetValue(entity, null);

				foreach (var outputConvention in this.outputConventions)
				{
					if (!outputConvention.ShouldBeApplied(entityType, property, valueAsObj)) continue;
					valueAsObj = outputConvention.Apply(valueAsObj);
				}

				foreach (var customAttribute in property.GetCustomAttributes(false))
				{
					var attribute = customAttribute as IStringFormatterAttribute;
					if (attribute == null) continue;
					valueAsObj = attribute.Format(valueAsObj);
				}

				var value = valueAsObj == null
								? ""
								: valueAsObj.ToString();
				return string.Concat(this.fieldQualifier, value, this.fieldQualifier);
			});
			this.connectionResolver.WriteLine<TEntity>(string.Join(this.delimitor, values));
		}
	}
}