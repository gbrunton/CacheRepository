using System;
using System.Data;
using System.Data.SqlClient;

namespace CacheRepository
{
	public class ConnectionResolver : ISqlConnectionResolver, IDisposable
	{
		private readonly string connectionString;
		private SqlTransaction transaction;
		private SqlConnection sqlConnection;

		public ConnectionResolver(string connectionString)
		{
			if (connectionString == null) throw new ArgumentNullException("connectionString");
			this.connectionString = connectionString;
		}

		public IDbConnection GetConnection()
		{
			if (this.sqlConnection == null)
			{
				this.sqlConnection = new SqlConnection(this.connectionString);
				this.sqlConnection.Open();
				this.transaction = sqlConnection.BeginTransaction();
			}
			return sqlConnection;
		}

		public IDbTransaction GetTransaction()
		{
			return this.transaction;
		}

		public void Dispose()
		{
			this.transaction.Commit();
			this.transaction.Dispose();
			this.sqlConnection.Dispose();
		}
	}
}