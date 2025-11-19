using Microsoft.AspNetCore.Mvc;

namespace ReflectionIT.Mvc.Paging;

/// <summary>
/// ASP.NET Core view component that renders pager navigation for an <see cref="IPagingList"/>.
/// </summary>
/// <remarks>
/// The component is registered under the name "Pager".
/// Invoke it from Razor as:
/// <code>
/// @await Component.InvokeAsync("Pager", new { pagingList = Model.Items })
/// </code>
/// The view that is rendered is resolved using <see cref="PagingOptions.Current.ViewName"/>.
/// </remarks>
[ViewComponent(Name = "Pager")]
public class PagerViewComponent : ViewComponent {

    /// <summary>
    /// Executes the Pager view component and returns the view configured in <see cref="PagingOptions.Current"/>.
    /// </summary>
    /// <param name="pagingList">The paging model containing the items and pagination metadata to render.</param>
    /// <returns>An <see cref="IViewComponentResult"/> that renders the pager UI.</returns>
    public IViewComponentResult Invoke(IPagingList pagingList) {
        return View(PagingOptions.Current.ViewName, pagingList);
    }
}
