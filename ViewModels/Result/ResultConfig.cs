//-----------------------------------------------------------------------
// <copyright file="ResultConfig.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class ResultConfig
    {
        /// <summary>
        /// 執行成功返回值("1")
        /// </summary>
        public static int Ok = 1;
        /// <summary>
        /// 執行成功返回值提示("成功")
        /// </summary>
        public static string SuccessfulMessage = "操作成功！";


        /// <summary>
        /// 執行失敗返回值("0")
        /// </summary>
        public static int Fail = 0;
        /// <summary>
        /// 執行失敗返回值提示（失敗）
        /// </summary>
        public static string FailMessage = "操作失敗！";

        /// <summary>
        /// 
        /// </summary>
        public static int NoPower = 2;
        /// <summary>
        /// 
        /// </summary>
        public static string FailMessageForNoPower = "沒有許可權，操作失敗！";


        public static string FailMessageForSystem = "系統出錯，操作失敗！";

        public static string FailMessageForNotFound = "系統出錯,404-NotFound！";
    }
}