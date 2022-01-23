//-----------------------------------------------------------------------
// <copyright file="Token.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------


namespace ServicesModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;



    public class Token
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 登入Id
        /// </summary>
        public string Eid { get; set; }
        /// <summary>
        /// Intranet的員工主鍵
        /// </summary>
        public string Indx { get; set; }
        /// <summary>
        /// Intranet的員工信箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Intranet的員工部門ID
        /// </summary>
        public string DeptNo { get; set; }

        public string MenuRights { get; set; }

        public string ToJsonString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public long RoleTid { get; set; }

        public string RoleName { get; set; }
        public Token()
        {

        }
        public Token(string cookie)
        {
            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(cookie);
            if (token == null)
            {
                throw new Exception("token invaild");
            }
            this.Eid = token.Eid;
            this.Indx = token.Indx;
            this.Email = token.Email;
            this.MenuRights = token.MenuRights;
            this.Code = token.Code;
            this.RoleTid = token.RoleTid;
            this.RoleName = token.RoleName;
            this.DeptNo = token.DeptNo;
        }
    }
}