﻿using AntData.ORM;
using Autofac.Annotation;
using Configuration;
using Infrastructure.StaticExt;
using Infrastructure.View;
using Infrastructure.Web;
using Mapping;
using Newtonsoft.Json;
using Repository.Interface;
using ServicesModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DbModel;
using Repository.Interceptors;
using ViewModels.Reuqest;

namespace Repository
{
    /// <summary>
    /// 選單處理
    /// </summary>
    [Component]
    public class MenuRespository : BaseRepository<SystemMenu>, IMenuRespository
    {

        private static readonly int DisableMenuLevel = 88;
        private static readonly ConcurrentDictionary<string, List<SystemMenuSM>> _cache = new ConcurrentDictionary<string, List<SystemMenuSM>>();

        /// <summary>
        /// Autofac屬性注入
        /// </summary>
        [Autowired]
        public IViewRenderService ViewRenderService { get; set; }

        #region Menu

        /// <summary>
        /// 獲取目前使用者的所有的選單
        /// </summary>
        /// <returns></returns>
        public List<SystemMenuSM> GetAllRightsMenus(string eid, string menuRights, bool isGod = false)
        {
            var isGlod = GlobalSetting.SuperList.Contains(eid) || isGod;
            if (string.IsNullOrEmpty(menuRights) && !isGlod)
            {
                return new List<SystemMenuSM>();
            }
            var right = new BigInteger(menuRights ?? "0");
            var allMenus = this.Entity.Where(r => r.IsActive).MappperTo<SystemMenuSM>().ToList();
            var parentMenu = allMenus.Where(r => r.ParentTid.Equals(0) && (right.TestBit((int)r.Tid) || isGlod))
                .OrderBy(r => r.OrderRule).ToList();
            //遞迴構建
            addActionChildren(allMenus, right, isGlod, ref parentMenu);
            return parentMenu;
        }

        /// <summary>
        /// 是否有對當前Url訪問的許可權
        /// </summary>
        /// <param name="currentUrl"></param>
        /// <param name="menuRights"></param>
        /// <returns></returns>
        public long HaveMenuPermission(string currentUrl, string menuRights)
        {
            if (string.IsNullOrEmpty(currentUrl) || string.IsNullOrEmpty(menuRights))
            {
                return -1;
            }
            currentUrl = "~/" + currentUrl;
            var menu = this.Entity.FirstOrDefault(r => r.Url.Equals(currentUrl));
            if (menu == null)
            {
                return 0;
            }

            if (menu.Level.HasValue && menu.Level.Value == DisableMenuLevel)//已經被邏輯刪除了
            {
                return 0;
            }

            var rights = new BigInteger(menuRights);
            if (!rights.TestBit((int)menu.Tid))
            {
                return -1;
            }
            return menu.Tid;
        }

        /// <summary>
        /// 檢視是否有操作許可權
        /// </summary>
        /// <returns></returns>
        public bool HaveActionPermission(long menuTid, long roleTid, string controlname, string actionName)
        {
            if (menuTid < 1 || string.IsNullOrEmpty(controlname) || string.IsNullOrEmpty(actionName) || roleTid < 1)
            {
                return false;
            }

            var role = this.Entitys.SystemRole.FirstOrDefault(r => r.Tid.Equals(roleTid));
            if (role == null)
            {
                return false;
            }

            var actionList = JsonConvert.DeserializeObject<List<MenuActionSM>>(role.ActionList)
                .ToDictionary(r => r.MenuTid, g => g.ActionList);

            if (!actionList.ContainsKey(menuTid))
            {
                return false;
            }

            //這個人擁有的按鈕許可權集合
            var ActionList = actionList[menuTid].Select(r => r.ActionId).Distinct().ToList();

            //按鈕許可權對應介面的許可權集合
            var list = Entitys.SystemPageAction.Where(r =>
                r.MenuTid.Equals(menuTid) && ActionList.Contains(r.ActionId)).ToList();

            var controlActionIdList = list.Select(r => r.ControlName + "@" + r.ActionName).Distinct().ToList();

            return controlActionIdList.Any(r => r.Equals(controlname + "@" + actionName));
        }

