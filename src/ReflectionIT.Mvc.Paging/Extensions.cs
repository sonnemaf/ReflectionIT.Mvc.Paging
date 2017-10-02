using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;

namespace ReflectionIT.Mvc.Paging {

    public static class Extensions {

        public static string DisplayNameFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression) where TModel : class {
            return html.DisplayNameForInnerType<TModel, TValue>(expression);
        }

        [Obsolete("remove the pagingList parameter, it is not used any more")]
        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, IPagingList pagingList) where TModel : class {
            return SortableHeaderFor(html, expression, ExpressionHelper.GetExpressionText(expression), pagingList);
        }

        [Obsolete("remove the pagingList parameter, it is not used any more")]
        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn, IPagingList pagingList) where TModel : class {
            return SortableHeaderFor(html, expression, sortColumn);
        }

        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression, string sortColumn) where TModel : class {
            var bldr = new HtmlContentBuilder();
            bldr.AppendHtml(html.ActionLink(html.DisplayNameForInnerType(expression), html.ViewData.Model.Action, html.ViewData.Model.GetRouteValueForSort(sortColumn)));
            IPagingList pagingList = html.ViewData.Model;
            if (pagingList.SortExpression == sortColumn) {
                bldr.AppendHtml(PagingOptions.Current.HtmlIndicatorDown);
            } else {
                if (pagingList.SortExpression == "-" + sortColumn) {
                    bldr.AppendHtml(PagingOptions.Current.HtmlIndicatorUp);
                }
            }
            return bldr;
        }

        public static IHtmlContent SortableHeaderFor<TModel, TValue>(this IHtmlHelper<PagingList<TModel>> html, Expression<Func<TModel, TValue>> expression) where TModel : class {
            return SortableHeaderFor(html, expression, ExpressionHelper.GetExpressionText(expression));
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string sortExpression) where T : class {
            int index = 0;
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

        private static LambdaExpression GenerateSelector<TEntity>(String propertyName, out Type resultType) where TEntity : class {
            // Create a parameter to pass into the Lambda expression (Entity => Entity.OrderByField).
            var parameter = Expression.Parameter(typeof(TEntity), "Entity");
            //  create the selector part, but support child properties
            PropertyInfo property;
            Expression propertyAccess;
            if (propertyName.Contains('.')) {
                // support to be sorted on child fields.
                String[] childProperties = propertyName.Split('.');
                property = typeof(TEntity).GetProperty(childProperties[0]);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
                for (int i = 1; i < childProperties.Length; i++) {
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

        private static MethodCallExpression GenerateMethodCall<TEntity>(IQueryable<TEntity> source, string methodName, String fieldName) where TEntity : class {
            Type type = typeof(TEntity);
            Type selectorResultType;
            LambdaExpression selector = GenerateSelector<TEntity>(fieldName, out selectorResultType);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
                                            new Type[] { type, selectorResultType },
                                            source.Expression, Expression.Quote(selector));
            return resultExp;
        }

        public static void AddPaging(this IServiceCollection services) {
            //Get a reference to the assembly that contains the view components
            var assembly = typeof(ReflectionIT.Mvc.Paging.PagerViewComponent).GetTypeInfo().Assembly;

            //Create an EmbeddedFileProvider for that assembly
            var embeddedFileProvider = new EmbeddedFileProvider(
                assembly, "ReflectionIT.Mvc.Paging"
            );

            //Add the file provider to the Razor view engine
            services.Configure<RazorViewEngineOptions>(options => {
                options.FileProviders.Add(embeddedFileProvider);
            });
        }

        public static void AddPaging(this IServiceCollection services, Action<PagingOptions> configureOptions) {
            AddPaging(services);
            configureOptions(PagingOptions.Current);
        }

    }
}