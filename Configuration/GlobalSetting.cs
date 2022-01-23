using System;
using System.Collections.Generic;
using System.Linq;

namespace Configuration
{
    public class GlobalSetting
    {

        /// <summary>
        /// 目前登入使用者的Eid
        /// </summary>
        public static string CurrentLoginUserGuid = "PJW_LoginID";
        /// <summary>
        /// 帶有許可權的MenuTid
        /// </summary>
        public static string CurrentMenu = "PJW_Menu";


        #region 超級管理模式

        public static List<string> SuperList
        {
            get
            {
                var result = new List<string>();
                var list = ConfigHelper.GetConfig("SuperList", string.Empty)
                    .Split(new string[] { "],[" }, StringSplitOptions.None).ToList();
                foreach (var li in list)
                {
                    result.Add(li.Replace("]", "").Replace("[", "").ToLower());
                }
                return result;
            }
        }
        #endregion
    }
}
