using System;
using System.Data;
using System.Data.SqlClient;

namespace CacheRepository.ConnectionResolvers
{
	public class SqlConnectionResolver : ISqlConnectionResolver
	{
		private readonly ConnectionResolver connectionResolver;

		public SqlConnectionResolver(string connectionString)
		{
			if (connectionString == null) throw new ArgumentNullException("connectionString");
			this.connectionResolver = new ConnectionResolver(new SqlConnection(connectionString));
		}

		public IDbConnection GetConnection()
		{
			return this.connectionResolver.GetConnection();
		}

		public IDbTransaction GetTransaction()
		{
			return this.connectionResolver.GetTransaction();
		}

		public void Dispose()
		{
			this.connectionResolver.Dispose();
		}
	}
}