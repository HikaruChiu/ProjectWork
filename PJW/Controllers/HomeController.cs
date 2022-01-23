using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc;
using Repository;
using Repository.Interface;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace PJW.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IAccountRespository AccountRespository;

        public HomeController(IAccountRespository _accountRespository)
        {
            AccountRespository = _accountRespository;
        }

        public async Task<IActionResult> Index()
        {
            //SSO登入用
            string Eid = HttpContext.Request.Query["T"]; //帳號(必須AES加密)
            string Pwd = HttpContext.Request.Query["E"]; //密碼(必須AES加密)
            string Any = HttpContext.Request.Query["A"]; //混淆用餐數
            //Eid = CodingUtils.AesEncrypt("hikaruchiu");
            //Pwd = CodingUtils.AesEncrypt("1234");
            //Any = "adwdddawdawd";
            if (!string.IsNullOrWhiteSpace(Eid) && !string.IsNullOrWhiteSpace(Pwd) && !string.IsNullOrWhiteSpace(Any))
            {
                //把帶過來的加密參數AES解密
                Eid = CodingUtils.AesDecrypt(Eid);
                Pwd = CodingUtils.AesDecrypt(Pwd);

                LogOnVM info = new();
                info.eid = Eid;
                info.pwd = Pwd;
                SSO sso = new(AccountRespository);
                ResultJsonNoDataInfo result = await sso.LogOnSSO(info);
                if (result.Status != ResultConfig.Ok)
                {
                    return Content("SSO 登入失敗");
                }

                WebUtils.WriteCookie("LoginFrom","intranet");

                //return RedirectToAction("SSO", "Home", info);

            }


            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public IActionResult SSO(LogOnVM info)
        {
            return View(info);
        }

        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>

        public ActionResult DashBord()
        {
            return Content($"專案工作任務管理系統");
            //return RedirectToAction("List1", "WorkLog");
        }
    }
}