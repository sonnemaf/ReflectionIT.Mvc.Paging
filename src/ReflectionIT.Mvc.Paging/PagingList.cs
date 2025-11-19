using Microsoft.EntityFrameworkCore;

namespace ReflectionIT.Mvc.Paging;

/// <summary>
/// Provides static factory methods for creating paged lists from queries or collections, supporting both synchronous
/// and asynchronous operations with optional sorting and paging parameters.
/// </summary>
/// <remarks>The PagingList class enables efficient pagination of large data sets by retrieving only the items for
/// a specific page, along with metadata such as the current page index, total page count, and total record count.
/// Methods automatically adjust the requested page index to the nearest valid page if it is out of range. Sorting can
/// be applied using a sort expression where supported. All methods require the source query or collection to be
/// non-null and the page size to be greater than zero.</remarks>
public static class PagingList {

    /// <summary>
    /// Asynchronously creates a new paging list from the specified ordered query, using the given page size and page index.
    /// </summary>
    /// <remarks>The method executes the query asynchronously to retrieve the total item count and the items for the
    /// requested page. The query must be ordered to ensure consistent paging results.</remarks>
    /// <typeparam name="T">The type of the elements in the query and paging list. Must be a reference type.</typeparam>
    /// <param name="qry">The ordered queryable source from which to retrieve items for the paging list. Must not be null.</param>
    /// <param name="pageSize">The number of items to include on each page. Must be greater than zero.</param>
    /// <param name="pageIndex">The one-based index of the page to retrieve. If the value is less than 1 or greater than the total number of pages,
    /// it will be adjusted to the nearest valid page.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paging list for the specified page,
    /// including the items for that page and paging metadata.</returns>
    public static async Task<PagingList<T>> CreateAsync<T>(IOrderedQueryable<T> qry, int pageSize, int pageIndex) where T : class {
        var totalRecordCount = await qry.CountAsync().ConfigureAwait(false);
        var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

        pageIndex = Math.Max(1, Math.Min(pageIndex, pageCount));

        return new PagingList<T>(await qry.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false),
            pageIndex, pageCount, totalRecordCount);
    }

    /// <summary>
    /// Asynchronously creates a paged list from the specified query, applying sorting and pagination parameters.
    /// </summary>
    /// <remarks>The method calculates the total number of records and pages based on the provided query and
    /// page size. If the requested page index is out of range, it is automatically adjusted to the nearest valid page.
    /// Sorting is applied using the specified or default sort expression.</remarks>
    /// <typeparam name="T">The type of the elements in the source query.</typeparam>
    /// <param name="qry">The source query from which to retrieve items. Must not be null.</param>
    /// <param name="pageSize">The number of items to include on each page. Must be greater than zero.</param>
    /// <param name="pageIndex">The one-based index of the page to retrieve. Values less than 1 are treated as 1; values greater than the total
    /// number of pages are set to the last page.</param>
    /// <param name="sortExpression">The sort expression to apply to the query. If null or empty, the default sort expression is used.</param>
    /// <param name="defaultSortExpression">The default sort expression to use if <paramref name="sortExpression"/> is null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="PagingList{T}"/> with
    /// the items for the specified page, sorted as requested. If the query contains no items, the returned list will be
    /// empty.</returns>
    public static async Task<PagingList<T>> CreateAsync<T>(IQueryable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) where T : class {
        var totalRecordCount = await qry.CountAsync().ConfigureAwait(false);
        var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

        pageIndex = Math.Max(1, Math.Min(pageIndex, pageCount));

        return new PagingList<T>(await Extensions.OrderBy(qry, sortExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false),
            pageIndex, pageCount, sortExpression, defaultSortExpression, totalRecordCount);
    }

    /// <summary>
    /// Creates a new paged list containing the items for the specified page from the provided source sequence.
    /// </summary>
    /// <remarks>If the requested page index is outside the valid range, it is automatically adjusted to the
    /// nearest valid page. The method enumerates the entire source sequence to determine the total record
    /// count.</remarks>
    /// <typeparam name="T">The type of elements in the source sequence.</typeparam>
    /// <param name="qry">The source sequence to paginate. Cannot be null.</param>
    /// <param name="pageSize">The number of items to include in each page. Must be greater than zero.</param>
    /// <param name="pageIndex">The one-based index of the page to retrieve. Values less than 1 are treated as 1; values greater than the total
    /// number of pages are treated as the last page.</param>
    /// <returns>A <see cref="PagingList{T}"/> containing the items for the specified page, along with paging metadata such as the current page
    /// index, total page count, and total record count.</returns>
    public static PagingList<T> Create<T>(IEnumerable<T> qry, int pageSize, int pageIndex) where T : class {
        var totalRecordCount = qry.Count();
        var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

        pageIndex = Math.Max(1, Math.Min(pageIndex, pageCount));

        return new PagingList<T>(qry.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), pageIndex, pageCount, totalRecordCount);
    }

    /// <summary>
    /// Creates a new paging list from the specified query, applying sorting and returning the items for the requested
    /// page.
    /// </summary>
    /// <remarks>If the requested page index is outside the valid range, it is automatically adjusted to the
    /// nearest valid page. The method applies the specified sort expression before paging the results. The source
    /// collection must support sorting by the given expression.</remarks>
    /// <typeparam name="T">The type of elements in the source collection.</typeparam>
    /// <param name="qry">The source collection to paginate. Must not be null.</param>
    /// <param name="pageSize">The number of items to include on each page. Must be greater than zero.</param>
    /// <param name="pageIndex">The one-based index of the page to retrieve. Values less than 1 are treated as 1; values greater than the total
    /// number of pages are set to the last page.</param>
    /// <param name="sortExpression">The sort expression to apply to the collection. Must be a valid property name or expression supported by the
    /// underlying query provider.</param>
    /// <param name="defaultSortExpression">The default sort expression to use if the provided sort expression is not valid or not specified.</param>
    /// <returns>A <see cref="PagingList{T}"/> containing the items for the specified page, along with paging and sorting metadata.</returns>
    public static PagingList<T> Create<T>(IEnumerable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) where T : class {
        var totalRecordCount = qry.Count();
        var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

        pageIndex = Math.Max(1, Math.Min(pageIndex, pageCount));

        return new PagingList<T>(qry.OrderBy(sortExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
            pageIndex, pageCount, sortExpression, defaultSortExpression, totalRecordCount);
    }

    /// <summary>
    /// Creates a new <see cref="PagingList{T}"/> instance representing a single page of data from an ordered query, using the
    /// specified paging and sorting parameters.
    /// </summary>
    /// <remarks>If the specified pageIndex is outside the valid range, it is automatically adjusted to the
    /// nearest valid page. The method does not perform sorting; the input sequence must already be ordered as
    /// required.</remarks>
    /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
    /// <param name="orderedQuery">The ordered sequence of items to be paged. The sequence should already be sorted according to the desired sort
    /// expression.</param>
    /// <param name="pageSize">The number of items to include on each page. Must be greater than zero.</param>
    /// <param name="pageIndex">The one-based index of the page to retrieve. Values less than 1 are treated as 1; values greater than the total
    /// number of pages are treated as the last page.</param>
    /// <param name="totalRecordCount">The total number of records available in the full result set, before paging is applied. Must be zero or greater.</param>
    /// <param name="sortExpression">The sort expression used to order the items. This should match the ordering applied to the source query.</param>
    /// <param name="defaultSortExpression">The default sort expression to use if no sort expression is specified.</param>
    /// <returns>A <see cref="PagingList{T}"/> containing the items for the specified page, along with paging and sorting metadata.</returns>
    public static PagingList<T> Create<T>(IEnumerable<T> orderedQuery, int pageSize, int pageIndex, int totalRecordCount, string sortExpression, string defaultSortExpression) where T : class {
        var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

        pageIndex = Math.Max(1, Math.Min(pageIndex, pageCount));

        return new PagingList<T>(orderedQuery.ToList(),
            pageIndex, pageCount, sortExpression, defaultSortExpression, totalRecordCount);
    }

}