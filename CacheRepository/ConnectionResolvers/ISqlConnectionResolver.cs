using System;
using System.Data;

namespace CacheRepository.ConnectionResolvers
{
	public interface ISqlConnectionResolver : IDisposable
	{
		IDbConnection GetConnection();
		IDbTransaction GetTransaction();
	}
}