        /// <summary>
        /// 載入menuTree
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userToken"></param>
        /// <param name="roleSencondeID">遞迴專用</param>
        /// <returns></returns>
        public List<SystemMenuSM> GetMenuTree(long roleId, Token userToken, long roleSencondeID = 0)
        {
            var isGod = GlobalSetting.SuperList.Contains(userToken.Eid);
            var isIgnoreMune = false;
            if (roleId < 1)
            {
                if (isGod)
                {
                    //獲取所有的選單
                    var allMenus = GetAllRightsMenusTwo(string.Empty, null, true);
                    GetCheckAllChild(allMenus);
                    return allMenus;
                }

                isIgnoreMune = true;
                roleId = userToken.RoleTid;
                if (roleSencondeID > 0)
                {
                    roleId = roleSencondeID;
                }
            }

            var systemRole = this.Entitys.SystemRole.FirstOrDefault(r => r.IsActive && r.Tid.Equals(roleId));
            if (systemRole == null) return new List<SystemMenuSM>();

            var menuRights = systemRole.MenuRights;
            if (string.IsNullOrEmpty(systemRole.ActionList))
            {
                systemRole.ActionList = "[]";
            }
            var actionList = JsonConvert.DeserializeObject<List<MenuActionSM>>(systemRole.ActionList);
            var right = new BigInteger(menuRights ?? "0");

            List<SystemMenuSM> allMenusList;
            if (!isGod && !isIgnoreMune)
            {
                allMenusList = GetMenuTree(0, userToken);
                var newAllMenuList = new List<SystemMenuSM>();
                GetAllChild(allMenusList, ref newAllMenuList);
                allMenusList = newAllMenuList;
                allMenusList.ForEach(r => r.ChildMunuList = new List<SystemMenuSM>());
            }
            else
            {
                allMenusList = this.Entity.Where(r => r.Level != DisableMenuLevel).MappperTo<SystemMenuSM>().ToList();

            }
            var parentMenu = allMenusList.Where(r => r.ParentTid.Equals(0))
                .OrderBy(r => r.OrderRule).ToList();
            parentMenu.ForEach(r => r.HasMenu = right.TestBit((int)r.Tid));
            if (isIgnoreMune)
            {
                parentMenu = parentMenu.Where(r => r.HasMenu).ToList();
            }
            //遞迴構建
            addActionChildrenTwo(allMenusList, right, actionList.ToDictionary(r => r.MenuTid, g => g.ActionList), isIgnoreMune, ref parentMenu);
            if (isIgnoreMune)
            {
                parentMenu.ForEach(r => r.HasMenu = false);
            }

            GetCheckAllChild(parentMenu);
            return parentMenu;
        }

        /// <summary>
        /// 禁用某選單
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        public string DisableMenu(long menuTid)
        {
            var updateResult = this.Entity.Where(r => r.Tid.Equals(menuTid))
                .Set(r => r.DataChangeLastTime, DateTime.Now)
                .Set(r => r.IsActive, false)
                .Set(r => r.Level, DisableMenuLevel)//目前最多支援2級。。 88是代表這個選單不用了
                .Update() > 0;
            return !updateResult ? Tip.UpdateError : String.Empty;
        }

        /// <summary>
        /// 獲取目前的Menu資訊
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        public SystemMenuSM GetCurrentMenu(long menuTid)
        {
            var menu = this.Entity.FirstOrDefault(r => r.Tid.Equals(menuTid));
            return menu != null ? MapperTo<SystemMenu, SystemMenuSM>(menu) : new SystemMenuSM();
        }

        /// <summary>
        /// 更新某個選單的資訊
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string UpdateMenu(AddMenuVm model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Url) || model.Tid < 1)
            {
                return Tip.BadRequest;
            }

            //取得上一層層級
            var p = this.Entity.Where(r => r.Tid.Equals(model.ParentTid)).FirstOrDefault();

