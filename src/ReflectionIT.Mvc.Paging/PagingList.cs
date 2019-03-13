using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace ReflectionIT.Mvc.Paging {

    public static class PagingList {

        /// <summary>
        /// Create a paging list based on the EntityFramework ordered query 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">Entity Framework ordered query</param>
        /// <param name="pageSize">The size of the Page</param>
        /// <param name="pageIndex">The index of the Page (1 based, not zero)</param>
        /// <returns>The PagingListOfT</returns>
        public static async Task<PagingList<T>> CreateAsync<T>(IOrderedQueryable<T> qry, int pageSize, int pageIndex) where T : class {
            var totalRecordCount = await qry.CountAsync().ConfigureAwait(false);
            var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

            return new PagingList<T>(await qry.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false),
                                        pageSize, pageIndex, pageCount, totalRecordCount);
        }

        /// <summary>
        /// Create a paging list based on the EntityFramework query with a given sort order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">Entity Framework query</param>
        /// <param name="pageSize">The size of the Page</param>
        /// <param name="pageIndex">The index of the Page (1 based, not zero)</param>
        /// <param name="sortExpression">Sort expression</param>
        /// <param name="defaultSortExpression">Default sort expression</param>
        /// <returns>The PagingListOfT</returns>
        public static async Task<PagingList<T>> CreateAsync<T>(IQueryable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) where T : class {
            var totalRecordCount = await qry.CountAsync().ConfigureAwait(false);
            var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

            return new PagingList<T>(await Extensions.OrderBy(qry, sortExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false),
                                     pageSize, pageIndex, pageCount, sortExpression, defaultSortExpression, totalRecordCount);
        }

        /// <summary>
        /// Create a paging list based in a LINQ to Object ordered query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">LINQ to Object ordered query</param>
        /// <param name="pageSize">The size of the Page</param>
        /// <param name="pageIndex">The index of the Page (1 based, not zero)</param>
        /// <returns>The PagingListOfT</returns>
        public static PagingList<T> Create<T>(IEnumerable<T> qry, int pageSize, int pageIndex) where T : class {
            var totalRecordCount = qry.Count();
            var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

            return new PagingList<T>(qry.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), pageSize, pageIndex, pageCount, totalRecordCount);
        }

        /// <summary>
        /// Create a paging list based in a LINQ to Object query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">LINQ to Object ordered query</param>
        /// <param name="pageSize">The size of the Page</param>
        /// <param name="pageIndex">The index of the Page (1 based, not zero)</param>
        /// <param name="sortExpression">Sort expression</param>
        /// <param name="defaultSortExpression">Default sort expression</param>
        /// <returns>The PagingListOfT</returns>
        public static PagingList<T> Create<T>(IEnumerable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) where T : class {
            var totalRecordCount = qry.Count();
            var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

            return new PagingList<T>(qry.OrderBy(sortExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                                     pageSize, pageIndex, pageCount, sortExpression, defaultSortExpression, totalRecordCount);
        }

        /// <summary>
        /// Create a paging list based in a LINQ to Object query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orderedQuery">LINQ to Object ordered query</param>
        /// <param name="pageSize">The size of the Page</param>
        /// <param name="pageIndex">The index of the Page (1 based, not zero)</param>
        /// <param name="totalRecordCount">Total Records</param>
        /// <param name="sortExpression">Sort expression</param>
        /// <param name="defaultSortExpression">Default sort expression</param>
        /// <returns>The PagingListOfT</returns>
        public static PagingList<T> Create<T>(IEnumerable<T> orderedQuery, int pageSize, int pageIndex, int totalRecordCount, string sortExpression, string defaultSortExpression) where T : class {
            var pageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);

            return new PagingList<T>(orderedQuery.ToList(),
                                     pageSize, pageIndex, pageCount, sortExpression, defaultSortExpression, totalRecordCount);
        }

    }
}