using System.Collections.Generic;
using PJW.Web.Filter;
using Configuration;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;
using System.Linq;

namespace PJW.Web.Admin.Controllers
{
    [ServiceFilter(typeof(AuthorizeFilterAttribute))]
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class HomeController : BaseController
    {
        private readonly IMenuRespository MenuRespository;

        public HomeController(IMenuRespository _menuRespository)
        {
            MenuRespository = _menuRespository;
        }
        public ActionResult Index()
        {
            var roleName = UserToken.RoleName;
            if (string.IsNullOrEmpty(roleName))
            {
                roleName = "無許可權";
            }
            if (GlobalSetting.SuperList.Contains(UserToken.Eid.ToLower()))
            {
                roleName = "管理者";
            }

            ViewBag.Eid = UserToken.Eid;
            ViewBag.RoleName = roleName;
            ViewBag.UserName = UserToken.Code;
            return View();
        }



        #region 選單


        /// <summary>
        /// 獲取所有的Menu
        /// </summary>
        /// <returns></returns>
        [API("選單頁面訪問")]
        public ActionResult MenuList()
        {
            return View();
        }


        /// <summary>
        /// 載入Menu Tree
        /// </summary>
        /// <returns></returns>
        [API("載入Menu Tree")]
        public JsonResult GetMenuTree([FromForm]string roleId = null)
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            var respositoryResult = MenuRespository.GetMenuTree(string.IsNullOrEmpty(roleId) ? 0 : long.Parse(roleId), UserToken);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }
        [API("獲取API集合")]
        public JsonResult GetControllerActions([FromForm]string roleId = null)
        {
            var result = new ResultJsonInfo<List<APIDescription>>();
            var respositoryResult = APIAttibuteHelper.GetAllDescriptions();
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        [API("載入所有的選單和按鈕")]
        public JsonResult GetMenuActionTree()
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            if (!GlobalSetting.SuperList.Contains(UserToken.Eid))
            {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessageForNoPower;
                result.Data = new List<SystemMenuSM>();
                return Json(result);
            }
            var respositoryResult = MenuRespository.GetMenuTree(0, UserToken);
            if (respositoryResult == null)
            {
                result.Status = ResultConfig.Fail;
                result.Info = "沒有配置任何選單";
                result.Data = new List<SystemMenuSM>();
                return Json(result);
            }
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }
        /// <summary>
        /// 獲取子選單
        /// </summary>
        /// <param name="menuTid"></param>
        /// <returns></returns>
        [API("獲取子選單")]
        public JsonResult GetSubMenu([FromForm]long menuTid)
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            var respositoryResult = MenuRespository.GetSubMenus(menuTid);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = respositoryResult;
            return Json(result);
        }

        [API("獲取全部一級選單")]
        public JsonResult GetAllParentMenus()
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            var allParentMenuList = MenuRespository.GetAllParentMenus();
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = allParentMenuList;
            return Json(result);
        }

        [API("取得兩層父選單")]
        public JsonResult GetLv1And2Menus()
        {
            var result = new ResultJsonInfo<List<SystemMenuSM>>();
            var allParentMenuList = MenuRespository.GetAllParentMenus();
            List<SystemMenuSM> Menu = new List<SystemMenuSM>();
            foreach (SystemMenuSM parent in allParentMenuList) {
                Menu.Add(parent);
                List<SystemMenuSM> subMenuList = MenuRespository.GetSubMenus(parent.Tid);
                Menu.AddRange(subMenuList);
                
            }

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = Menu;
            return Json(result);
        }

        [API("獲取全部選單")]
        public JsonResult GetAllMenus()
        {//var result = new SearchResult<List<SystemMenuSM>>();
            var result = new SearchResult<List<SystemMenuSM>>();
            List<SystemMenuSM> allMenuList = MenuRespository.GetAllMenus();
            List<SystemMenuSM> Menu = new List<SystemMenuSM>();

            foreach (var menuOne in allMenuList.Where(a => a.Level == 1).OrderBy(a => a.OrderRule)) //一級選單
            {
                Menu.Add(menuOne);
                foreach (var menuTwo in allMenuList.Where(a => a.Level == 2 && a.ParentTid == menuOne.Tid).OrderBy(a => a.OrderRule)) //二級選單
                {
                    Menu.Add(menuTwo);
                    foreach (var menuThree in allMenuList.Where(a => a.Level == 3 && a.ParentTid == menuTwo.Tid).OrderBy(a => a.OrderRule))
                    {
                        Menu.Add(menuThree);
                    }
                }
            }

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Rows = Menu;
            result.Total = Menu.Count;
            return Json(result);
        }

        /// <summary>
        /// 新增選單
        /// </summary>
        /// <returns></returns>
        [API("新增選單")]
        public JsonResult AddMenu([FromForm] AddMenuVm model)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = MenuRespository.AddMenu(model);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }

        /// <summary>
        /// 修改選單
        /// </summary>
        /// <returns></returns>
        [API("獲取修改的選單詳情")]
        public JsonResult GetEditMenu([FromForm] AddMenuVm model)
        {
            var result = new ResultJsonInfo<SystemMenuSM>();
            var CurrentMenu = MenuRespository.GetCurrentMenu(model.Tid);
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = CurrentMenu;
            return Json(result);
        }

        /// <summary>
        /// 修改選單
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [API("修改選單")]
        public JsonResult UpdateMenu([FromForm]AddMenuVm model)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = MenuRespository.UpdateMenu(model);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }
        /// <summary>
        /// 禁用選單
        /// </summary>
        /// <returns></returns>
        [API("禁用選單")]
        public JsonResult DisableMenu([FromForm] long menuTid)
        {
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = MenuRespository.DisableMenu(menuTid);
            if (string.IsNullOrEmpty(respositoryResult))
            {
                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult;
            }
            return Json(result);
        }

        #endregion




    }
}