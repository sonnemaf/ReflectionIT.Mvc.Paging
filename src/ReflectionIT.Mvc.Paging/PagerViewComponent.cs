using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ReflectionIT.Mvc.Paging
{
    [ViewComponent(Name = "Pager")]
    public class PagerViewComponent : ViewComponent {

        public static string ViewName { get; set; } = "Bootstrap3";

        public static int DefaultNumberOfPagesToShow { get; set; } = 5;

        public IViewComponentResult Invoke(IPagingList pagingList) {
            return View(ViewName, pagingList);
        }
    }
}
