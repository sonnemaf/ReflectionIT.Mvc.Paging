namespace ReflectionIT.Mvc.Paging; 

/// <summary>
/// Represents a paged subset of items including paging and sorting metadata,
/// as well as helpers to generate route values for paging and sorting links in Razor Pages.
/// </summary>
/// <typeparam name="T">The type of the items in the list. Must be a reference type.</typeparam>
public class PagingList<T> : List<T>, IPagingList<T> where T : class {

    /// <summary>
    /// Gets the 1-based index of the current page.
    /// </summary>
    public int PageIndex { get; }

    /// <summary>
    /// Gets the total number of available pages.
    /// </summary>
    public int PageCount { get; }

    /// <summary>
    /// Gets the total number of records across all pages.
    /// </summary>
    public int TotalRecordCount { get; }

    /// <summary>
    /// Gets or sets the action name used when generating paging links.
    /// Typically used by tag helpers or link generation in views.
    /// Defaults to <c>"Index"</c>.
    /// </summary>
    public string Action { get; set; }

    /// <summary>
    /// Gets or sets the route/query parameter name that holds the page index.
    /// Defaults to <see cref="PagingOptions.Current"/>.<see cref="PagingOptions.PageParameterName"/>
    /// </summary>
    public string PageParameterName { get; set; }

    /// <summary>
    /// Gets or sets the route/query parameter name that holds the sort expression.
    /// Defaults to <see cref="PagingOptions.Current"/>.<see cref="PagingOptions.SortExpressionParameterName"/>
    /// </summary>
    public string SortExpressionParameterName { get; set; }

    /// <summary>
    /// Gets the current sort expression. A leading <c>-</c> denotes descending order.
    /// When equal to <see cref="DefaultSortExpression"/>, the sort parameter is omitted from generated routes.
    /// </summary>
    public string SortExpression { get; } = string.Empty;

    /// <summary>
    /// Gets or sets a seed of route values to use for link generation.
    /// Values returned by the helper methods are based on this dictionary and
    /// will override specific entries for page and sort parameters as needed.
    /// </summary>
    public Microsoft.AspNetCore.Routing.RouteValueDictionary? RouteValue { get; set; }

    /// <summary>
    /// Gets the default sort expression. If the current <see cref="SortExpression"/>
    /// equals this value, generated routes will not include a sort parameter.
    /// </summary>
    public string DefaultSortExpression { get; } = string.Empty;

    /// <summary>
    /// Creates a new <see cref="PagingList{T}"/> from an ordered queryable sequence.
    /// </summary>
    /// <param name="qry">The ordered queryable source.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="pageIndex">The 1-based page index to retrieve.</param>
    /// <returns>A task that represents the asynchronous creation of a <see cref="PagingList{T}"/>.</returns>
    /// <remarks>This method is obsolete. Use <c>PagingList.CreateAsync&lt;T&gt;()</c> instead.</remarks>
    [Obsolete("Use PagingList.CreateAsync<T>() instead")]
    public static Task<PagingList<T>> CreateAsync(IOrderedQueryable<T> qry, int pageSize, int pageIndex) {
        return PagingList.CreateAsync(qry, pageSize, pageIndex);
    }

