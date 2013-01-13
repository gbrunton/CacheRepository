using System;
using System.Collections.Generic;

namespace CacheRepository.BulkInsertStrategies
{
	public interface IBulkInsertStrategy
	{
		void Insert(Type type, IEnumerable<dynamic> entities);
	}
}