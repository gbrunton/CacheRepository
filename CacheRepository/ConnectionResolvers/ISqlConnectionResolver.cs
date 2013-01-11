using System.Data;

namespace CacheRepository.ConnectionResolvers
{
	public interface ISqlConnectionResolver
	{
		IDbConnection GetConnection();
		IDbTransaction GetTransaction();		 
	}
}