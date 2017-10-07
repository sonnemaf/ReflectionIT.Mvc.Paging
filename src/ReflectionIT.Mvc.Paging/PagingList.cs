﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReflectionIT.Mvc.Paging
{
    public class PagingList
    {
        public static async Task<PagingList<T>> CreateAsync<T>(IOrderedQueryable<T> qry, int pageSize, int pageIndex)
            where T : class
        {
            try
            {
                var pageCount = (int)Math.Ceiling(await qry.CountAsync() / (double)pageSize);

                return new PagingList<T>(await qry.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                    pageSize, pageIndex, pageCount);
            }
            catch(InvalidOperationException) //If not async methods are not implemented
            {

                var pageCount = (int)Math.Ceiling(qry.Count() / (double)pageSize);

                return new PagingList<T>(qry.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                    pageSize, pageIndex, pageCount);
            }
        }

        public static async Task<PagingList<T>> CreateAsync<T>(IQueryable<T> qry, int pageSize, int pageIndex,
            string sortExpression, string defaultSortExpression) where T : class
        {
            try
            {
                var pageCount = (int)Math.Ceiling(await qry.CountAsync() / (double)pageSize);

                return new PagingList<T>(
                    await qry.OrderBy(sortExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                    pageSize, pageIndex, pageCount, sortExpression, defaultSortExpression);
            }
            catch(InvalidOperationException) //If not async methods are not implemented/is generic qry
            {
                var pageCount = (int)Math.Ceiling(qry.Count() / (double)pageSize);

                return new PagingList<T>(
                    qry.OrderBy(sortExpression).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                    pageSize, pageIndex, pageCount, sortExpression, defaultSortExpression);
            }
        }
    }
}