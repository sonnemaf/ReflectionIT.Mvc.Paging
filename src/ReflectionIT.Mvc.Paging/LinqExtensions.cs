using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace ReflectionIT.Mvc.Paging;

/// <summary>
/// Provides reflection-based LINQ extension methods to dynamically order
/// in-memory sequences (IEnumerable) and query providers (IQueryable) by property name.
/// </summary>
/// <remarks>
/// - For IEnumerable sources, multiple sort keys can be specified using a comma-separated list.
///   Prefix a property with '-' to sort descending.
/// - For IQueryable sources, this overload applies a single ascending OrderBy. Use the returned
///   query to chain additional ordering operators if needed.
/// Property names are matched against public instance properties and are case-sensitive.
/// </remarks>
internal static class LinqExtensions {

    /// <summary>
    /// Builds a lambda expression that accesses the specified property on a given type.
    /// </summary>
    /// <param name="objType">The declaring type that contains the property.</param>
    /// <param name="pi">The property to access.</param>
    /// <returns>
    /// A lambda expression of the form <c>x =&gt; x.Property</c> where <c>x</c> is of type <paramref name="objType"/>.
    /// </returns>
    private static LambdaExpression GetOrderExpression(Type objType, PropertyInfo pi) {
        var paramExpr = Expression.Parameter(objType);
        var propAccess = Expression.PropertyOrField(paramExpr, pi.Name);
        var expr = Expression.Lambda(propAccess, paramExpr);
        return expr;
    }

    /// <summary>
    /// Orders an <see cref="IEnumerable{T}"/> by one or more property names using reflection.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The source sequence to order. Must not be <see langword="null"/>.</param>
    /// <param name="name">
    /// One or more property names separated by commas. Prefix a name with '-' to apply descending order.
    /// Example: <c>"LastName, FirstName, -Age"</c>.
    /// </param>
    /// <returns>The ordered sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="query"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">A specified property name does not exist on the element type.</exception>
    /// <remarks>
    /// Property names are matched case-sensitively against public instance properties.
    /// Nested properties are not supported.
    /// </remarks>
    public static IEnumerable<T> OrderBy<T>([NotNull] this IEnumerable<T>? query, string name) {
        ArgumentNullException.ThrowIfNull(query);
        var index = 0;
        var a = name.Split(',');
        foreach (var item in a) {
            var m = index++ > 0 ? "ThenBy" : "OrderBy";
            if (item.StartsWith('-')) {
                m += "Descending";
                name = item[1..];
            } else {
                name = item;
            }
            name = name.Trim();
            
            var propInfo = typeof(T).GetProperty(name) ?? throw new ArgumentException($"{name} property could not be found", nameof(name));
            var expr = GetOrderExpression(typeof(T), propInfo);
            var method = typeof(Enumerable).GetMethods().FirstOrDefault(mt => mt.Name == m && mt.GetParameters().Length == 2);
            if (method is not null) {
                var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
                query = (IEnumerable<T>)genericMethod.Invoke(null, new object[] { query, expr.Compile() })!;
            }
        }
        return query;
    }

    /// <summary>
    /// Orders an <see cref="IQueryable{T}"/> by a single property name in ascending order using reflection.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The source query to order.</param>
    /// <param name="name">The name of a public instance property on the element type.</param>
    /// <returns>The ordered query.</returns>
    /// <exception cref="ArgumentException">The property specified by <paramref name="name"/> does not exist on the element type.</exception>
    /// <exception cref="InvalidOperationException">The LINQ <c>OrderBy</c> method on <see cref="Queryable"/> could not be located.</exception>
    /// <remarks>
    /// This method applies only an ascending <c>OrderBy</c>. For additional sort keys, chain
    /// <c>ThenBy</c>/<c>ThenByDescending</c> using strongly typed expressions after the returned query.
    /// </remarks>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string name) {
        var propInfo = typeof(T).GetProperty(name) ?? throw new ArgumentException($"{name} property could not be found", nameof(name));
        var expr = GetOrderExpression(typeof(T), propInfo);

        var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2) ?? throw new InvalidOperationException($"OrderBy method could not be found");
        var genericMethod = method.MakeGenericMethod(typeof(T), propInfo.PropertyType);
        return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, expr })!;
    }
}