    /// <summary>
    /// Creates a new <see cref="PagingList{T}"/> from a queryable sequence with sorting.
    /// </summary>
    /// <param name="qry">The queryable source.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="pageIndex">The 1-based page index to retrieve.</param>
    /// <param name="sortExpression">The current sort expression. A leading <c>-</c> denotes descending order.</param>
    /// <param name="defaultSortExpression">The default sort expression used when none is specified.</param>
    /// <returns>A task that represents the asynchronous creation of a <see cref="PagingList{T}"/>.</returns>
    /// <remarks>This method is obsolete. Use <c>PagingList.CreateAsync&lt;T&gt;()</c> instead.</remarks>
    [Obsolete("Use PagingList.CreateAsync<T>() instead")]
    public static Task<PagingList<T>> CreateAsync(IQueryable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) {
        return PagingList.CreateAsync(qry, pageSize, pageIndex, sortExpression, defaultSortExpression);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagingList{T}"/> class with paging metadata.
    /// </summary>
    /// <param name="list">The items in the current page.</param>
    /// <param name="pageIndex">The 1-based index of the current page.</param>
    /// <param name="pageCount">The total number of pages.</param>
    /// <param name="totalRecordCount">The total number of records across all pages.</param>
    internal PagingList(List<T> list, int pageIndex, int pageCount, int totalRecordCount)
        : base(list) {
        this.TotalRecordCount = totalRecordCount;
        this.PageIndex = pageIndex;
        this.PageCount = pageCount;
        this.Action = "Index";
        this.PageParameterName = PagingOptions.Current.PageParameterName;
        this.SortExpressionParameterName = PagingOptions.Current.SortExpressionParameterName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagingList{T}"/> class with paging and sorting metadata.
    /// </summary>
    /// <param name="list">The items in the current page.</param>
    /// <param name="pageIndex">The 1-based index of the current page.</param>
    /// <param name="pageCount">The total number of pages.</param>
    /// <param name="sortExpression">The current sort expression. A leading <c>-</c> denotes descending order.</param>
    /// <param name="defaultSortExpression">The default sort expression used when none is specified.</param>
    /// <param name="totalRecordCount">The total number of records across all pages.</param>
    internal PagingList(List<T> list, int pageIndex, int pageCount, string sortExpression, string defaultSortExpression, int totalRecordCount)
        : this(list, pageIndex, pageCount, totalRecordCount) {

        this.SortExpression = sortExpression;
        this.DefaultSortExpression = defaultSortExpression;
    }

    /// <summary>
    /// Builds a route value dictionary for navigating to a specific page while preserving
    /// the current sort expression (unless equal to the default).
    /// </summary>
    /// <param name="pageIndex">The 1-based page index to navigate to.</param>
    /// <returns>
    /// A <see cref="Microsoft.AspNetCore.Routing.RouteValueDictionary"/> that includes the page parameter
    /// and, when applicable, the sort parameter merged with <see cref="RouteValue"/>.
    /// </returns>
    public Microsoft.AspNetCore.Routing.RouteValueDictionary GetRouteValueForPage(int pageIndex) {

        var dict = this.RouteValue == null ? new Microsoft.AspNetCore.Routing.RouteValueDictionary() :
                                             new Microsoft.AspNetCore.Routing.RouteValueDictionary(this.RouteValue);

        dict[this.PageParameterName] = pageIndex;

        if (this.SortExpression != this.DefaultSortExpression) {
            dict[this.SortExpressionParameterName] = this.SortExpression;
        }

        return dict;
    }

    /// <summary>
    /// Builds a string dictionary (suitable for tag helpers) for navigating to a specific page
    /// while preserving the current sort expression (unless equal to the default).
    /// </summary>
    /// <param name="pageIndex">The 1-based page index to navigate to.</param>
    /// <returns>
    /// A dictionary of string keys and values that includes the page parameter and, when applicable,
    /// the sort parameter merged with <see cref="RouteValue"/>.
    /// </returns>
    public Dictionary<string, string?> GetRouteDataForPage(int pageIndex) {
        var dict = this.RouteValue == null ? new Microsoft.AspNetCore.Routing.RouteValueDictionary() :
                                             new Microsoft.AspNetCore.Routing.RouteValueDictionary(this.RouteValue);

        dict[this.PageParameterName] = pageIndex;

        if (this.SortExpression != this.DefaultSortExpression) {
            dict[this.SortExpressionParameterName] = this.SortExpression;
        }

        return dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
    }

    /// <summary>
    /// Builds a route value dictionary that toggles or applies the provided sort expression.
    /// If the supplied expression equals the current <see cref="SortExpression"/>,    
    /// the expression is toggled between ascending and descending by adding/removing a leading <c>-</c>.
    /// </summary>
    /// <param name="sortExpression">The sort expression to apply or toggle.</param>
    /// <returns>
    /// A <see cref="Microsoft.AspNetCore.Routing.RouteValueDictionary"/> that includes the sort parameter
    /// merged with <see cref="RouteValue"/>.
    /// </returns>
    public Microsoft.AspNetCore.Routing.RouteValueDictionary GetRouteValueForSort(string sortExpression) {

        var dict = this.RouteValue == null ? new Microsoft.AspNetCore.Routing.RouteValueDictionary() :
                                             new Microsoft.AspNetCore.Routing.RouteValueDictionary(this.RouteValue);

        if (sortExpression == this.SortExpression) {
            sortExpression = this.SortExpression.StartsWith('-') ? sortExpression[1..] : $"-{sortExpression}";
        }

        dict[this.SortExpressionParameterName] = sortExpression;

        return dict;
    }

    /// <summary>
    /// Gets or sets the maximum number of page links to display in paging controls.
    /// Defaults to <see cref="PagingOptions.Current"/>.<see cref="PagingOptions.DefaultNumberOfPagesToShow"/>.
    /// </summary>
    public int NumberOfPagesToShow { get; set; } = PagingOptions.Current.DefaultNumberOfPagesToShow;

    /// <summary>
    /// Gets the first page index to show in a compact page number range based on the current page
    /// and <see cref="NumberOfPagesToShow"/>. The value is clamped to the valid page range.
    /// </summary>
    public int StartPageIndex {
        get {
            var half = (int)((this.NumberOfPagesToShow - 0.5) / 2);
            var start = Math.Max(1, this.PageIndex - half);
            if (start + this.NumberOfPagesToShow - 1 > this.PageCount) {
                start = this.PageCount - this.NumberOfPagesToShow + 1;
            }
            return Math.Max(1, start);
        }
    }

    /// <summary>
    /// Gets the last page index to show in a compact page number range based on the current page
    /// and <see cref="NumberOfPagesToShow"/>. The value is clamped to the valid page range.
    /// </summary>
    public int StopPageIndex => Math.Min(this.PageCount, this.StartPageIndex + this.NumberOfPagesToShow - 1);

}