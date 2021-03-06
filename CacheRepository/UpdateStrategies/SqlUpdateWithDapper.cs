﻿using System;
using CacheRepository.ConnectionResolvers;
using DapperExtensions;

namespace CacheRepository.UpdateStrategies
{
	public class SqlUpdateWithDapper : IUpdateStrategy
	{
		private readonly ISqlConnectionResolver connectionResolver;

		public SqlUpdateWithDapper(ISqlConnectionResolver connectionResolver)
		{
			if (connectionResolver == null) throw new ArgumentNullException("connectionResolver");
			this.connectionResolver = connectionResolver;
		}

		public void Update<TEntity>(TEntity entity) where TEntity : class
		{
			// what happens if an indexed property gets updated???
			// See Issue #2
			this.connectionResolver
				.GetConnection()
				.Update(entity, this.connectionResolver.GetTransaction());
		}
	}
}