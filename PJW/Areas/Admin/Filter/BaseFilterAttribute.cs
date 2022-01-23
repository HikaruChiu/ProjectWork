using System;
using Configuration;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ViewModels.Result;

namespace PJW.Web.Filter
{
    ///　<summary>
    ///　許可權攔截
    ///　</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class BaseFilterAttribute : ActionFilterAttribute
    {


        #region 未授權和登入處理

        protected void Forbidden(ActionExecutingContext filterContext, string userInfo = null)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult(new ResultJsonNoDataInfo()
                {
                    Info = ResultConfig.FailMessageForNoPower,
                    Status = ResultConfig.Fail
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(userInfo))
                {
                    var http403 = new RouteValueDictionary(new
                    {
                        area="Admin",
                        action = "http403",
                        controller = "error",
                        userInfo = userInfo
                    });
                    filterContext.Result = new RedirectToRouteResult(http403);
                    return;
                }
                filterContext.Result = new RedirectResult("~/Admin/Error/http403");
            }
        }

        /// <summary>
        /// 未授權處理
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="currentContext"></param>
        protected void Unauthorized(ActionExecutingContext filterContext, filterContextInfo currentContext)
        {
            //session失效
            if (filterContext.HttpContext.Request.IsAjaxRequest()) //if Ajax request
            {
                filterContext.Result = new JsonResult(new ResultJsonNoDataInfo()
                {
                    Info = ResultConfig.FailMessageForNoPower,
                    Status = ResultConfig.NoPower
                });
            }
            else
            {
                var requestUrl = filterContext.HttpContext.Request.SafeGetDisplayUrl();
                if ((currentContext.controllerName.ToLower().Equals("home") &&
                     currentContext.actionName.ToLower().Equals("index")))
                {
                    filterContext.Result = new RedirectResult("~/Admin/Account/Login");
                    return;
                }
                else if (!string.IsNullOrEmpty(requestUrl) && !requestUrl.ToLower().Contains("admin/home/index"))
                {
                    filterContext.Result = new RedirectResult("~/Admin/Account/Login?returnUrl=" + WebUtils.UrlEncode(requestUrl));
                    return;
                }
                filterContext.Result = new RedirectResult("~/Admin/Account/Login");
            }
        }
        #endregion


        #region WriteMenuCookie

        protected void WriteMenuCookie(long menuTid)
        {
            string strName = GlobalSetting.CurrentMenu;
            string strValue = CodingUtils.AesEncrypt(menuTid.ToString());
            WebUtils.WriteCookie(strName, strValue);
        }



        #endregion
    }

    public class filterContextInfo
    {
        public filterContextInfo(ActionExecutingContext filterContext)
        {
            #region 獲取鏈接中的字元


            //獲取模組名稱
            //  module = filterContext.HttpContext.Request.Url.Segments[1].Replace('/', ' ').Trim();

            //獲取 controllerName 名稱
            controllerName = filterContext.RouteData.Values["controller"].ToString();

            //獲取ACTION 名稱
            actionName = filterContext.RouteData.Values["action"].ToString();


            area = filterContext.RouteData.Values["area"]?.ToString();

            #endregion 獲取鏈接中的字元
        }

   

        /// <summary>
        /// 獲取模組名稱
        /// </summary>
        public string module { get; set; }

        /// <summary>
        /// 獲取 controllerName 名稱
        /// </summary>
        public string controllerName { get; set; }

        /// <summary>
        /// 獲取ACTION 名稱
        /// </summary>
        public string actionName { get; set; }
        public string area { get; set; }
    }
}