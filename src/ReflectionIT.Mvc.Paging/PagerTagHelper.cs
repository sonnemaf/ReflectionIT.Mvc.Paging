using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReflectionIT.Mvc.Paging {

    //// You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    //[HtmlTargetElement("pager")]
    //public class PagerTagHelper : TagHelper {

    //    public IPagingList Model { get; set; }
    //    public IRazorViewEngine RazorViewEngine { get; }
    //    public ITempDataProvider TempdataProvider { get; }
    //    public IServiceProvider ServiceProvider { get; }

    //    public PagerTagHelper(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider,
    //        IServiceProvider serviceProvider) {
    //        this.RazorViewEngine = razorViewEngine;
    //        this.TempdataProvider = tempDataProvider;
    //        this.ServiceProvider = serviceProvider;
    //    }


    //    public override async void Process(TagHelperContext context, TagHelperOutput output) {

    //        var httpContext = new DefaultHttpContext { RequestServices = ServiceProvider };
    //        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

    //        using (var sw = new StringWriter()) {
    //            var viewResult = this.RazorViewEngine.FindView(actionContext, "PagerB3", false);

    //            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) {
    //                Model = this.Model
    //            };

    //            var viewContext = new ViewContext(
    //                actionContext,
    //                viewResult.View,
    //                viewDictionary,
    //                new TempDataDictionary(actionContext.HttpContext, this.TempdataProvider),
    //                sw,
    //                new HtmlHelperOptions()
    //            );

    //            await viewResult.View.RenderAsync(viewContext);

    //            output.Content.AppendHtml(sw.ToString());
    //        }

    //    }

}

