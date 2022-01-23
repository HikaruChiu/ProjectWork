//-----------------------------------------------------------------------
// <copyright file="ResultJsonNoDataInfo.cs" company="Company">
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
    ///API返回的Json模型（用於Insert、Update、Delete等不需要返回數據集的場景） 
    /// </summary>
    public class ResultJsonNoDataInfo : ResultJsonBase
    {
        public ResultJsonNoDataInfo()
        {

        }

        //#region field

        //private string guid;

        //#endregion

        //#region property

        ///// <summary>
        ///// 返回被操作記錄的Guid
        ///// </summary>
        //public string Guid
        //{
        //    get { return guid; }
        //    set { guid = value; }
        //}

        //#endregion
    }
}