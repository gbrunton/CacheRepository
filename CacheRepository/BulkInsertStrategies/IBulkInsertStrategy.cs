using System;
using System.Collections.Generic;
using CacheRepository.ConnectionResolvers;

namespace CacheRepository.BulkInsertStrategies
{
	public interface IBulkInsertStrategy
	{
		void Insert(Type type, IEnumerable<dynamic> entities);
		ISqlConnectionResolver ConnectionResolver { set; }
	}
}