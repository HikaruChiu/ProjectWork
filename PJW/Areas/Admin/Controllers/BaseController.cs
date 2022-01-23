using Microsoft.AspNetCore.Cors;
using ServicesModel;

namespace PJW.Web.Admin.Controllers
{
    /// <summary>
    /// 基礎控制器
    /// </summary>
    [EnableCors("Any")]//統一設定Cors策略
    public class BaseController : Microsoft.AspNetCore.Mvc.Controller
    {
        /// <summary>
        /// 使用者登入態
        /// </summary>
        public Token UserToken { get; set; }

        /// <summary>
        /// 目前選單Tid
        /// </summary>
        public long CurrentMenuTid { get; set; }

    }

}