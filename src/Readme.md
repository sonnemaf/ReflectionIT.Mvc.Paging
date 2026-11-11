# ReflectionIT.Mvc.Paging

ASP.NET Core library for **paging and sorting** (and filtering) of Entity Framework Core and `IEnumerable<T>` data sources, with a ready-to-use pager view component.

- Targets **.NET 10.0** and **.NET 11.0**
- Async paging for `IQueryable<T>` (EF Core) and sync paging for `IEnumerable<T>`
- Multi-column and nested-property sorting, e.g. `"Name,-Age"` or `"Supplier.CompanyName"`
- Pager view component with Bootstrap 3, 4 and 5 views
- Sortable column headers that show a sort-direction indicator
- Page index is clamped automatically when it is out of range

## Installation

```
dotnet add package ReflectionIT.Mvc.Paging
```

## Quick start

### 1. Register (optional)

```csharp
builder.Services.AddPaging(options => {
    options.ViewName = "Bootstrap5";           // Bootstrap3 | Bootstrap4 | Bootstrap5
    options.PageParameterName = "pageindex";   // default: "pageIndex"
    options.SortExpressionParameterName = "sort"; // default: "sortExpression"
    options.DefaultNumberOfPagesToShow = 7;    // default: 5
});
```

### 2. `_ViewImports.cshtml`

```cshtml
@using ReflectionIT.Mvc.Paging
@addTagHelper *, ReflectionIT.Mvc.Paging
```

### 3. Controller

```csharp
public async Task<IActionResult> Index(string filter, int pageindex = 1, string sort = "ProductName") {
    var qry = _context.Products.AsNoTracking().AsQueryable();

    if (!string.IsNullOrWhiteSpace(filter)) {
        qry = qry.Where(p => EF.Functions.Like(p.ProductName, "%" + filter + "%"));
    }

    // pageSize = 10, default sort = "ProductName"
    var model = await PagingList.CreateAsync(qry, 10, pageindex, sort, "ProductName");

    // route values that are preserved in the pager and sort links
    model.RouteValue = new RouteValueDictionary { { "filter", filter } };

    return View(model);
}
```

### 4. View

```cshtml
@model PagingList<Product>

<nav aria-label="Products navigation">
    <vc:pager paging-list="Model" />
</nav>

<table class="table">
    <tr>
        <th>@Html.SortableHeaderFor(m => m.ProductName)</th>
        <th>@Html.SortableHeaderFor(m => m.Supplier.CompanyName, "Supplier.CompanyName, ProductName")</th>
        <th>@Html.SortableHeaderFor(m => m.UnitPrice, htmlAttributes: new { @class = "text-end" })</th>
    </tr>
    @foreach (var item in Model) {
        <tr>
            <td>@item.ProductName</td>
            <td>@item.Supplier.CompanyName</td>
            <td>@item.UnitPrice</td>
        </tr>
    }
</table>
```

Clicking a sortable header sorts ascending. Clicking it again toggles to descending (a `-` prefix), and so on.

## Creating a `PagingList<T>`

All factory methods are on the static `PagingList` class and require `pageSize > 0`.

| Method | Use when |
| --- | --- |
| `CreateAsync(IOrderedQueryable<T>, pageSize, pageIndex)` | EF Core query that is already ordered |
| `CreateAsync(IQueryable<T>, pageSize, pageIndex, sortExpression, defaultSortExpression)` | EF Core query, sorted by a sort expression |
| `Create(IEnumerable<T>, pageSize, pageIndex)` | In-memory sequence, no sorting |
| `Create(IEnumerable<T>, pageSize, pageIndex, sortExpression, defaultSortExpression)` | In-memory sequence, sorted by a sort expression |
| `Create(IEnumerable<T>, pageSize, pageIndex, totalRecordCount, sortExpression, defaultSortExpression)` | You already paged and sorted the data and know the total count |

`T` must be a reference type. If `pageIndex` is below 1 it becomes 1. If it is above the page count it becomes the last page. An empty source returns page 1 with 0 pages.

### Sort expressions

- `"Name"`: ascending
- `"-Name"`: descending
- `"Name,-Age"`: multiple columns
- `"Supplier.CompanyName"`: nested property (IQueryable overload)

An unknown property throws an `ArgumentException`. When `SortExpression` equals `DefaultSortExpression`, the sort parameter is left out of the generated links.

## `PagingList<T>` members

`PagingList<T>` derives from `List<T>` and holds the items of the current page.

| Member | Description |
| --- | --- |
| `PageIndex`, `PageCount`, `TotalRecordCount` | Paging metadata (`PageIndex` is 1-based) |
| `SortExpression`, `DefaultSortExpression` | Current and default sort |
| `Action` | Action used for links (default `"Index"`) |
| `PageParameterName`, `SortExpressionParameterName` | Query parameter names |
| `RouteValue` | Extra route values included in every link (e.g. a filter) |
| `NumberOfPagesToShow`, `StartPageIndex`, `StopPageIndex` | Window of page links shown in the pager |
| `GetRouteValueForPage(int)` / `GetRouteDataForPage(int)` | Route values for a page link |
| `GetRouteValueForSort(string)` | Route values for a sort link (toggles direction) |

## Pager view components

```cshtml
<vc:pager paging-list="Model" />
<vc:pager-with-named-view paging-list="Model" view-name="Bootstrap4" />
```

`pager` uses `PagingOptions.Current.ViewName`. `pager-with-named-view` takes an optional view name and falls back to the same default. You can supply your own pager view by adding a view with that name under `Views/Shared/Components/Pager/`. The model is `IPagingList`.

## Options

`PagingOptions.Current` holds the global defaults:

| Option | Default |
| --- | --- |
| `ViewName` | `"Bootstrap5"` |
| `PageParameterName` | `"pageIndex"` |
| `SortExpressionParameterName` | `"sortExpression"` |
| `DefaultNumberOfPagesToShow` | `5` |
| `HtmlIndicatorUp` / `HtmlIndicatorDown` | Bootstrap 3 glyphicon chevrons |

The sort indicators default to Bootstrap 3 glyphicons. Override them if you use another icon set:

```csharp
options.HtmlIndicatorUp   = " <i class=\"bi bi-chevron-up\"></i>";
options.HtmlIndicatorDown = " <i class=\"bi bi-chevron-down\"></i>";
```

## Sample

See the [SampleApp](./SampleApp) project, in particular `ProductsController` and `Views/Products/Index.cshtml`, for a complete example with filtering, sorting and paging.

## Links

- Source: https://github.com/sonnemaf/ReflectionIT.Mvc.Paging
- Author: Fons Sonnemans, Reflection IT