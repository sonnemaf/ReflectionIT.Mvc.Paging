using Microsoft.AspNetCore.Routing;

namespace ReflectionIT.Mvc.Paging; 

/// <summary>
/// Represents a pageable list that exposes paging and sorting metadata,
/// as well as helpers to generate route values for links in UI components.
/// </summary>
/// <remarks>
/// Implementations provide information such as the current page, total pages,
/// total records and sort expression, and can construct <see cref="RouteValueDictionary"/>
/// instances to be used when generating paging and sorting links.
/// </remarks>
public interface IPagingList {

    /// <summary>
    /// Gets or sets the action or page name used when generating paging links.
    /// </summary>
    /// <value>
    /// The action (MVC) or page (Razor Pages) name.
    /// </value>
    string Action { get; set; }

    /// <summary>
    /// Creates a <see cref="RouteValueDictionary"/> containing the route values
    /// required to navigate to the specified page.
    /// </summary>
    /// <param name="pageIndex">The target page index to navigate to.</param>
    /// <returns>
    /// A <see cref="RouteValueDictionary"/> with the route values for the target page.
    /// </returns>
    RouteValueDictionary GetRouteValueForPage(int pageIndex);

    /// <summary>
    /// Creates a <see cref="RouteValueDictionary"/> containing the route values
    /// required to apply the specified sort expression.
    /// </summary>
    /// <param name="sortExpression">The sort expression to apply (e.g. column and direction).</param>
    /// <returns>
    /// A <see cref="RouteValueDictionary"/> with the route values for the specified sort.
    /// </returns>
    RouteValueDictionary GetRouteValueForSort(string sortExpression);

    /// <summary>
    /// Gets the total number of pages available.
    /// </summary>
    /// <value>
    /// The total page count across the entire data set.
    /// </value>
    int PageCount { get; }

    /// <summary>
    /// Gets the index of the current page.
    /// </summary>
    /// <value>
    /// The current page index. The index base (zero- or one-based) is defined by the implementation.
    /// </value>
    int PageIndex { get; }

    /// <summary>
    /// Gets the total number of records across all pages.
    /// </summary>
    /// <value>
    /// The total record count in the data set.
    /// </value>
    int TotalRecordCount { get; }

    /// <summary>
    /// Gets or sets additional route values to preserve when generating links,
    /// such as active filters or query parameters.
    /// </summary>
    /// <value>
    /// A <see cref="RouteValueDictionary"/> with extra route values to include, or <see langword="null"/>.
    /// </value>
    RouteValueDictionary? RouteValue { get; set; }

    /// <summary>
    /// Gets the current sort expression used to order the data.
    /// </summary>
    /// <value>
    /// The sort expression currently applied.
    /// </value>
    string SortExpression { get; }

    /// <summary>
    /// Gets or sets the maximum number of page numbers to display in a pager UI.
    /// </summary>
    /// <value>
    /// The number of page links to show around the current page.
    /// </value>
    int NumberOfPagesToShow { get; set; }

    /// <summary>
    /// Gets the first page index in the current pager window.
    /// </summary>
    /// <value>
    /// The starting page index for the visible page range.
    /// </value>
    int StartPageIndex { get; }

    /// <summary>
    /// Gets the last page index in the current pager window.
    /// </summary>
    /// <value>
    /// The ending page index for the visible page range.
    /// </value>
    int StopPageIndex { get; }
}
