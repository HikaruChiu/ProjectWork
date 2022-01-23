//-----------------------------------------------------------------------
// <copyright file="IAccount.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using DbModel;
using ServicesModel;
using ViewModels.Reuqest;

namespace Repository.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    /// <summary>
    /// 角色許可權管理
    /// </summary>
    public interface IRoleRespository : IRepository<SystemRole>
    {
        /// <summary>
        /// 獲取角色列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        Task<Tuple<long, List<SystemRole>>> GetList(RoleVm model, Token eid);
        /// <summary>
        /// 新增Role
        /// </summary>
        /// <param name="role"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<string> UseTransactionAddRole(AddRoleVm role, Token uid);
        /// <summary>
        /// 獲取所有的角色
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        Task<List<SystemRole>> GetAllRoleList(Token uid);
        /// <summary>
        /// 獲取目前頁面的action集合
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="RoleTid"></param>
        /// <param name="currentMenuTid"></param>
        /// <returns></returns>
        Task<SystemAction> GetSystemUserActions(string eid, long RoleTid, long currentMenuTid);
        /// <summary>
        /// 頁面與介面關聯
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> UseTransactionAddRoleActions(RoleAction model);
        /// <summary>
        /// 許可權與介面關聯配置 獲取某一個選單的某一個功能配置的介面列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<SystemPageAction>> GetRoleActions(RoleAction model);
        /// <summary>
        /// 刪除角色
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> DeleteRole(long tid, Token user);
    }
}