﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels.Reuqest
{
    public class LogOnVM
    {

        /// <summary>
        /// 登入名稱
        /// </summary>
        public string eid { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string pwd { get; set; }


    }

    public class UserAddRoleVm
    {
        /// <summary>
        /// 使用者Tid
        /// </summary>
        public long UserTid { get; set; }

        /// <summary>
        /// 角色Tid
        /// </summary>
        public long RoleTid { get; set; }

    }

    public class ChangeFieldVm
    {
        /// <summary>
        /// 主鍵
        /// </summary>
        public long Tid { get; set; }

        /// <summary>
        /// 欄位
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

    }
}
