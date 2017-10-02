using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace ReflectionIT.Mvc.Paging {

    public class PagingList {

        public static async Task<PagingList<T>> CreateAsync<T>(IOrderedQueryable<T> qry, int pageSize, int pageIndex) where T : class {
            var pageCount = (int)Math.Ceiling(await qry.CountAsync() / (double)pageSize);

            return new PagingList<T>(await qry.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                                        pageSize, pageIndex, pageCount);
        }

        public static async Task<PagingList<T>> CreateAsync<T>(IQueryable<T> qry, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) where T : class {
            var pageCount = (int)Math.Ceiling(await qry.CountAsync() / (double)pageSize);

            return new PagingList<T>(await qry.OrderBy(sortExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                                     pageSize, pageIndex, pageCount, sortExpression, defaultSortExpression);
        }

       

    }
}