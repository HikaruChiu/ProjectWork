//-----------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Company">
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
    /// 搜索結果
    /// </summary>
    public class SearchResult<T> : Result.ResultJsonBase
    {



        #region property

        #region 分頁資訊


        /// <summary>
        /// 總記錄數
        /// </summary>
        [Description("總記錄數")]
        public long Total { get; set; }

        /// <summary>
        /// 總記錄數
        /// </summary>
        [Description("總記錄數")]
        public T Rows { get; set; }

        #endregion

        #endregion
    }
}