            var updateResult = this.Entity.Where(r => r.Tid.Equals(model.Tid))
                .Set(r => r.DataChangeLastTime, DateTime.Now)
                .Set(r => r.Class, model.Class)
                .Set(r => r.Name, model.Name)
                .Set(r => r.Url, model.Url)
                .Set(r => r.ParentTid, model.ParentTid)
                .Set(r => r.OrderRule, model.OrderRule)
                .Set(r => r.IsActive, model.IsActive)
                .Set(r => r.Level, p.Level + 1)
                .Update() > 0;
            return !updateResult ? Tip.UpdateError : String.Empty;
        }

        /// <summary>
        /// 新增Menu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string AddMenu(AddMenuVm model)
        {
            if (model == null || string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Url))
            {
                return Tip.BadRequest;
            }
            var myLevel = 1;
            if (model.ParentTid != 0)
            {
                var parentMenu = this.Entity.FirstOrDefault(r => r.Tid.Equals(model.ParentTid));
                if (parentMenu != null && parentMenu.Level != null)
                {
                    var pLevel = parentMenu.Level.Value;
                    myLevel = pLevel + 1;
                }
            }
            //選單最多2級 
            //2021-09-29 Hikaru 改為3級
            if (myLevel > 3)
            {
                return Tip.MenuOverFlow;
            }

            SystemMenu menu = new SystemMenu
            {
                Class = model.Class,
                IsActive = model.IsActive,
                Name = model.Name,
                Url = model.Url,
                OrderRule = model.OrderRule,
                ParentTid = model.ParentTid,
                Level = myLevel,
                DataChangeLastTime = DateTime.Now
            };
            var result = this.Save(menu) > 0;
            return !result ? Tip.InserError : String.Empty;
        }

        /// <summary>
        /// 獲取子選單
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        public List<SystemMenuSM> GetSubMenus(long menuTid)
        {
            var allMenus = this.Entity.Where(r => r.ParentTid == menuTid && r.Level != DisableMenuLevel)
                .OrderBy(r => r.OrderRule)
                .MappperTo<SystemMenuSM>()
                .ToList();
            return allMenus;
        }

        /// <summary>
        /// 獲取所有父選單
        /// </summary>
        /// <returns></returns>
        public List<SystemMenuSM> GetAllParentMenus()
        {
            var allMenus = this.Entity.Where(r => r.ParentTid == 0 && r.Level != DisableMenuLevel)
                .OrderBy(r => r.OrderRule)
                .MappperTo<SystemMenuSM>().ToList();

            return allMenus;
        }

        /// <summary>
        /// 取得兩層父選單
        /// </summary>
        /// <returns></returns>
        public List<SystemMenuSM> GetLv1And2Menus()
        {
            var allMenus = this.Entity.Where(r => r.Level <= 2) //取得兩層父選單
                .OrderBy(r => r.OrderRule)
                .MappperTo<SystemMenuSM>().ToList();
            return allMenus;
        }

        /// <summary>
        /// 獲取所有選單
        /// </summary>
        /// <returns></returns>
        public List<SystemMenuSM> GetAllMenus()
        {
            var allMenus = this.Entity //取得兩層父選單
                .OrderBy(r => r.OrderRule)
                .MappperTo<SystemMenuSM>().ToList();
            return allMenus;
        }

        #endregion

        #region Private

        private void GetAllChild(List<SystemMenuSM> allMenusList, ref List<SystemMenuSM> newAllMenuList)
        {
            foreach (var all in allMenusList)
            {
                if (all.Tid < 1)
                {
                    continue;
                }
                newAllMenuList.Add(all);
                if (all.ChildMunuList != null && all.ChildMunuList.Count > 0)
                {
                    GetAllChild(all.ChildMunuList, ref newAllMenuList);
                }
            }
        }
        private void GetCheckAllChild(List<SystemMenuSM> allMenusList)
        {
            foreach (var all in allMenusList)
            {
                if (all.Tid < 1)
                {
                    all.ChildMunuList = null;
                    continue;
                }
                if (all.ChildMunuList != null && all.ChildMunuList.Count > 0)
                {
                    if (all.ChildMunuList.All(r => r.HasMenu)) all.HasMenu = true;
                    GetCheckAllChild(all.ChildMunuList);
                }
            }
        }

        private void addActionChildren(List<SystemMenuSM> sysActionSMList, BigInteger right, bool isGod,
            ref List<SystemMenuSM> systemActionSM)
        {
            foreach (var item in systemActionSM)
            {
                var parentId = item.Tid;
                var parentActionList = sysActionSMList.Where(r => r.ParentTid.Equals(parentId)
                                                                  &&
                                                                  (right.TestBit((int)r.Tid) ||
                                                                   isGod))
                    .OrderBy(r => r.OrderRule).ToList();
                if (parentActionList.Any())
                {
                    addActionChildren(sysActionSMList, right, isGod, ref parentActionList);
                    item.ChildMunuList.AddRange(parentActionList);
                }
            }
        }

        private void addActionChildrenTwo(List<SystemMenuSM> sysActionSMList, BigInteger right, Dictionary<long, List<ActionSM>> acList,
            bool ignoreHasMenu, ref List<SystemMenuSM> systemActionSM)
        {
            foreach (var item in systemActionSM)
            {
                var parentId = item.Tid;
                var parentActionList = sysActionSMList.Where(r => r.ParentTid.Equals(parentId))
                    .OrderBy(r => r.OrderRule).ToList();
                if (ignoreHasMenu)
                {
                    parentActionList.ForEach(r => r.HasMenu = right.TestBit((int)r.Tid));
                    parentActionList = parentActionList.Where(r => r.HasMenu).ToList();
                    parentActionList.ForEach(r => r.HasMenu = false);
                }
                if (parentActionList.Any())
                {
                    parentActionList.ForEach(r =>
                    {
                        //如果含有url 認為是menu節點 就去解析 拿到actionList
                        if (!string.IsNullOrEmpty(r.Url) && !r.Url.Trim().Equals("#"))
                        {
                            var hasP = acList.ContainsKey(r.Tid);
                            r.ChildMunuList.Add(new SystemMenuSM
                            {
                                ActionId = "index",
                                Name = "頁面訪問",
                                HasMenu = hasP && acList[r.Tid].Exists(p => p.ActionId.Equals("index")),
                                ParentTid = r.Tid
                            });

                            var actionList = GetActionsList(r.Url);
                            actionList.ForEach(y =>
                            {
                                y.HasMenu = hasP && acList[r.Tid].Exists(p => p.ActionId.Equals(y.ActionId));
                                y.ParentTid = r.Tid;
                            });
                            r.ChildMunuList.AddRange(actionList);
                            if (ignoreHasMenu)
                            {
                                r.ChildMunuList = r.ChildMunuList.Where(r1 => r1.HasMenu).ToList();
                                r.ChildMunuList.ForEach(r1 => r1.HasMenu = false);
                            }
                        }
                    });
                    addActionChildrenTwo(sysActionSMList, right, acList, ignoreHasMenu, ref parentActionList);
                    item.ChildMunuList.AddRange(parentActionList);
                }


            }
        }


        private List<SystemMenuSM> GetAllRightsMenusTwo(string eid, string menuRights, bool isGod = false)
        {
            var isGlod = GlobalSetting.SuperList.Contains(eid) || isGod;
            if (string.IsNullOrEmpty(menuRights) && !isGlod)
            {
                return new List<SystemMenuSM>();
            }
            var right = new BigInteger(menuRights ?? "0");
            var allMenus = this.Entity.Where(r => r.Level != DisableMenuLevel).MappperTo<SystemMenuSM>().ToList();
            var parentMenu = allMenus.Where(r => r.ParentTid.Equals(0) && (right.TestBit((int)r.Tid) || isGlod))
                .OrderBy(r => r.OrderRule).ToList();
            //遞迴構建
            addActionChildrenThree(allMenus, right, isGod, ref parentMenu);
            return parentMenu;
        }

        private void addActionChildrenThree(List<SystemMenuSM> sysActionSMList, BigInteger right, bool isGod,
           ref List<SystemMenuSM> systemActionSM)
        {
            foreach (var item in systemActionSM)
            {
                var parentId = item.Tid;
                var parentActionList = sysActionSMList.Where(r => r.ParentTid.Equals(parentId)
                                                                  &&
                                                                  (right.TestBit((int)r.Tid) ||
                                                                   isGod))
                    .OrderBy(r => r.OrderRule).ToList();
                if (parentActionList.Any())
                {
                    parentActionList.ForEach(r =>
                    {
                        //如果含有url 認為是menu節點 就去解析 拿到actionList
                        if (!string.IsNullOrEmpty(r.Url) && !r.Url.Trim().Equals("#"))
                        {
                            r.ChildMunuList.Add(new SystemMenuSM
                            {
                                ActionId = "index",
                                Name = "頁面訪問",
                                ParentTid = r.Tid

                            });
                            var actionList = GetActionsList(r.Url);
                            actionList.ForEach(y => y.ParentTid = r.Tid);
                            r.ChildMunuList.AddRange(actionList);
                        }
                    });
                    addActionChildrenThree(sysActionSMList, right, isGod, ref parentActionList);
                    item.ChildMunuList.AddRange(parentActionList);
                }
            }
        }

        /// <summary>
        /// 解析頁面 獲取action
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private List<SystemMenuSM> GetActionsList(string url)
        {
            if (string.IsNullOrEmpty(url)) return new List<SystemMenuSM>();
            if (url.StartsWith("http")) return new List<SystemMenuSM>();

            if (_cache.TryGetValue(url, out var cache)) return cache;
            var body = ViewRenderService.RenderToStringAsync(url.Replace("~/", ""), null).ConfigureAwait(false).GetAwaiter().GetResult();
            var actionList = body.GetValueAndNameByClass("action-id", "action-name", "authorization");
            cache = actionList.Select(r => new SystemMenuSM
            {
                ActionId = r.Key,
                Name = r.Value
            }).ToList();

            _cache.TryAdd(url, cache);

            return cache;
        }

        #endregion
    }
}
