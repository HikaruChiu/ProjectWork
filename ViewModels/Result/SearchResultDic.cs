//-----------------------------------------------------------------------
// <copyright file="SearchResultDic.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace ViewModels.Result
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 返回結果有額外的資訊
    /// </summary>
    public class SearchResultDic<T, TF> : Result.ResultJsonInfo<T>
    {
        public SearchResultDic()
        {
            this.PageCount = 0;
            this.RecordCount = 0;
            this.ResultDic = new Dictionary<TF, TF>();
        }


        /// <summary>
        /// 總頁數
        /// </summary>
        public long PageCount { get; set; }

        /// <summary>
        /// 總記錄數
        /// </summary>
        public long RecordCount { get; set; }

        public Dictionary<TF, TF> ResultDic;

    }
}