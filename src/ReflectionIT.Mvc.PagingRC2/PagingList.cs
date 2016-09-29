using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Routing;

namespace ReflectionIT.Mvc.Paging {

    public class PagingList<T> : List<T>, IPagingList where T : class {

        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public string Action { get; set; }
        public string SortExpression { get; set; }

        public string DefaultSortExpression { get; set; }

        public PagingList(IOrderedQueryable<T> qry, int pageSize, int pageIndex)
            : base(qry.Skip((pageIndex - 1) * pageSize).Take(pageSize)) {
            this.PageIndex = pageIndex;
            this.PageCount = (int)Math.Ceiling(qry.Count() / (double)pageSize);
            this.Action = "Index";
        }

        public PagingList(IQueryable<T> qry, string sortExpression, string defaultSortExpression, int pageSize, int pageIndex)
            : this(qry.OrderBy(sortExpression) as IOrderedQueryable<T>, pageSize, pageIndex) {
            this.SortExpression = sortExpression;
            this.DefaultSortExpression = defaultSortExpression;
        }

        public RouteValueDictionary RouteValue { get; set; }

        public RouteValueDictionary GetRouteValueForPage(int pageIndex) {
            RouteValueDictionary dict =
                this.RouteValue == null ? new RouteValueDictionary() :
                new RouteValueDictionary(this.RouteValue);

            dict["page"] = pageIndex;

            if (this.SortExpression != this.DefaultSortExpression) {
                dict["sortExpression"] = this.SortExpression;
            }

            return dict;
        }

        public RouteValueDictionary GetRouteValueForSort(string sortExpression) {
            RouteValueDictionary dict =
                this.RouteValue == null ? new RouteValueDictionary() :
                new RouteValueDictionary(this.RouteValue);

            if (sortExpression == this.SortExpression) {
                sortExpression = "-" + sortExpression;
            }

            dict["sortExpression"] = sortExpression;

            return dict;
        }

    }
}