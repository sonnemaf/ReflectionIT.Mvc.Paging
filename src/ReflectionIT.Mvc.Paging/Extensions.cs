using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ReflectionIT.Mvc.Paging;

/// <summary>
/// Extension methods for working with PagingList in Razor Pages and MVC views,
/// including generating sortable table headers, dynamic ordering, paging options configuration,
/// and route value utilities.
/// </summary>
public static class Extensions {

    /// <summary>
    /// Gets the display name for a model property when the model is a PagingList wrapper.
    /// </summary>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to PagingList of TModel.</param>
    /// <param name="expression">An expression identifying the property.</param>
    /// <returns>The display name for the specified property.</returns>
    public static string DisplayNameFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression) where TModel : class {
        return html.DisplayNameForInnerType<TModel, TValue>(expression);
    }

    /// <summary>
    /// Obsolete: Use the overloads without the pagingList parameter.
    /// </summary>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to PagingList of TModel.</param>
    /// <param name="expression">An expression identifying the property.</param>
    /// <param name="pagingList">Not used.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    [Obsolete("remove the pagingList parameter, it is not used any more", true)]
    public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, IPagingList pagingList) where TModel : class {
        var member = ((MemberExpression)expression.Body).Member;
        return SortableHeaderFor(html, expression, member.Name, pagingList);
    }

    /// <summary>
    /// Obsolete: Use the overloads without the pagingList parameter.
    /// </summary>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to PagingList of TModel.</param>
    /// <param name="expression">An expression identifying the property.</param>
    /// <param name="sortColumn">The sort column name to use in the route.</param>
    /// <param name="pagingList">Not used.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    [Obsolete("remove the pagingList parameter, it is not used any more", true)]
    public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn, IPagingList pagingList) where TModel : class {
        return SortableHeaderFor(html, expression, sortColumn);
    }

    /// <summary>
    /// Generates a sortable table header link for a property on the inner TModel when the page model exposes a PagingList via a selector.
    /// </summary>
    /// <typeparam name="TViewModel">The current view model type.</typeparam>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to TViewModel.</param>
    /// <param name="modelSelector">Selector to obtain the PagingList from the view model.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <param name="sortColumn">The sort column name to use in the route. Typically the property name, e.g., "Name" or nested "Customer.Name".</param>
    /// <param name="htmlAttributes">Optional HTML attributes for the generated anchor.</param>
    /// <returns>An HTML content instance for a sortable header with sort direction indicator when active.</returns>
    /// <remarks>
    /// When the current sort matches the provided sortColumn (or its negated form), a visual indicator is appended.
    /// </remarks>
    public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel, PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression, string sortColumn, object? htmlAttributes) where TModel : class {
        var pagingList = modelSelector(html.ViewData.Model);
        var bldr = new HtmlContentBuilder();

        bldr.AppendHtml(html.ActionLink(html.DisplayNameForInnerType(expression), pagingList.Action, pagingList.GetRouteValueForSort(sortColumn), htmlAttributes));

        if (pagingList.SortExpression == sortColumn || "-" + pagingList.SortExpression == sortColumn || pagingList.SortExpression == "-" + sortColumn) {
            bldr.AppendHtml(pagingList.SortExpression.StartsWith('-') ? PagingOptions.Current.HtmlIndicatorUp : PagingOptions.Current.HtmlIndicatorDown);
        }
        return bldr;
    }

    /// <summary>
    /// Generates a sortable table header link for a property on the inner TModel when the page model exposes a PagingList via a selector.
    /// </summary>
    /// <typeparam name="TViewModel">The current view model type.</typeparam>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to TViewModel.</param>
    /// <param name="modelSelector">Selector to obtain the PagingList from the view model.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <param name="sortColumn">The sort column name to use in the route.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel, PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression, string sortColumn) where TModel : class {
        return SortableHeaderFor(html: html, modelSelector: modelSelector, expression: expression, sortColumn: sortColumn, htmlAttributes: null);
    }

    /// <summary>
    /// Generates a sortable table header link for the specified property; the sort column is inferred from the expression's member name.
    /// </summary>
    /// <typeparam name="TViewModel">The current view model type.</typeparam>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to TViewModel.</param>
    /// <param name="modelSelector">Selector to obtain the PagingList from the view model.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel, PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression) where TModel : class {
        var member = ((MemberExpression)expression.Body).Member;
        return SortableHeaderFor(html, modelSelector, expression, member.Name);
    }

    /// <summary>
    /// Generates a sortable table header link for the specified property with custom HTML attributes; the sort column is inferred from the expression's member name.
    /// </summary>
    /// <typeparam name="TViewModel">The current view model type.</typeparam>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to TViewModel.</param>
    /// <param name="modelSelector">Selector to obtain the PagingList from the view model.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <param name="htmlAttributes">Optional HTML attributes for the generated anchor.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel, PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression, object htmlAttributes) where TModel : class {
        var member = ((MemberExpression)expression.Body).Member;
        return SortableHeaderFor(html, modelSelector, expression, member.Name, htmlAttributes);
    }

    /// <summary>
    /// Generates a sortable table header link for a PagingList-bound helper using an explicit sort column.
    /// </summary>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to PagingList of TModel.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <param name="sortColumn">The sort column name to use in the route.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn) where TModel : class {
        return SortableHeaderFor(html, expression, sortColumn, htmlAttributes: null);
    }

    /// <summary>
    /// Generates a sortable table header link for a PagingList-bound helper using an explicit sort column and attributes.
    /// </summary>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to PagingList of TModel.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <param name="sortColumn">The sort column name to use in the route.</param>
    /// <param name="htmlAttributes">Optional HTML attributes for the generated anchor.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn, object? htmlAttributes) where TModel : class {
        var bldr = new HtmlContentBuilder();
        bldr.AppendHtml(html.ActionLink(html.DisplayNameForInnerType(expression), html.ViewData.Model.Action, html.ViewData.Model.GetRouteValueForSort(sortColumn), htmlAttributes));
        var pagingList = html.ViewData.Model;

        if (pagingList.SortExpression == sortColumn || "-" + pagingList.SortExpression == sortColumn || pagingList.SortExpression == "-" + sortColumn) {
            bldr.AppendHtml(pagingList.SortExpression.StartsWith('-') ? PagingOptions.Current.HtmlIndicatorUp : PagingOptions.Current.HtmlIndicatorDown);
        }
        return bldr;
    }

    /// <summary>
    /// Generates a sortable table header link where the sort column is inferred from the expression's member name.
    /// </summary>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to PagingList of TModel.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression) where TModel : class {
        var member = ((MemberExpression)expression.Body).Member;
        return SortableHeaderFor(html, expression, member.Name);
    }

    /// <summary>
    /// Generates a sortable table header link with custom HTML attributes where the sort column is inferred from the expression's member name.
    /// </summary>
    /// <typeparam name="TModel">The inner model type contained by the PagingList.</typeparam>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="html">The HTML helper bound to PagingList of TModel.</param>
    /// <param name="expression">An expression identifying the inner model property.</param>
    /// <param name="htmlAttributes">Optional HTML attributes for the generated anchor.</param>
    /// <returns>An HTML content instance for a sortable header.</returns>
    public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes) where TModel : class {
        var member = ((MemberExpression)expression.Body).Member;
        return SortableHeaderFor(html, expression, member.Name, htmlAttributes);
    }

    /// <summary>
    /// Orders an <see cref="IQueryable{T}"/> dynamically based on a sort expression string.
    /// </summary>
    /// <typeparam name="T">The element type of the query.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="sortExpression">
    /// A sort expression supporting:
    /// - Comma-separated fields for multi-column sort, e.g., "Name,-Age".
    /// - Prefix '-' for descending order on a field, e.g., "-Created".
    /// - Nested properties using '.', e.g., "Customer.Name".
    /// </param>
    /// <returns>The ordered query.</returns>
    /// <exception cref="ArgumentException">Thrown when a specified property cannot be found.</exception>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string sortExpression) where T : class {
        var index = 0;
        var a = sortExpression.Split(',');
        foreach (var item in a) {
            var m = index++ > 0 ? "ThenBy" : "OrderBy";
            if (item.StartsWith('-')) {
                m += "Descending";
                sortExpression = item[1..];
            } else {
                sortExpression = item;
            }

            var mc = GenerateMethodCall<T>(source, m, sortExpression.TrimStart());
            source = source.Provider.CreateQuery<T>(mc);
        }
        return source;
    }

    /// <summary>
    /// Builds a lambda selector for the given property (supports nested paths with '.').
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="propertyName">The property name or nested path (e.g., "Customer.Name").</param>
    /// <param name="resultType">Outputs the property type resolved from the path.</param>
    /// <returns>A lambda expression like <c>Entity =&gt; Entity.Property</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when a property in the chain cannot be found.</exception>
    private static LambdaExpression GenerateSelector<TEntity>(string propertyName, out Type resultType) where TEntity : class {
        // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
        var parameter = Expression.Parameter(typeof(TEntity), "Entity");
        //  create the selector part, but support child properties
        PropertyInfo property;
        Expression propertyAccess;
        if (propertyName.Contains('.')) {
            // support to be sorted on child fields.
            var childProperties = propertyName.Split('.');
            property = typeof(TEntity).GetProperty(childProperties[0]) ?? throw new ArgumentException($"Property '{childProperties[0]}' not found", nameof(propertyName));
            propertyAccess = Expression.MakeMemberAccess(parameter, property);
            for (var i = 1; i < childProperties.Length; i++) {
                property = property.PropertyType.GetProperty(childProperties[i]) ?? throw new ArgumentException($"Property '{childProperties[i]}' not found", nameof(propertyName));
                propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
            }
        } else {
            property = typeof(TEntity).GetProperty(propertyName) ?? throw new ArgumentException($"{propertyName} property could not be found", nameof(propertyName)); ;
            propertyAccess = Expression.MakeMemberAccess(parameter, property);
        }
        resultType = property.PropertyType;
        // Create the order by expression.
        return Expression.Lambda(propertyAccess, parameter);
    }

    /// <summary>
    /// Generates the appropriate <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, System.Linq.Expressions.Expression{System.Func{TSource, TKey}})"/>
    /// or <c>ThenBy</c> method call expression for the provided source and field name.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="methodName">The LINQ method to invoke ("OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending").</param>
    /// <param name="fieldName">The property name or nested path.</param>
    /// <returns>A method call expression representing the order operation.</returns>
    private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, string fieldName) where TEntity : class {
        var type = typeof(TEntity);
        var selector = GenerateSelector<TEntity>(fieldName, out var selectorResultType);
        var resultExp = Expression.Call(typeof(Queryable), methodName,
                                        new Type[] { type, selectorResultType },
                                        source.Expression, Expression.Quote(selector));
        return resultExp;
    }

    /// <summary>
    /// Obsolete: This call is no longer required since moving to Razor Class Libraries.
    /// </summary>
    [Obsolete("This call is not required any move since we moved to Razor Class Libraries", true)]
    public static void AddPaging(this IServiceCollection services) {
    }

    /// <summary>
    /// Configures the global <see cref="PagingOptions"/> for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">An action to configure paging options.</param>
    public static void AddPaging(this IServiceCollection services, Action<PagingOptions> configureOptions) {
        configureOptions(PagingOptions.Current);
    }

    /// <summary>
    /// Gets a route value dictionary for a specific page index while excluding certain keys.
    /// </summary>
    /// <param name="pl">The paging list.</param>
    /// <param name="pageIndex">The destination page index.</param>
    /// <param name="excludes">Keys to exclude from the result.</param>
    /// <returns>A dictionary of route values as strings.</returns>
    public static Dictionary<string, string?> GetRouteData(this IPagingList pl, int pageIndex, params string[] excludes) {
        var dict = pl.GetRouteValueForPage(pageIndex);
        return dict.Where(kvp => kvp.Value is not null && !excludes.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => Convert.ToString(kvp.Value));
    }

}