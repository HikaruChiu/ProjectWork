﻿//-----------------------------------------------------------------------
// <copyright file="ConditionBase.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------


using System.ComponentModel;

namespace ViewModels.Condition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 分頁查詢
    /// </summary>
    public class ConditionBase
    {
        private int pageSize;

        /// <summary>
        /// 條數,預設10
        /// </summary>
        [Description("條數,預設10")]
        public int PageSize
        {
            get { return pageSize <= 0 ? 10 : pageSize; }
            set { pageSize = value; }
        }

        private int pageIndex;

        /// <summary>
        /// 頁數,預設1
        /// </summary>
        [Description("頁數,預設1")]
        public int PageIndex
        {
            get { return pageIndex <= 0 ? 1 : pageIndex; }
            set { pageIndex = value; }
        }

        private string orderBy;

        /// <summary>
        /// 排序欄位
        /// </summary>
        [Description("排序欄位")]
        public string OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        private string orderSequence;

        /// <summary>
        /// asc | desc
        /// </summary>
        [Description("asc | desc")]
        public string OrderSequence
        {
            get { return string.IsNullOrEmpty(orderSequence) ? "asc" : orderSequence; }
            set { orderSequence = value; }
        }
    }
}