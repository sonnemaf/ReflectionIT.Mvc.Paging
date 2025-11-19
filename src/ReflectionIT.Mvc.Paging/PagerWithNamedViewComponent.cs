using Microsoft.AspNetCore.Mvc;

namespace ReflectionIT.Mvc.Paging; 

/// <summary>
/// View Component that renders the pager UI for an <see cref="IPagingList"/> using a specified view name.
/// </summary>
/// <remarks>
/// If no view name is provided when the component is invoked, the value of <see cref="PagingOptions.Current"/>.<see cref="PagingOptions.ViewName"/> is used.
/// The <see cref="IPagingList" /> is passed to the view as the model.
/// </remarks>
/// <example>
/// Usage in a Razor Page:
/// <code>
/// @await Component.InvokeAsync("PagerWithNamedView", new { pagingList = Model.Items, viewName = "_BootstrapPager" })
/// </code>
/// </example>
/// <seealso cref="IPagingList"/>
/// <seealso cref="PagingOptions"/>
[ViewComponent(Name = "PagerWithNamedView")]
public class PagerWithNamedViewComponent : ViewComponent {

    /// <summary>
    /// Renders the pager using the provided <see cref="IPagingList"/> and an optional view name.
    /// </summary>
    /// <param name="pagingList">The paging list model to render.</param>
    /// <param name="viewName">Optional view name to use for rendering. When <c>null</c>, the default from <see cref="PagingOptions.Current"/> is used.</param>
    /// <returns>An <see cref="IViewComponentResult"/> that renders the pager view.</returns>
    public IViewComponentResult Invoke(IPagingList pagingList, string? viewName = default) {
        return View(viewName ?? PagingOptions.Current.ViewName, pagingList);
    }

}
