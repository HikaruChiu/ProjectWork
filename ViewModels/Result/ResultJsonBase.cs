//-----------------------------------------------------------------------
// <copyright file="ResultJsonBase.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace ViewModels.Result
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// ResultJsonBase
    /// </summary>
    public class ResultJsonBase
    {

        public ResultJsonBase()
        {

        }

        #region field

        private int status;
        private string info;

        #endregion

        #region property

        /// <summary>
        /// 狀態
        /// </summary>
        [Description("狀態(0:失敗 1:成功)")]
        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// 提示資訊
        /// </summary>
        [Description("提示資訊")]
        public string Info
        {
            get { return info; }
            set { info = value; }
        }

        #endregion
    }
}