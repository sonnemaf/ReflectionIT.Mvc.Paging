﻿using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReflectionIT.Mvc.Paging {

    public class PagingList<T> : List<T>, IPagingList<T> where T : class {

        public int PageIndex { get; }
        public int PageCount { get; }
        public int TotalRecordCount { get; }
        public string Action { get; set; }
        public string PageParameterName { get; set; }
        public string SortExpressionParameterName { get; set; }
        public string SortExpression { get; } = String.Empty;

        public RouteValueDictionary? RouteValue { get; set; }

        public string DefaultSortExpression { get; } = String.Empty;

        [Obsolete("Use PagingList.CreateAsync<T>() instead")]
        public static Task<PagingList<T>> CreateAsync(IOrderedQueryable<T> qry, int pageSize, int pageIndex) {
            return PagingList.CreateAsync(qry, pageSize, pageIndex);
        }

        [Obsolete("Use PagingList.CreateAsync<T>() instead")]
        public static Task<PagingList<T>> CreateAsync(IQueryable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) {
            return PagingList.CreateAsync(qry, pageSize, pageIndex, sortExpression, defaultSortExpression);
        }

        internal PagingList(List<T> list, int pageIndex, int pageCount, int totalRecordCount)
            : base(list) {
            this.TotalRecordCount = totalRecordCount;
            this.PageIndex = pageIndex;
            this.PageCount = pageCount;
            this.Action = "Index";
            this.PageParameterName = PagingOptions.Current.PageParameterName;
            this.SortExpressionParameterName = PagingOptions.Current.SortExpressionParameterName;
        }

        internal PagingList(List<T> list, int pageIndex, int pageCount, string sortExpression, string defaultSortExpression, int totalRecordCount)
            : this(list, pageIndex, pageCount, totalRecordCount) {

            this.SortExpression = sortExpression;
            this.DefaultSortExpression = defaultSortExpression;
        }


        public RouteValueDictionary GetRouteValueForPage(int pageIndex) {

            var dict = this.RouteValue == null ? new RouteValueDictionary() :
                                                 new RouteValueDictionary(this.RouteValue);

            dict[this.PageParameterName] = pageIndex;

            if (this.SortExpression != this.DefaultSortExpression) {
                dict[this.SortExpressionParameterName] = this.SortExpression;
            }

            return dict;
        }

        public Dictionary<string, string?> GetRouteDataForPage(int pageIndex) {
            var dict = this.RouteValue == null ? new RouteValueDictionary() :
                                                 new RouteValueDictionary(this.RouteValue);

            dict[this.PageParameterName] = pageIndex;

            if (this.SortExpression != this.DefaultSortExpression) {
                dict[this.SortExpressionParameterName] = this.SortExpression;
            }

            return dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
        }

        public RouteValueDictionary GetRouteValueForSort(string sortExpression) {

            var dict = this.RouteValue == null ? new RouteValueDictionary() :
                                                 new RouteValueDictionary(this.RouteValue);

            if (sortExpression == this.SortExpression) {
                sortExpression = this.SortExpression.StartsWith("-") ? sortExpression[1..] : "-" + sortExpression;
            }

            dict[this.SortExpressionParameterName] = sortExpression;

            return dict;
        }

        public int NumberOfPagesToShow { get; set; } = PagingOptions.Current.DefaultNumberOfPagesToShow;

        public int StartPageIndex {
            get {
                var half = (int)((this.NumberOfPagesToShow - 0.5) / 2);
                var start = Math.Max(1, this.PageIndex - half);
                if (start + this.NumberOfPagesToShow - 1 > this.PageCount) {
                    start = this.PageCount - this.NumberOfPagesToShow + 1;
                }
                return Math.Max(1, start);
            }
        }

        public int StopPageIndex => Math.Min(this.PageCount, this.StartPageIndex + this.NumberOfPagesToShow - 1);

    }
}