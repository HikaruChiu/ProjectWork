﻿using Infrastructure.Logging;
using Infrastructure.View;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PJW.Web
{
    public class ViewRenderService : IViewRenderService
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        public ViewRenderService(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderToStringAsync(string viewName, object model)
        {
            try
            {
                var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                using (var sw = new StringWriter())
                {
                    IView view = null;
                    var getViewResult = _razorViewEngine.GetView(executingFilePath: "~/", viewPath: "~/" + viewName + ".cshtml", isMainPage: true);
                    if (getViewResult.Success)
                    {
                        view = getViewResult.View;
                    }

                    if (view == null)
                    {
                        var viewResult = _razorViewEngine.FindView(actionContext, viewName, true);
                        view = viewResult?.View;
                    }

                    if (view == null)
                    {
                        var arr = viewName.Split('/');
                        if (arr.Length == 3)
                        {
                            var routeData = new RouteData();
                            routeData.Values["area"] = arr[0];
                            routeData.Values["controller"] = arr[1];
                            var routeDesc = new ActionDescriptor();
                            routeDesc.RouteValues["area"] = arr[0];
                            routeDesc.RouteValues["controller"] = arr[1];
                            actionContext = new ActionContext(httpContext, routeData, routeDesc);
                            var viewResult = _razorViewEngine.FindView(actionContext, arr[2], true);
                            view = viewResult?.View;
                        }

                    }

                    if (view == null)
                    {
                        LogHelper.Warn("ViewRenderService", "沒有找到對應的view：" + viewName);
                        return string.Empty;
                    }

                    var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };

                    var viewContext = new ViewContext(
                        actionContext,
                        view,
                        viewDictionary,
                        new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                        sw,
                        new HtmlHelperOptions()
                    );

                    await view.RenderAsync(viewContext);
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Warn("ViewRenderService", "找view出錯了：" + viewName, ex);
                return string.Empty;
            }
        }
    }
}