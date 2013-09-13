using System.Data;

namespace CacheRepository.ConnectionResolvers
{
	public interface ISqlConnectionResolver : IConnectionResolver
	{
		IDbConnection GetConnection();
		IDbTransaction GetTransaction();
		void Commit();
	}
}