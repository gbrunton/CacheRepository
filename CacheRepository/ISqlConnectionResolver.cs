using System.Data;

namespace CacheRepository
{
	public interface ISqlConnectionResolver
	{
		IDbConnection GetConnection();
		IDbTransaction GetTransaction();		 
	}
}