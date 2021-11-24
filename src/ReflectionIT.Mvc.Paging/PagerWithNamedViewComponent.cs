using Microsoft.AspNetCore.Mvc;

namespace ReflectionIT.Mvc.Paging {

    [ViewComponent(Name = "PagerWithNamedView")]
    public class PagerWithNamedViewComponent : ViewComponent {

        public IViewComponentResult Invoke(IPagingList pagingList, string? viewName = default) {
            return View(viewName ?? PagingOptions.Current.ViewName, pagingList);
        }

    }
}
