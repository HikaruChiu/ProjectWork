//-----------------------------------------------------------------------
// <copyright file="SearchConditionInfo.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace ViewModels.Condition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 查詢 適用於分頁查詢 直接包裝成對像ConditionModel
    /// </summary>
    public class SearchConditionInfo<T> : ConditionBase
    {
        private T conditionModel;

        public T ConditionModel
        {
            get { return conditionModel; }
            set { conditionModel = value; }
        }
    }
}