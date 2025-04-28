﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ReflectionIT.Mvc.Paging {
    internal static class LinqExtensions {
        private static PropertyInfo? GetPropertyInfo(Type objType, string name) {
            var matchedProperty = objType.GetProperty(name);
            return matchedProperty;
        }
        private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi) {
            var paramExpr = Expression.Parameter(objType);
            var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
            var expr = Expression.Lambda(propAccess, paramExpr);
            return expr;
        }

        public static IEnumerable<T> OrderBy<T>([NotNull] this IEnumerable<T>? query, string name) {
            ArgumentNullException.ThrowIfNull(query);

            var index = 0;
            var a = name.Split(',');
            foreach (var item in a) {
                var m = index++ > 0 ? "ThenBy" : "OrderBy";
                if (item.StartsWith("-")) {
                    m += "Descending";
                    name = item[1..];
                } else {
                    name = item;
                }
                name = name.Trim();
                
                PropertyInfo propInfo = GetPropertyInfo(typeof(T), name);
                LambdaExpression expr = GetOrderExpression(typeof(T), propInfo);
                var method = typeof(Enumerable).GetMethods().FirstOrDefault(mt => mt.Name == m && mt.GetParameters().Length == 2);
                if (method is not null) {
                    var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
                    query = (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() });
                }
            }
            return query;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string name) {
            var propInfo = GetPropertyInfo(typeof(T), name);
            var expr = GetOrderExpression(typeof(T), propInfo);

            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
            var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr });
        }
    }
}