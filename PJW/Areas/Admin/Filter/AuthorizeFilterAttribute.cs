using Autofac.Annotation;
using Configuration;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Repository.Interface;
using ServicesModel;
using System;
using System.Linq;
using System.Reflection;
using PJW.Web.Admin.Controllers;
using DbModel;

namespace PJW.Web.Filter
{
    [Component]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizeFilterAttribute : AuthorizeServiceFilterAttribute, IFilterFactory, IFilterMetadata
    {

        /// <inheritdoc />
        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));
            IFilterMetadata requiredService = serviceProvider.GetRequiredService(typeof(AuthorizeFilterAttribute)) as IFilterMetadata;
            if (requiredService != null)
                return requiredService;
            throw new InvalidOperationException();
        }

        public bool IsReusable { get; }
    }
    ///　<summary>
    ///　許可權攔截
    ///　</summary>
    public class AuthorizeServiceFilterAttribute : BaseFilterAttribute
    {
        [Autowired]
        public IAccountRespository AccountRespository { get; set; }

        [Autowired]
        public IMenuRespository MenuRespository { get; set; }



        public AuthorizeServiceFilterAttribute()
        {
            AllowAll = false;
        }
        /// <summary>
        /// 是否允許所有人檢視
        /// </summary>
        public bool AllowAll { get; set; }

        /// <summary>
        /// 在執行操作方法之前由 ASP.NET MVC 框架呼叫。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (AllowAll) { return; }
            var currentContext = new filterContextInfo(filterContext);
            SystemUsers systemUser;
            //檢查是否登入
            //從cookie 拿到token
            var token = CodingUtils.AesDecrypt(WebUtils.GetCookie(GlobalSetting.CurrentLoginUserGuid));
            if (string.IsNullOrEmpty(token))
            {

                Unauthorized(filterContext, currentContext);//跳轉登入
                return;
            }

            try
            {

                var tokenObj = new Token(token);
                systemUser = AccountRespository.Entity.FirstOrDefault(r => r.Eid.Equals(tokenObj.Eid));
                ((BaseController)filterContext.Controller).UserToken = tokenObj;
                var smTid = CodingUtils.AesDecrypt(WebUtils.GetCookie(GlobalSetting.CurrentMenu));
                if (!string.IsNullOrEmpty(smTid))
                {
                    ((BaseController)filterContext.Controller).CurrentMenuTid = long.Parse(smTid);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Warn("OnActionExecuting", ex);
                WebUtils.CookieClear();
                Unauthorized(filterContext, currentContext);//跳轉登入
                return;
            }

            //檢測使用者是否被禁用
            if (systemUser == null || !systemUser.IsActive)
            {
                WebUtils.CookieClear();
                Forbidden(filterContext);//跳轉登入
                return;
            }

          

            //檢查目前使用者是否有訪問當前menu的許可權
            var currentUrl = currentContext.controllerName + "/" + currentContext.actionName;

            if (!string.IsNullOrEmpty(currentContext.area))
            {
                currentUrl = currentContext.area + "/" + currentUrl;
            }

            //檢查是否是上帝模式//
            if (GlobalSetting.SuperList.Contains(systemUser.Eid))
            {
                return;
            }


            var menuTid = MenuRespository.HaveMenuPermission(currentUrl, systemUser.MenuRights);
            if (menuTid < 0)
            {
                Forbidden(filterContext, string.Concat("[", systemUser.Eid, "-", systemUser.UserName, "]"));
                return;
            }


            if (menuTid == 0)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var refer = filterContext.HttpContext.Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrEmpty(refer))
                    {
                        var s1 = refer.Split('?')[0].Split('/').ToList();
                        s1.Reverse();
                        if (s1.Count < 2) return;
                        var s2 = s1.Take(2).Reverse().ToList();
                        currentUrl = s2[0] + "/" + s2[1];
                        if (!string.IsNullOrEmpty(currentContext.area))
                        {
                            currentUrl = currentContext.area + "/" + currentUrl;
                        }
                        menuTid = MenuRespository.HaveMenuPermission(currentUrl, systemUser.MenuRights);
                    }
                }

            }

            if (menuTid == 0)
            {
                return;
            }

            //走到這裡 一定是menu配置過的
            WriteMenuCookie(menuTid);

            if (menuTid == 0) return;

            if (filterContext.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var apiAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttribute<APIAttribute>();
                if (apiAttribute != null)
                {
                    var controlFullName = controllerActionDescriptor.ControllerTypeInfo.Name;
                    var actionFullName = controllerActionDescriptor.MethodInfo.Name;
                    //檢查目前Menu 是否配置過了目前的Action
                    var isExist = MenuRespository.HaveActionPermission(menuTid, systemUser.RoleTid, controlFullName, actionFullName);
                    if (!isExist)
                    {
                        Forbidden(filterContext, string.Concat("[", systemUser.Eid, "-", systemUser.UserName, "]"));
                    }
                }
            }


        }

    }


}