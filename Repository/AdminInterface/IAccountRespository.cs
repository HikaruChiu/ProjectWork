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
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 系統使用者
    /// </summary>
    public interface IAccountRespository : IRepository<SystemUsers>
    {
        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> LogOn(LogOnVM info);
        /// <summary>
        /// 獲取使用者列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Tuple<long, List<UserSM>>> GetUserList(AccountVm model, Token user);
        /// <summary>
        /// 給使用者賦予角色
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> UserAddRole(UserAddRoleVm info);
        /// <summary>
        /// 禁用使用者
        /// </summary>
        /// <param name="userTid"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> UserDelete(long userTid);
        /// <summary>
        /// 更改使用者的屬性
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> ChangeField(ChangeFieldVm info);
        /// <summary>
        /// 手動新增使用者
        /// </summary>
        /// <param name="info"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Tuple<bool, string>> UserAdd(SystemUsers info, Token user);

        /// <summary>
        /// 獲取系統使用者資訊
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<SystemUsers> GetUserInfo(Token user);

        /// <summary>
        /// 更新個人資訊
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> UpdateUserInfo(SystemUsers user);

        /// <summary>
        /// 更新密碼
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<string> UpdatePwd(UpdatePwdVm user);
    }
}