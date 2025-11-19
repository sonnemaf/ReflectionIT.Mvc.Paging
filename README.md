# ReflectionIT.Mvc.Paging
ASP.NET Core 8.0, 9.0 Paging (including filtering and sorting) solution using Entity Framework Core and IEnumerable<T>

More info: https://reflectionit.nl/blog/2017/paging-in-asp-net-core-mvc-and-entityframework-core

# NuGet packages

| Package | Version |
| ------ | ------ |
| ReflectionIT.Mvc.Paging | [![NuGet](https://img.shields.io/nuget/v/ReflectionIT.Mvc.Paging)](https://www.nuget.org/packages/ReflectionIT.Mvc.Paging/) |         

Use https://www.nuget.org/packages/ReflectionIT.Mvc.Paging/3.5.0 if you are still using ASP.NET Core 2.2

# Setup 
Add the following code to the ConfigureServices() method of the Startup class. You can/should set the PageParameterName to 'pageindex' to solve the Page "Area" problem in ASP.NET Core 2.2 and higher. See https://github.com/sonnemaf/ReflectionIT.Mvc.Paging/issues/21

```
services.AddPaging(options => {
    options.ViewName = "Bootstrap5";
    options.PageParameterName = "pageindex";
});
```            

# Controller
This Index action in this DemoController creates a PagingList and passes it as a Model to the View.

```
using Microsoft.AspNetCore.Mvc;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.Controllers
{
    public class DemoController : Controller {

        private static readonly List<Models.DemoViewModel> _sampleData = GetSampleData();

        private static List<Models.DemoViewModel> GetSampleData() {
            return Enumerable.Range(1, 100).Select(n =>
                new Models.DemoViewModel() {
                    Name = "Item" + n,
                    Number = n / 5
                }).ToList();
        }

        public IActionResult Index(int pageindex = 1, string sort = "Name") {

            var qry = from sd in _sampleData
                      where sd.Number > -5
                      select sd;

            var model = PagingList.Create(qry, 10, pageindex, sort, "Name");

            return View(model);
        }
    }
}
```

# View
The view has the PagingList<T> as the Model. The **<vc:pager paging-list="@Model"></vc:pager>** renders the pagers using a ViewComponent. These are stored as Razor pager files inside the ReflectionIT.Mvc.Paging library. You can create your own Razor pager files in the Views\Shared\Components\Pager folder.

```xml
@using ReflectionIT.Mvc.Paging
@addTagHelper *, ReflectionIT.Mvc.Paging
@model PagingList<SampleApp.Models.DemoViewModel>

@{
    ViewData["Title"] = "Demo";
}

<h1>Demo</h1>
Total Record Count: @Model.TotalRecordCount
<nav aria-label="Products navigation example">
    <vc:pager paging-list="@Model"></vc:pager>
</nav>

<table class="table">
    <tr>
        <th>
            @Html.SortableHeaderFor(model => model.Name)
        </th>
        <th>
            @Html.SortableHeaderFor(model => model.Number, "Number,Name")
        </th>
    </tr>

    @foreach (var item in Model) {
        <tr>
            <td>
                @Html.DisplayFor(modelItem => item.Name)
            </td>
            <td>
                @Html.DisplayFor(modelItem => item.Number)
            </td>
        </tr>
    }
</table>

<nav aria-label="Products navigation example">
    <vc:pager paging-list="@Model" />
</nav>

```

# Model
```
public class DemoViewModel {

    public string Name { get; set; }
    public int Number { get; set; }

}
```
