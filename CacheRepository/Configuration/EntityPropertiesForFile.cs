using System;

namespace CacheRepository.Configuration
{
	public class EntityPropertiesForFile
	{
		public EntityPropertiesForFile(Type entityType)
		{
			if (entityType == null) throw new ArgumentNullException("entityType");
			EntityType = entityType;
		}

		public Type EntityType { get; private set; }
		public string FileName { private get; set; }

		public string GetFileName()
		{
			return string.IsNullOrEmpty(this.FileName)
				       ? EntityType.Name
				       : this.FileName;
		}
	}
}