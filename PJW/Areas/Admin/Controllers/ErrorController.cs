using Configuration;
using Infrastructure.Web;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using ViewModels.Result;

namespace PJW.Web.Admin.Controllers
{
    /// <summary>
    /// 錯誤
    /// </summary>
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class ErrorController : BaseController
    {

        public IHttpContextAccessor HttpContextAccessor { get; }

        public ErrorController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        [HttpGet("admin/error")]
        public IActionResult Index()
        {
            IExceptionHandlerPathFeature iExceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (iExceptionHandlerFeature != null)
            {
                string path = iExceptionHandlerFeature.Path;
                Exception exception = iExceptionHandlerFeature.Error;

                //Write code here to log the exception details
                return View("Error", iExceptionHandlerFeature);
            }
            return View();
        }

        [Route("admin/error/404")]
        public ActionResult Http404()
        {
            if (HttpContextAccessor.HttpContext.Request.IsAjaxRequest())
            {
                return Http405();
            }
            return View();
        }

        public ActionResult Http403(string userInfo)
        {
            ViewBag.userInfo = userInfo;
            return View();
        }


        /// <summary>
        /// 未登入
        /// </summary>
        public ActionResult NoLogin()
        {
            return RedirectToAction("Login", "Account",new {area="Admin"});
        }

        /// <summary>
        /// 沒有許可權
        /// </summary>
        /// <returns>ActionResult.</returns>
        [Route("admin/error/401")]
        public JsonResult Http401()
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForNoPower;
            return Json(result);
        }

        /// <summary>
        /// 系統錯誤
        /// </summary>
        /// <returns></returns>
        [Route("admin/error/500")]
        public JsonResult Http500(string exception =null)
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForSystem + (exception!=null?$"【Exception:{exception}】":"");
            return Json(result);
        }

        public ActionResult Http405()
        {
            var result = new ResultJsonNoDataInfo();
            result.Status = ResultConfig.Fail;
            result.Info = ResultConfig.FailMessageForNotFound;
            return Json(result);
        }
    }
}