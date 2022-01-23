using System.Collections.Generic;
using System.Threading.Tasks;
using PJW.Web.Admin.Controllers;
using PJW.Web.Filter;
using Configuration;
using DbModel;
using Infrastructure.Web;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace PJW.Web.Admin.Controllers
{
    /// <summary>
    /// 系統使用者
    /// </summary>
    [API("使用者")]
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class AccountController : BaseController
    {
        private readonly IAccountRespository AccountRespository;
        /// <summary>
        /// 構造方法
        /// </summary>
        /// <param name="_accountRespository"></param>
        public AccountController(IAccountRespository _accountRespository)
        {
            AccountRespository = _accountRespository;
        }

        /// <summary>
        /// 登入頁面
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 個人資訊頁面
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("使用者資訊頁面訪問")]
        public async Task<ActionResult> UserDetail()
        {
            var currentUser = await AccountRespository.GetUserInfo(UserToken);
            ViewBag.UserName = currentUser?.UserName;
            ViewBag.Phone = currentUser?.Phone;
            return View();
        }

        /// <summary>
        /// 更新個人資訊
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [API("更新個人資訊")]
        [AuthorizeFilter]
        public async Task<ActionResult> UpdateUserInfo([FromForm] SystemUsers user)
        {
            var result = new ResultJsonNoDataInfo();
            user.Eid = UserToken.Eid;
            var respositoryResult = await AccountRespository.UpdateUserInfo(user);
            if (string.IsNullOrEmpty(respositoryResult))
            {

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 更新登入密碼
        /// </summary>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [API("更新登入密碼")]
        [AuthorizeFilter]
        public async Task<ActionResult> UpdatePwd([FromForm] UpdatePwdVm user)
        {
            var result = new ResultJsonNoDataInfo();
            user.Eid = UserToken.Eid;
            var respositoryResult = await AccountRespository.UpdatePwd(user);
            if (string.IsNullOrEmpty(respositoryResult))
            {

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 退出系統
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            SessionClear();
            if (WebUtils.GetCookie("LoginFrom") == "intranet")
            {
                string intraUrl = ConfigHelper.GetConfig<string>("Intranet:LogoutUrl");
                return Redirect(intraUrl);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LogOn([FromForm] LogOnVM info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.LogOn(info);
            if (respositoryResult.Item1)
            {

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult.Item2 ?? ResultConfig.FailMessage;
            }
            return Json(result);
        }

        /// <summary>
        /// 使用者列表頁面
        /// </summary>
        /// <returns></returns>
        [AuthorizeFilter]
        [API("使用者列表頁面訪問")]
        public ActionResult UserList()
        {
            return View();
        }

        /// <summary>
        /// 獲取所有的使用者
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthorizeFilter]
        [HttpPost]
        [API("獲取所有的使用者")]
        public async Task<JsonResult> GetUserList([FromForm] AccountVm model)
        {
            var result = new SearchResult<List<UserSM>>();
            var respositoryResult = await AccountRespository.GetUserList(model, UserToken);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Rows = respositoryResult.Item2;
            result.Total = respositoryResult.Item1;
            return Json(result);
        }

        /// <summary>
        /// 給使用者賦予角色
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [ValidateAntiForgeryToken]
        [API("給使用者賦予角色")]
        public async Task<JsonResult> UserAddRole([FromForm] UserAddRoleVm info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.UserAddRole(info);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        /// <summary>
        /// 手動新增使用者
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [API("手動新增使用者")]
        public async Task<JsonResult> UserAdd([FromForm] SystemUsers info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.UserAdd(info,UserToken);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        /// <summary>
        /// 禁用使用者
        /// </summary>
        /// <param name="UserTid"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [ValidateAntiForgeryToken]
        [API("禁用使用者")]
        public async Task<JsonResult> UserDelete(long UserTid)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.UserDelete(UserTid);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        /// <summary>
        /// 改變某個屬性值
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeFilter]
        [ValidateAntiForgeryToken]
        [API("改變某個屬性值")]
        public async Task<JsonResult> ChangeField([FromForm] ChangeFieldVm info)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.ChangeField(info);
            if (respositoryResult.Item1)
            {
                result.Status = ResultConfig.Ok;
                result.Info = string.Empty;
            }
            else
            {
                result.Status = ResultConfig.Ok;
                result.Info = string.IsNullOrEmpty(respositoryResult.Item2) ? ResultConfig.FailMessage : respositoryResult.Item2;
            }
            return Json(result);
        }

        #region Private
        private void SessionClear()
        {
            WebUtils.CookieClear();
        }

        #endregion

    }

}