using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ReflectionIT.Mvc.Paging {

    public static class Extensions {

#pragma warning disable IDE0022 // Use expression body for methods

        public static string DisplayNameFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression) where TModel : class {
            return html.DisplayNameForInnerType<TModel, TValue>(expression);
        }

        [Obsolete("remove the pagingList parameter, it is not used any more")]
        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, IPagingList pagingList) where TModel : class {
            var member = (expression.Body as MemberExpression).Member;
            return SortableHeaderFor(html, expression, member.Name, pagingList);
        }

        [Obsolete("remove the pagingList parameter, it is not used any more")]
        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn, IPagingList pagingList) where TModel : class {
            return SortableHeaderFor(html, expression, sortColumn);
        }

        public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel,  PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression, string sortColumn, object htmlAttributes) where TModel : class {
            var pagingList = modelSelector(html.ViewData.Model);
            var bldr = new HtmlContentBuilder();

            bldr.AppendHtml(html.ActionLink(html.DisplayNameForInnerType(expression), pagingList.Action, pagingList.GetRouteValueForSort(sortColumn), htmlAttributes));

            if (pagingList.SortExpression == sortColumn || "-" + pagingList.SortExpression == sortColumn || pagingList.SortExpression == "-" + sortColumn) {
                bldr.AppendHtml(pagingList.SortExpression.StartsWith("-") ? PagingOptions.Current.HtmlIndicatorUp : PagingOptions.Current.HtmlIndicatorDown);
            }
            return bldr;
        }

        public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel, PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression, string sortColumn) where TModel : class {
            return SortableHeaderFor(html, modelSelector, expression, sortColumn, null);
        }

        public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel, PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression) where TModel : class {
            var member = (expression.Body as MemberExpression).Member;
            return SortableHeaderFor(html, modelSelector, expression, member.Name);
        }

        public static IHtmlContent SortableHeaderFor<TViewModel, TModel, TValue>(this IHtmlHelper<TViewModel> html, Func<TViewModel, PagingList<TModel>> modelSelector, Expression<Func<TModel, TValue>> expression, object htmlAttributes) where TModel : class {
            var member = (expression.Body as MemberExpression).Member;
            return SortableHeaderFor(html, modelSelector, expression, member.Name, htmlAttributes);
        }

        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn) where TModel : class {
            return SortableHeaderFor(html, expression, sortColumn, htmlAttributes: null);
        }

        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn, object htmlAttributes) where TModel : class {
            var bldr = new HtmlContentBuilder();
            bldr.AppendHtml(html.ActionLink(html.DisplayNameForInnerType(expression), html.ViewData.Model.Action, html.ViewData.Model.GetRouteValueForSort(sortColumn), htmlAttributes));
            IPagingList pagingList = html.ViewData.Model;

            if (pagingList.SortExpression == sortColumn || "-" + pagingList.SortExpression == sortColumn || pagingList.SortExpression == "-" + sortColumn) {
                bldr.AppendHtml(pagingList.SortExpression.StartsWith("-") ? PagingOptions.Current.HtmlIndicatorUp : PagingOptions.Current.HtmlIndicatorDown);
            }
            return bldr;
        }

        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression) where TModel : class {
            var member = (expression.Body as MemberExpression).Member;
            return SortableHeaderFor(html, expression, member.Name);
        }

        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes) where TModel : class {
            var member = (expression.Body as MemberExpression).Member;
            return SortableHeaderFor(html, expression, member.Name, htmlAttributes);
        }
#pragma warning restore IDE0022 // Use expression body for methods

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string sortExpression) where T : class {
            var index = 0;
            var a = sortExpression.Split(',');
            foreach (var item in a) {
                var m = index++ > 0 ? "ThenBy" : "OrderBy";
                if (item.StartsWith("-")) {
                    m += "Descending";
                    sortExpression = item.Substring(1);
                } else {
                    sortExpression = item;
                }

                var mc = GenerateMethodCall<T>(source, m, sortExpression.TrimStart());
                source = source.Provider.CreateQuery<T>(mc);
            }
            return source;
        }

        private static LambdaExpression GenerateSelector<TEntity>(string propertyName, out Type resultType) where TEntity : class {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.')) {
                // support to be sorted on child fields.
                var childProperties = propertyName.Split('.');
                property = typeof(TEntity).GetProperty(childProperties[0]);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (var i = 1; i < childProperties.Length; i++) {
                    property = property.PropertyType.GetProperty(childProperties[i]);
                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            } else {
                property = typeof(TEntity).GetProperty(propertyName);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }
            resultType = property.PropertyType;
            // Create the order by expression.
            return Expression.Lambda(propertyAccess, parameter);
        }

        private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, string fieldName) where TEntity : class {
            var type = typeof(TEntity);
            var selector = GenerateSelector<TEntity>(fieldName, out var selectorResultType);
            var resultExp = Expression.Call(typeof(Queryable), methodName,
                                            new Type[] { type, selectorResultType },
                                            source.Expression, Expression.Quote(selector));
            return resultExp;
        }

        [Obsolete("This call is not required any move since we moved to Razor Class Libraries")]
        public static void AddPaging(this IServiceCollection services) {
            ////Get a reference to the assembly that contains the view components
            //var assembly = typeof(ReflectionIT.Mvc.Paging.PagerViewComponent).GetTypeInfo().Assembly;

            ////Create an EmbeddedFileProvider for that assembly
            //var embeddedFileProvider = new EmbeddedFileProvider(
            //    assembly, "ReflectionIT.Mvc.Paging"
            //);

            ////Add the file provider to the Razor view engine
            //services.Configure<RazorViewEngineOptions>(options => {
            //    options.FileProviders.Add(embeddedFileProvider);
            //});
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static void AddPaging(this IServiceCollection services, Action<PagingOptions> configureOptions) {
#pragma warning restore IDE0060 // Remove unused parameter
                               //AddPaging(services);
            configureOptions(PagingOptions.Current);
        }

        public static Dictionary<string, string> GetRouteData(this IPagingList pl, int pageIndex, params string[] excludes) {
            var dict = pl.GetRouteValueForPage(pageIndex);
            return dict.Where(kvp => kvp.Value is object && !excludes.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
        }

    }
}