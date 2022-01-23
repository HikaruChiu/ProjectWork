//-----------------------------------------------------------------------
// <copyright file="ResultJsonInfo.cs" company="Company">
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
    /// API返回的Json模型（用於Select等需要返回數據集的場景）
    /// </summary>
    public class ResultJsonInfo<T> : ResultJsonBase
    {
        public ResultJsonInfo()
        {

        }

        #region field

        private T data;

        #endregion

        #region property

        /// <summary>
        /// 返回數據
        /// </summary>
        [Description("返回數據")]
        public T Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion
    }
}