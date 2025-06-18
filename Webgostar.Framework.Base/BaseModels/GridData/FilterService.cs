using System.Linq.Expressions;
using System.Reflection;

namespace Webgostar.Framework.Base.BaseModels.GridData
{
    public static class FilterService
    {
        public static IQueryable<T> PagingList<T>(this IQueryable<T> query, BaseGrid baseGrid)
        {
            try
            {
                var skip = (baseGrid.CurrentPage - 1) * baseGrid.Limit;

                if (string.IsNullOrWhiteSpace(baseGrid.OrderField)) return query.Skip(skip).Take(baseGrid.Limit);

                var typeModel = typeof(T).GetProperty(baseGrid.OrderField);

                if (typeModel == null) return query.Skip(skip).Take(baseGrid.Limit);

                var orderProperty = GetGetter<T, object>(typeModel);

                query = baseGrid.OrderType == OrderType.Ascending ? query.OrderBy(orderProperty) : (IQueryable<T>)query.OrderByDescending(orderProperty);

                return query.Skip(skip).Take(baseGrid.Limit);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IQueryable<T> FilterList<T>(this IQueryable<T> query, BaseGrid baseGrid)
        {
            try
            {
                foreach (var i in baseGrid.FilterParam)
                {
                    var filter = TranslationException<T>(i.Key, i.Value);

                    query = query.Where(filter);
                }

                if (string.IsNullOrWhiteSpace(baseGrid.OrderField)) return query;

                var orderProperty = GetGetter<T, object>(typeof(T).GetProperty(baseGrid.OrderField));

                query = baseGrid.OrderType == OrderType.Ascending ?
                    query.OrderBy(orderProperty) :
                        (IQueryable<T>)query.OrderByDescending(orderProperty);

                return query;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IQueryable<T> FilterPagingList<T>(this IQueryable<T> query, BaseGrid baseGrid)
        {
            try
            {
                var skip = (baseGrid.CurrentPage - 1) * baseGrid.Limit;

                foreach (var i in baseGrid.FilterParam)
                {
                    var filter = TranslationException<T>(i.Key, i.Value);

                    query = query.Where(filter);
                }

                if (string.IsNullOrWhiteSpace(baseGrid.OrderField)) return query.Skip(skip).Take(baseGrid.Limit);

                var orderProperty = GetGetter<T, object>(typeof(T).GetProperty(baseGrid.OrderField));

                query = baseGrid.OrderType == OrderType.Ascending ?
                    query.OrderBy(orderProperty) :
                    (IQueryable<T>)query.OrderByDescending(orderProperty);

                return query.Skip(skip).Take(baseGrid.Limit);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Expression<Func<T, bool>> TranslationException<T>(string key, string value)
        {
            try
            {
                var expression = GetGetter<T>(key);

                var type = expression.Body.Type;

                return type switch
                {
                    Type _ when type == typeof(string) => StringContainsOrdinalIgnoreCase<T>(expression, value),
                    Type _ when type.BaseType == typeof(Enum) => EnumTranslationContains<T>(expression, value),
                    Type _ when type == typeof(long) => NumberTranslationContains<T>(expression, value),
                    _ => null
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Expression<Func<T, bool>> EnumTranslationContains<T>(Expression<Func<T, object>> expression,
            string value)
        {
            try
            {
                var method = typeof(FilterService).GetMethod(nameof(EnumTranslationContains),
                    [typeof(Enum), typeof(string)]);

                var enumContains = Expression.Call(method, expression.Body,
                    Expression.Constant(value));

                return Expression.Lambda<Func<T, bool>>(enumContains, expression.Parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Expression<Func<T, bool>> EnumTranslationContains<T>(LambdaExpression expression, string value)
        {
            try
            {
                var method = typeof(FilterService).GetMethod(nameof(EnumTranslationContains),
                    [typeof(Enum), typeof(string)]);

                var enumContains = Expression.Call(method, expression.Body,
                    Expression.Constant(value));

                return Expression.Lambda<Func<T, bool>>(enumContains, expression.Parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Expression<Func<T, bool>> NumberTranslationContains<T>(Expression<Func<T, object>> expression,
            string value)
        {
            try
            {
                return CallMethodType(expression,
                    typeof(long),
                    nameof(long.Equals),
                    [typeof(long)],
                    [value]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Expression<Func<T, bool>> NumberTranslationContains<T>(LambdaExpression expression, string value)
        {
            try
            {
                return CallMethodType<T>(expression,
                    typeof(long),
                    nameof(long.Equals),
                    [typeof(long)],
                    [value]);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Expression<Func<T, bool>> StringContainsOrdinalIgnoreCase<T>(Expression<Func<T, object>> expression,
            string value)
        {
            try
            {
                return CallMethodType(expression,
                    typeof(string),
                    nameof(string.Contains),
                    [typeof(string)],
                    [value]);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Expression<Func<T, bool>> StringContainsOrdinalIgnoreCase<T>(LambdaExpression expression, string value)
        {
            try
            {
                return CallMethodType<T>(expression,
                    typeof(string),
                    nameof(string.Contains),
                    [typeof(string)],
                    [value]);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Expression<Func<T, bool>> CallMethodType<T>(Expression<Func<T, object>> expression,
            Type type,
            string method,
            Type[] parameters,
            object[] values)
        {
            try
            {
                var methodInfo = type.GetMethod(method, parameters);

                var vals = values.OrEmptyIfNull().Select(Expression.Constant);

                var methodCall = Expression.Call(expression.Body, methodInfo, vals);

                var lambda = Expression.Lambda<Func<T, bool>>(methodCall, expression.Parameters);

                return lambda;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static Expression<Func<T, bool>> CallMethodType<T>(LambdaExpression expression,
            Type type,
            string method,
            Type[] parameters,
            object[] values)
        {
            try
            {
                var methodInfo = type.GetMethod(method, parameters);

                var obhVals = new object[values.Length];

                for (var i = 0; i < values.Length; i++)
                {
                    obhVals[i] = Convert.ChangeType(values[i], type);
                }

                var vals = obhVals.OrEmptyIfNull().Select(Expression.Constant);

                var methodCall = Expression.Call(expression.Body, methodInfo, vals);

                var lambda = Expression.Lambda<Func<T, bool>>(methodCall, expression.Parameters);

                return lambda;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool EnumTranslationContains(object value, string text)
        {
            try
            {
                return value is Enum enumValue && enumValue.ToString().Contains(text, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            try
            {
                return source ?? [];
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Expression<Func<T, P>> GetGetter<T, P>(string propName)
        {
            try
            {
                return GetGetter<T, P>(typeof(T).GetProperty(propName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Expression<Func<T, P>> GetGetter<T, P>(PropertyInfo propInfo)
        {
            try
            {
                var entityParameter = Expression.Parameter(typeof(T), "e");
                Expression propertyAccess = Expression.Property(entityParameter, propInfo);
                var funcType = typeof(Func<,>).MakeGenericType(typeof(T), propInfo.PropertyType);
                var result = Expression.Lambda(funcType, propertyAccess, entityParameter);

                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(parameter, propInfo);
                Expression conversion = Expression.Convert(property, typeof(object));
                return Expression.Lambda<Func<T, P>>(conversion, parameter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static LambdaExpression GetGetter<T>(string propName)
        {
            try
            {
                return GetGetter<T>(typeof(T).GetProperty(propName));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static LambdaExpression GetGetter<T>(PropertyInfo propInfo)
        {
            try
            {
                var entityParameter = Expression.Parameter(typeof(T), "e");
                Expression propertyAccess = Expression.Property(entityParameter, propInfo);
                var funcType = typeof(Func<,>).MakeGenericType(typeof(T), propInfo.PropertyType);
                return Expression.Lambda(funcType, propertyAccess, entityParameter);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
