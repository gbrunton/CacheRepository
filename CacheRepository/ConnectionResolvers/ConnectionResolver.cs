﻿using System;
using System.Data;

namespace CacheRepository.ConnectionResolvers
{
	public class ConnectionResolver : ISqlConnectionResolver
	{
		private readonly IDbConnection connection;
		private IDbTransaction transaction;

		public ConnectionResolver(IDbConnection connection)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			this.connection = connection;
		}

		public IDbConnection GetConnection()
		{
			if (this.connection.State != ConnectionState.Open)
			{
				this.connection.Open();
				this.transaction = connection.BeginTransaction();
			}
			return connection;
		}

		public IDbTransaction GetTransaction()
		{
			return this.transaction;
		}

		public void Dispose()
		{
			this.transaction.Dispose();
			this.connection.Dispose();
		}	
	}
}