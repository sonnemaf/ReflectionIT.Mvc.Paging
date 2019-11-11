using Microsoft.AspNetCore.Mvc;

namespace ReflectionIT.Mvc.Paging {
    [ViewComponent(Name = "Pager")]
    public class PagerViewComponent : ViewComponent {

        public IViewComponentResult Invoke(IPagingList pagingList) {
            return View(PagingOptions.Current.ViewName, pagingList);
        }
    }
}
