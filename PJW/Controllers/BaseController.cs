using Configuration;
using DbModel;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Microsoft.AspNetCore.Cors;
using Repository;
using ServicesModel;

namespace PJW.Web.Controllers
{
    /// <summary>
    /// 基礎控制器
    /// </summary>
    [EnableCors("Any")]//統一設定Cors策略
    public class BaseController : Microsoft.AspNetCore.Mvc.Controller
    {
        //private Token _token;
        /// <summary>
        /// 使用者登入態
        /// </summary>
        public Token UserToken {
            get {
                var token = CodingUtils.AesDecrypt(WebUtils.GetCookie(GlobalSetting.CurrentLoginUserGuid));
                var tokenObj = new Token(token);
                return tokenObj;
            } 
        }

        /// <summary>
        /// 目前選單Tid
        /// </summary>
        public long CurrentMenuTid { get; set; }

    }

}
