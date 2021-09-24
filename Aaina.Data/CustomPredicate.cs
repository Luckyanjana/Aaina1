using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Aaina.Data
{
    public class CustomPredicate
    {

        public static async Task<GridResult> ToPaggedListAsync<TEntity>(IQueryable<TEntity> query, GridParameterModel model)
            where TEntity : class
        {
            var dataTableResult = new GridResult();

            foreach (var item in model.Columns)
            {
                var col = item.Data;
                if (!string.IsNullOrEmpty(item.Search.Value))
                {
                    const string spliter = "@#$";
                    var values = item.Search.Value.Split(new[] { spliter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        var names = item.Name.Split(new[] { spliter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        for (int i = 0; i < values.Count; i++)
                        {
                            var value = System.Net.WebUtility.HtmlDecode(values[i]);
                            var colType = GetColumnType<TEntity>(col);
                            var searchType = names[i];
                            bool isListSearch = false;
                            if (value.IndexOf("[\"") == 0 && value.LastIndexOf("]") == value.Length - 1 && value.Contains(","))
                            {
                                searchType = "or";
                                isListSearch = true;
                            }

                            switch (colType)
                            {
                                case "int":
                                case "int64":
                                    if (isListSearch)
                                    {
                                        var list = new List<string>();//JsonConvert.DeserializeObject<List<string>>(value.ToString());//TODO:Json library calling issue is coming
                                        foreach (var val in list)
                                        {
                                            query = WhereHelper(query, col, Convert.ToInt32(val), searchType);
                                        }
                                    }
                                    else
                                    {
                                        query = WhereHelper(query, col, Convert.ToInt32(value), searchType);
                                    }

                                    break;

                                case "short":
                                    query = WhereHelper(query, col, Convert.ToInt32(value), searchType);
                                    break;

                                case "decimal":
                                    query = WhereHelper(query, col, Convert.ToDecimal(value), searchType);
                                    break;

                                case "datetime":
                                    var searchDate = Convert.ToDateTime(value);
                                    //commented after discussion with rakesh sir
                                    if (Convert.ToDateTime(value).TimeOfDay == System.TimeSpan.FromSeconds(0) && searchType == "<=")
                                    {
                                        searchDate = searchDate.AddHours(24);
                                    }

                                    query = WhereHelper(query, col, searchDate, searchType);
                                    break;

                                case "bool":
                                    query = WhereHelper(query, col, Convert.ToBoolean(value), searchType);
                                    break;

                                case "guid":
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        query = WhereHelper(query, col, Guid.Parse(value), searchType);
                                    }

                                    break;

                                case "string":
                                    query = WhereHelper(query, col, value, searchType);

                                    break;
                            }
                        }
                    }
                }
            }

            //var parameterExpressions = new[] { Expression.Parameter(typeof(TEntity), string.Empty) };
            var parameterExpressions = new[] { Expression.Parameter(typeof(TEntity), string.Empty) };

            if (model.Order.Any())
            {
                if (model.Columns.ToList().Count > model.Order.FirstOrDefault().Column)
                {
                    var orderByField = model.Columns.ToList()[model.Order.FirstOrDefault().Column].Data;
                    var property = typeof(TEntity).GetProperty(orderByField);
                    if (property == null)
                    {
                        property = typeof(TEntity).GetProperty(model.Columns.FirstOrDefault(m => m.Data != string.Empty)?.Data);
                    }

                    query = query.Provider.CreateQuery<TEntity>(Expression.Call(typeof(Queryable), model.Order.FirstOrDefault()?.Dir == "asc" ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, query.Expression, Expression.Lambda(Expression.Property(parameterExpressions[0], property), parameterExpressions)));
                }
            }

            try
            {
                dataTableResult.Data = model.Length == -1
                    ? (await query.ToListAsync()).ToList<object>()
                    : (await query.Skip(model.Start).Take(model.Length).ToListAsync()).ToList<object>();
            }
            catch (Exception ex)
            {
            }
            dataTableResult.RecordsTotal = await query.CountAsync();
            dataTableResult.RecordsFiltered = dataTableResult.RecordsTotal;
            dataTableResult.Draw = model.Draw;

            return dataTableResult;
        }


        public static GridResult ToPaggedListSync<TEntity>(IQueryable<TEntity> query, GridParameterModel model)
            where TEntity : class
        {
            var dataTableResult = new GridResult();

            foreach (var item in model.Columns)
            {
                var col = item.Data;
                if (!string.IsNullOrEmpty(item.Search.Value))
                {
                    const string spliter = "@#$";
                    var values = item.Search.Value.Split(new[] { spliter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        var names = item.Name.Split(new[] { spliter }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        for (int i = 0; i < values.Count; i++)
                        {
                            var value = values[i];
                            var colType = GetColumnType<TEntity>(col);
                            var searchType = names[i];
                            switch (colType)
                            {
                                case "int":
                                case "int64":
                                    query = WhereHelper(query, col, Convert.ToInt32(value), searchType);
                                    break;

                                case "short":
                                    query = WhereHelper(query, col, Convert.ToInt32(value), searchType);
                                    break;

                                case "decimal":
                                    query = WhereHelper(query, col, Convert.ToDecimal(value), searchType);
                                    break;

                                case "datetime":
                                    var searchDate = Convert.ToDateTime(value);
                                    if (Convert.ToDateTime(value).TimeOfDay == System.TimeSpan.FromSeconds(0) && searchType == "<=")
                                    {
                                        searchDate = searchDate.AddHours(24);
                                    }

                                    query = WhereHelper(query, col, searchDate, searchType);
                                    break;

                                case "bool":
                                    query = WhereHelper(query, col, Convert.ToBoolean(value), searchType);
                                    break;

                                case "string":
                                    query = WhereHelper(query, col, value, searchType, false);
                                    break;
                            }
                        }
                    }
                }
            }

            var typeParams = new[] { Expression.Parameter(typeof(TEntity), string.Empty) };

            if (model.Order.Any())
            {
                if (model.Columns.ToList().Count > model.Order.FirstOrDefault().Column)
                {
                    var orderByField = model.Columns.ToList()[model.Order.FirstOrDefault().Column].Data;
                    var property = typeof(TEntity).GetProperty(orderByField);
                    if (property == null)
                    {
                        property = typeof(TEntity).GetProperty(model.Columns.FirstOrDefault(m => m.Data != string.Empty).Data);
                    }

                    query = query.Provider.CreateQuery<TEntity>(Expression.Call(typeof(Queryable), model.Order.FirstOrDefault().Dir == "asc" ? "OrderBy" : "OrderByDescending", new Type[] { typeof(TEntity), property.PropertyType }, query.Expression, Expression.Lambda(Expression.Property(typeParams[0], property), typeParams)));
                }
            }

            try
            {
                dataTableResult.Data = model.Length == -1
                    ? query.ToList().ToList<object>()
                    : query.Skip(model.Start).Take(model.Length).ToList().ToList<object>();
            }
            catch (Exception ex)
            {
                dataTableResult.Error = ex.Message;
            }

            dataTableResult.RecordsTotal = query.Count();
            dataTableResult.RecordsFiltered = dataTableResult.RecordsTotal;
            dataTableResult.Draw = model.Draw;
            return dataTableResult;
        }


        private static string GetColumnType<TEntity>(string columnName)
            where TEntity : class
        {
            string columnType = string.Empty;

            foreach (PropertyDescriptor propertyInfo in TypeDescriptor.GetProperties(typeof(TEntity)))
            {
                if (propertyInfo.Name == columnName)
                {
                    if (propertyInfo.PropertyType == typeof(Guid) || propertyInfo.PropertyType == typeof(Guid?))
                    {
                        columnType = "guid";
                    }
                    else if (propertyInfo.PropertyType == typeof(string))
                    {
                        columnType = "string";
                    }
                    else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
                    {
                        columnType = "int";
                    }
                    else if (propertyInfo.PropertyType == typeof(short) || propertyInfo.PropertyType == typeof(short?))
                    {
                        columnType = "short";
                    }
                    else if (propertyInfo.PropertyType == typeof(long) || propertyInfo.PropertyType == typeof(long?))
                    {
                        columnType = "int64";
                    }
                    else if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
                    {
                        columnType = "decimal";
                    }
                    else if (propertyInfo.PropertyType == typeof(byte) || propertyInfo.PropertyType == typeof(byte?))
                    {
                        columnType = "int";
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
                    {
                        columnType = "datetime";
                    }
                    else if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
                    {
                        columnType = "bool";
                    }
                }
            }

            return columnType;
        }


        private static IQueryable<TEntity> WhereHelper<TEntity>(IQueryable<TEntity> source, string columnName, object value, string filterType,
            bool isFromDB = false)
        {
            ParameterExpression table = Expression.Parameter(typeof(TEntity), string.Empty);
            Expression column = Expression.PropertyOrField(table, columnName);
            try
            {
                Expression valueExpression2 = Expression.Convert(Expression.Constant(value), column.Type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Expression valueExpression = Expression.Convert(Expression.Constant(value), column.Type);
            Expression where = null;

            switch (filterType)
            {
                case "<":
                    where = Expression.LessThan(column, valueExpression);
                    break;

                case "<=":
                    where = Expression.LessThanOrEqual(column, valueExpression);
                    break;

                case "=":
                    where = Expression.Equal(column, valueExpression);
                    break;

                case "or":
                    where = Expression.Or(column, valueExpression);
                    break;

                case ">":
                    where = Expression.GreaterThan(column, valueExpression);
                    break;

                case ">=":
                    where = Expression.GreaterThanOrEqual(column, valueExpression);
                    break;

                case "!=":
                    where = Expression.NotEqual(column, valueExpression);
                    break;

                case "contains":
                    where = isFromDB
                         ? Expression.Call(column, typeof(string).GetMethod("Contains"), valueExpression)
                         : where = Expression.Call(Expression.Call(column, "ToUpper", null), "Contains", null, Expression.Convert(Expression.Constant(value.ToString().ToUpper()), column.Type));
                    break;
            }

            Expression lambda = Expression.Lambda(where, new ParameterExpression[] { table });

            Type[] exprArgTypes = { source.ElementType };

            MethodCallExpression methodCall = Expression.Call(typeof(Queryable), "Where", exprArgTypes, source.Expression, lambda);

            return (IQueryable<TEntity>)source.Provider.CreateQuery<TEntity>(methodCall);
        }
    }
}
