using Microsoft.AspNet.Routing;
using System;
namespace ReflectionIT.Mvc.Paging {
    public interface IPagingList {
        string Action { get; set; }
        RouteValueDictionary GetRouteValueForPage(int pageIndex);
        int PageCount { get; set; }
        int PageIndex { get; set; }
        RouteValueDictionary RouteValue { get; set; }
        string SortExpression { get; set; }

    }
}
