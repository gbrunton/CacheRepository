using System;
using System.Reflection;

namespace CacheRepository.OutputConventions
{
	public interface IOutputConvention
	{
		bool ShouldBeApplied(Type entityType, PropertyInfo property, object valueAsObj);
		object Apply(object valueAsObj);
	}
}