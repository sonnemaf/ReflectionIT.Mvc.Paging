using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.Controllers
{
    public class DemoController : Controller {

        private static List<Models.DemoViewModel> _sampleData = GetSampleData();

        private static List<Models.DemoViewModel> GetSampleData() {
            return Enumerable.Range(1, 100).Select(n =>
                new Models.DemoViewModel() {
                    Name = "Item" + n,
                    Number = n / 5
                }).ToList();
        }

        public IActionResult Index(int pageNumber = 1, string sort = "Name") {

            var qry = from sd in _sampleData
                      where sd.Number > -5
                      select sd;

            var model = PagingList.Create(qry, 10, pageNumber, sort, "Name");
            model.PageParameterName = "pageNumber";
            model.SortExpressionParameterName = "sort";

            return View(model);
        }
    }
}
