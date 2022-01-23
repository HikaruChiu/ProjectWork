//-----------------------------------------------------------------------
// <copyright file="ReuqestBase.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace ViewModels.Reuqest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 適用於 WebAPI的請求包裝
    /// </summary>
    public class ReuqestBase
    {
        public ReuqestBase()
        {
            // this.Token = string.Empty;

        }
        // public string Token { get; set; }
    }


    public class RequestInfo<T> : ReuqestBase
    {
        public RequestInfo()
        {
            this.Data = default(T);
        }

        [Description("請求參數對像")]
        public T Data { get; set; }
    }
}