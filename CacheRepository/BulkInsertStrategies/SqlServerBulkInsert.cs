using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using CacheRepository.Configuration.Configs;

namespace CacheRepository.BulkInsertStrategies
{
	public class SqlServerBulkInsert : IBulkInsertStrategy
	{
		private readonly SqlRepositoryConfig repositoryConfig;

		public SqlServerBulkInsert(SqlRepositoryConfig repositoryConfig)
		{
			if (repositoryConfig == null) throw new ArgumentNullException("repositoryConfig");
			this.repositoryConfig = repositoryConfig;
		}

		public void Insert(Type type, IEnumerable<dynamic> entities)
		{
			BulkInsert
				(
					type,
					(SqlConnection)this.repositoryConfig.ConnectionResolver.GetConnection(),
					(SqlTransaction)this.repositoryConfig.ConnectionResolver.GetTransaction(), 
					type.Name,
			        entities
				);
		}

		// I got this from here:
		// http://elegantcode.com/2012/01/26/sqlbulkcopy-for-generic-listt-useful-for-entity-framework-nhibernate/
		private static void BulkInsert(Type typeOfEntity, SqlConnection connection, SqlTransaction transaction, string tableName, IEnumerable<dynamic> list)
		{
			using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints, transaction))
			{
				bulkCopy.BulkCopyTimeout = 0;
				bulkCopy.DestinationTableName = tableName;

				var table = new DataTable();
				var props = TypeDescriptor.GetProperties(typeOfEntity)
					//Dirty hack to make sure we only have system data types 
					//i.e. filter out the relationships/collections
										.Cast<PropertyDescriptor>()
										.Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
										.ToArray();

				foreach (var propertyInfo in props)
				{
					bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
					table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
				}

				var listLength = 0;
				var values = new object[props.Length];
				foreach (var item in list)
				{
					listLength++;
					for (var i = 0; i < values.Length; i++)
					{
						values[i] = props[i].GetValue(item);
					}

					table.Rows.Add(values);
				}

				bulkCopy.BatchSize = listLength;
				bulkCopy.WriteToServer(table);
			}
		}
	}
}