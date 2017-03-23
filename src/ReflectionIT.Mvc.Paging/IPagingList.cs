using Microsoft.AspNetCore.Routing;

namespace ReflectionIT.Mvc.Paging {
    public interface IPagingList {
        string Action { get; set; }
        RouteValueDictionary GetRouteValueForPage(int pageIndex);
        int PageCount { get; }
        int PageIndex { get; }
        RouteValueDictionary RouteValue { get; set; }
        string SortExpression { get; }

        int NumberOfPagesToShow { get; set; }
        int StartPageIndex { get; }
        int StopPageIndex { get; }


    }
}
