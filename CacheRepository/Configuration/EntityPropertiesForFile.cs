using System;

namespace CacheRepository.Configuration
{
	public class EntityPropertiesForFile
	{
		public enum FileMode
		{
			Overwrite,
			Append
		}

		public EntityPropertiesForFile(Type entityType)
		{
			if (entityType == null) throw new ArgumentNullException("entityType");
			EntityType = entityType;
		}

		public Type EntityType { get; private set; }
		public string FileName { private get; set; }
		public FileMode? FileModeSetter { private get; set; }

		public string GetFileName()
		{
			return string.IsNullOrEmpty(this.FileName)
				       ? EntityType.Name
				       : this.FileName;
		}

		public System.IO.FileMode GetFileMode()
		{
			if (this.FileModeSetter == null || this.FileModeSetter == FileMode.Append) return System.IO.FileMode.Append;
			return System.IO.FileMode.OpenOrCreate;
		}
	}
}