﻿using ViewModels.Condition;

namespace ViewModels.Reuqest
{

    public class AccountVm : ConditionBase
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Eid
        /// </summary>
        public string Eid { get; set; }

        /// <summary>
        /// 角色TId
        /// </summary>
        public long RoleTid { get; set; }

    }



    public class UserVm : ConditionBase
    {
        /// <summary>
        /// 郵箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手機號
        /// </summary>
        public string Phone { get; set; }

    }


    public class UpdatePwdVm
    {
        public string Eid { get; set; }
        public string OldPwd { get; set; }
        public string Pwd { get; set; }
    }
}
