using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using CacheRepository.Utils;

namespace CacheRepository.EntityRetrieverStrategies.GetAllQueryStrategies
{
    public class CustomGetAllQuery : IGetAllQuery
    {
        public IEnumerable<dynamic> Execute<TEntity>(IDbConnection connection, string queryString, IDbTransaction transaction) where TEntity : class
        {
            var entityType = typeof(TEntity);

            // http://stackoverflow.com/a/29972767/384853
            var createEntity = Expression.Lambda<Func<TEntity>>(
                Expression.New(entityType.GetConstructor(Type.EmptyTypes))
                ).Compile();

            var entities = new List<TEntity>();
            var propertyInfos = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x).ToDictionary(x => x.Name);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = queryString;
                command.Transaction = transaction;
                using (var reader = command.ExecuteReader())
                {
                    var tuples = new List<Tuple<int, PropertyInfo>>();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var fieldName = reader.GetName(i);
                        PropertyInfo propertyInfo;
                        if (propertyInfos.TryGetValue(fieldName, out propertyInfo))
                        {
                            tuples.Add(new Tuple<int, PropertyInfo>(i, propertyInfo));
                        }
                    }

                    while (reader.Read())
                    {
                        var entity = createEntity();
                        entities.Add(entity);

                        tuples.Each(x =>
                        {
                            object value = null;
                            try
                            {
                                value = reader.GetValue(x.Item1);
                            } catch {}

                            if (value == null) return;

                            var changeToType = Nullable.GetUnderlyingType(x.Item2.PropertyType) ?? x.Item2.PropertyType;
                            x.Item2.SetValue(entity, Convert.ChangeType(value, changeToType), null);
                        });
                    }
                }
            }

            return entities;
        }
    }
}