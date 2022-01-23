using Infrastructure.StaticExt;
using Microsoft.AspNetCore.Http;
using System;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.RegularExpressions;

namespace Infrastructure.Web
{
    public static class WebUtils
    {

        public static string AppBaseUrl => $"{HttpContext.Current.Request.Scheme}://{HttpContext.Current.Request.Host}{HttpContext.Current.Request.PathBase}";

        /// <summary>
        /// 對 URL 字串進行編碼
        /// </summary>
        /// <param name="str">字串</param>
        /// <returns>編碼結果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// 原本的GetDisplayUrl 在請求的時候Host沒有值回報錯
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string SafeGetDisplayUrl(this HttpRequest request)
        {
            try
            {
                return request.GetDisplayUrl();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
            {
                var isHxr = request.Headers["X-Requested-With"] == "XMLHttpRequest";
                if (isHxr)
                {
                    return true;
                }
            }

            if (!string.IsNullOrEmpty(request.ContentType))
            {
                return request.ContentType.ToLower().Equals("application/x-www-form-urlencoded") ||
                       request.ContentType.ToLower().Equals("application/json");
            }

            return false;
        }

        /// <summary>
        /// 讀cookie值
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName];
            }

            return string.Empty;
        }

        public static void CookieClear()
        {
            foreach (string cookie in HttpContext.Current.Request.Cookies.Keys)
            {
                HttpContext.Current.Response.Cookies.Delete(cookie);
            }
        }

        /// <summary>
        /// 寫cookie值（未設定過期時間，則寫的是瀏覽器程序Cookie，一旦瀏覽器（是瀏覽器，非標籤頁）關閉，則Cookie自動失效）
        /// </summary>
        /// <param name="strName">名稱</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            CookieOptions cookieOptions = new();
            cookieOptions.HttpOnly = true;//加上 HttpOnly=true 防止XSS攻擊竊取cookie
            //cookieOptions.MaxAge = TimeSpan.FromMinutes(20);
            //cookieOptions.Expires = new DateTimeOffset(DateTime.Now.AddMinutes(240)); //2021-09-22 Hikaru 先設定cookie 過期是一天
            
            HttpContext.Current.Response.Cookies.Append(strName, strValue, cookieOptions);
        }


        /// <summary>
        /// 獲取客戶端瀏覽器的原始使用者代理資訊
        /// </summary>
        /// <returns></returns>
        public static string GetUserAgent()
        {
            return HttpContext.Current.Request.Headers["User-Agent"].ToString();
        }

        /// <summary>
        /// 獲得目前頁面客戶端的IP
        /// </summary>
        /// <returns>目前頁面客戶端的IP</returns>
        private static string GetIP()
        {
            var result = HttpContext.Current.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Connection.RemoteIpAddress.ToString();
            }

            return result;
        }

        /// <summary>
        /// 對於使用集群負載均衡的時候獲取真實客戶的IP
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string str = string.Empty;
            try
            {
                str = HttpContext.Current.Request.Headers["X-Forwarded-For"];
                if (string.IsNullOrEmpty(str))
                {
                    return GetIP();
                }
            }
            catch
            {
                str = "";
            }

            return str;
        }

        /// <summary>
        /// 獲得目前絕對路徑
        /// </summary>
        /// <param name="strPath">指定的路徑</param>
        /// <returns>絕對路徑</returns>
        public static string GetMapPath(string strPath)
        {
            strPath = strPath.Replace("/", "\\");
            if (strPath.StartsWith("\\"))
            {
                strPath = strPath.Substring(strPath.IndexOf('\\', 1)).TrimStart('\\');
            }

            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, strPath);
        }

    }
}