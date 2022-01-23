//-----------------------------------------------------------------------
// <copyright file="IAccount.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using DbModel;
using ServicesModel;
using ViewModels.Reuqest;

namespace Repository.Interface
{
    using System.Collections.Generic;


    /// <summary>
    /// 公共處理
    /// </summary>
    public interface ICommonRespository : IRepository
    {

        /// <summary>
        /// 查詢sql
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable SelectSqlExcute(string db, string sql);

        /// <summary>
        /// 執行sql
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        Tuple<int, string> SQLExcute(string db, string sql);

        /// <summary>
        /// 獲取所有的Table和Columns
        /// </summary>
        /// <returns></returns>
        string GetDbTablesAndColumns(string dbName);

        /// <summary>
        /// 獲取所有的表
        /// </summary>
        /// <returns></returns>
        List<CodeGenTable> GetDbTables();

        /// <summary>
        /// 獲取表下面所有的欄位
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        List<CodeGenField> GetDbTablesColumns(string dbName, string tableName);

        /// <summary>
        /// 自動產生程式碼
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        byte[] CodeGen(CodeGenVm model);

        /// <summary>
        /// 獲取所有的db
        /// </summary>
        /// <returns></returns>
        List<string> GetDbs();
    }
}