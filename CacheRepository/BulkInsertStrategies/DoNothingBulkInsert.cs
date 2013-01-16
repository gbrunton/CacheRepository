using System;
using System.Collections.Generic;

namespace CacheRepository.BulkInsertStrategies
{
	public class DoNothingBulkInsert : IBulkInsertStrategy
	{
		public void Insert(Type type, IEnumerable<dynamic> entities)
		{
			
		}
	}
}