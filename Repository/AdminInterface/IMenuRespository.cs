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
    using System.Collections.Generic;


    /// <summary>
    /// 選單處理
    /// </summary>
    public interface IMenuRespository : IRepository<SystemMenu>
    {
        /// <summary>
        /// 禁用某選單
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        string DisableMenu(long menuTid);
        /// <summary>
        /// 載入menuTree
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        SystemMenuSM GetCurrentMenu(long menuTid);
        /// <summary>
        /// 更新某個選單的資訊
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string UpdateMenu(AddMenuVm model);
        /// <summary>
        /// 新增Menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string AddMenu(AddMenuVm model);
        /// <summary>
        /// 獲取子選單
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        List<SystemMenuSM> GetSubMenus(long menuTid);
        /// <summary>
        /// 獲取所有父選單
        /// </summary>
        /// <returns></returns>
        List<SystemMenuSM> GetAllParentMenus();
        /// <summary>
        /// 取得兩層父選單
        /// </summary>
        /// <returns></returns>
        List<SystemMenuSM> GetLv1And2Menus();
        /// <summary>
        /// 獲取所有選單
        /// </summary>
        /// <returns></returns>
        List<SystemMenuSM> GetAllMenus();
        /// <summary>
        /// 獲取目前使用者的所有的選單
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="menuRights"></param>
        /// <param name="isGod"></param>
        /// <returns></returns>
        List<SystemMenuSM> GetAllRightsMenus(string eid, string menuRights, bool isGod = false);
        /// <summary>
        /// 是否有對當前Url訪問的許可權
        /// </summary>
        /// <param name="currentUrl"></param>
        /// <param name="menuRights"></param>
        /// <returns></returns>
        long HaveMenuPermission(string currentUrl, string menuRights);
        /// <summary>
        /// 檢視是否有操作許可權
        /// </summary>
        /// <param name="menuTid"></param>
        /// <param name="roleTid"></param>
        /// <param name="controlname"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        bool HaveActionPermission(long menuTid, long roleTid, string controlname, string actionName);
        /// <summary>
        /// 載入menuTree
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userToken"></param>
        /// <param name="roleSencondeID"></param>
        /// <returns></returns>
        List<SystemMenuSM> GetMenuTree(long roleId, Token userToken, long roleSencondeID = 0);
    }
}