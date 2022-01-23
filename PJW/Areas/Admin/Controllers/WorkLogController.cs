using System.Collections.Generic;
using System.Threading.Tasks;
using PJW.Web.Filter;
using Configuration;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Repository.Interface;
using ServicesModel;
using ViewModels.Result;
using ViewModels.Reuqest;
using Repository;
using System.Data;
using ViewModels.Condition;
using System.Linq;
using System;

namespace PJW.Web.Admin.Controllers
{
    [AuthorizeFilter]
    [API("工作任務管理")]
    [Area(nameof(Admin))]
    [Route("Admin/[controller]/[action]")]
    public class WorkLogController : BaseController
    {
        public readonly MyConnectionConfig _myConnectionConfig;
        public WorkLogController(MyConnectionConfig myConnectionConfig)
        {
            _myConnectionConfig = myConnectionConfig;
        }


        [API("工作分類頁面訪問")]
        public IActionResult SortList()
        {
            return View();
        }

        [API("工作分類頁面列表")]
        [HttpPost]
        public async Task<JsonResult> GetWorkLogSortList([FromForm] ConditionBase model)
        {
            var result = new SearchResult<List<dynamic>>();
            List<dynamic> sortData = new List<dynamic>();
            List<dynamic> deptData = new List<dynamic>();
            //實作DapperBase , 設定DB連線
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            DapperBase intraDB = new(_myConnectionConfig.IntranetDB);

            //Linq 的 JOIN 範例, 暫時用不到
            //using (IDbConnection connection = intraDB.OpenConnection()) {

            //    deptData = connection.Query("SELECT indx, nameC FROM department WHERE deletemark='N' ORDER BY sort").AsList();
            //}
            //using (IDbConnection connection = pjwDB.OpenConnection()) {

            //    sortData = connection.Query("SELECT * FROM work_log_sort").AsList();
            //}

            //var outData = (from s in sortData
            //          join d in deptData on s.work_log_sort_dept_no equals d.indx
            //          orderby s.work_log_sort_adddate descending
            //          select new { 
            //              SortNo = s.work_log_sort_no,
            //              SortName = s.work_log_sort_name,
            //              DepartName = d.nameC
            //          });

            string strSQL = @"SELECT
                        A.work_log_sort_no sortNo,
                        A.work_log_sort_name sortName,
                        B.nameC deptNo,
                        ISNULL(C.work_log_cnt, 0) work_log_cnt
                        FROM PJW.dbo.work_log_sort A
                        LEFT JOIN Intranet.dbo.department B
                        ON A.work_log_sort_dept_no = B.indx
                        LEFT JOIN (
	                        SELECT count(work_log_no) AS work_log_cnt, work_log_sort 
	                        FROM work_log
	                        GROUP BY work_log_sort
                        ) C ON A.work_log_sort_no = C.work_log_sort";
            //using (IDbConnection connection = pjwDB.OpenConnection()) {
            //    sortData = connection.Query(strSQL).AsList();
            //}

            //設定排序
            List<SortDescriptor> sortings = new();
            model.OrderBy = string.IsNullOrWhiteSpace(model.OrderBy) ? "sortNo" : model.OrderBy;
            if (!string.IsNullOrEmpty(model.OrderBy)) {
                sortings.Add(new SortDescriptor
                {
                    Direction = !string.IsNullOrEmpty(model.OrderSequence) & model.OrderSequence.ToLower() == "desc" ? SortDescriptor.SortingDirection.Descending : SortDescriptor.SortingDirection.Ascending,
                    Field = model.OrderBy
                });
            }
            Tuple<IEnumerable<dynamic>, int> data = await pjwDB.FindWithOffsetFetch(strSQL, null, model.PageIndex, model.PageSize, sortings);

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Total = data.Item2;
            result.Rows = data.Item1.ToList();
            return Json(result);
        }

        [API("工作分類修改頁面")]
        [HttpPost]
        public async Task<JsonResult> GetWorkLogSort([FromForm] string sortNo)
        {
            var result = new ResultJsonInfo<dynamic>();
            List<dynamic> sortData = new List<dynamic>();
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = pjwDB.OpenConnection()) {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("sortNo", sortNo);
                sortData = (List<dynamic>)await connection.QueryAsync(
                    "SELECT " +
                    " work_log_sort_no sortNo, " +
                    " work_log_sort_name sortName, " +
                    " work_log_sort_dept_no deptNo " +                    
                    "FROM work_log_sort " + 
                    "WHERE work_log_sort_no = @sortNo"                    
                    , parameters);
            }

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = sortData;
            return Json(result);

        }

        [API("獲取部門選單")]
        public JsonResult GetDeptList()
        {
            var result = new ResultJsonInfo<List<dynamic>>();
            List<dynamic> data = new List<dynamic>();
            //實作DapperBase , 設定DB連線
            DapperBase db = new(_myConnectionConfig.IntranetDB);
            using (IDbConnection connection = db.OpenConnection()) {
                data = connection.Query("SELECT indx AS deptNo, nameC AS deptName FROM department WHERE deletemark='N' ORDER BY sort").AsList();
            }


            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = data;
            return Json(result);
        }

        [API("工作任務分類新增")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> WorkLogSortAdd([FromForm] string sortName, string deptNo)
        {
            var result = new ResultJsonNoDataInfo();

            if (string.IsNullOrEmpty(sortName) || string.IsNullOrEmpty(deptNo)) {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessage;
                return Json(result);
            }

            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                string strSQL = $"INSERT INTO work_log_sort " + 
                    "(work_log_sort_name,work_log_sort_dept_no,work_log_sort_adduser_id,work_log_sort_adduser_name) " + 
                    "VALUES(@sortName, @deptNo, @addUserID, @addUserName) ";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("sortName", sortName);
                parameters.Add("deptNo", deptNo);
                parameters.Add("addUserID", UserToken.Indx);
                parameters.Add("addUserName", UserToken.Code);

                int effect = await connection.ExecuteAsync(strSQL, parameters);
                if (effect == 0) {
                    result.Status = ResultConfig.Fail;
                    result.Info = ResultConfig.FailMessage;
                } else {
                    result.Status = ResultConfig.Ok;
                    result.Info = ResultConfig.SuccessfulMessage;
                }
            }

            return Json(result);
        }

        [API("工作任務分類修改")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> WorkLogSortEdit([FromForm]string sortNo, string sortName, string deptNo)
        {
            var result = new ResultJsonNoDataInfo();

            if (string.IsNullOrEmpty(sortName) || string.IsNullOrEmpty(deptNo)) {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessage;
                return Json(result);
            }

            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                string strSQL = $"UPDATE work_log_sort SET " +
                    "work_log_sort_name = @sortName, " +
                    "work_log_sort_dept_no = @deptNo " +
                    "WHERE work_log_sort_no = @sortNo";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("sortNo", sortNo);
                parameters.Add("sortName", sortName);
                parameters.Add("deptNo", deptNo);

                int effect = await connection.ExecuteAsync(strSQL, parameters);
                if (effect == 0) {
                    result.Status = ResultConfig.Fail;
                    result.Info = ResultConfig.FailMessage;
                } else {
                    result.Status = ResultConfig.Ok;
                    result.Info = ResultConfig.SuccessfulMessage;
                }
            }

            return Json(result);
        }

        [API("工作任務分類刪除")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<JsonResult> WorkLogSortDelete([FromForm] string sortNo)
        {
            var result = new ResultJsonNoDataInfo();
            if (string.IsNullOrEmpty(sortNo)) {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessage;
                return Json(result);
            }

            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                string strSQL = $"DELETE work_log_sort WHERE work_log_sort_no = @sortNo";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("sortNo", sortNo);

                int effect = await connection.ExecuteAsync(strSQL, parameters);
                if (effect == 0) {
                    result.Status = ResultConfig.Fail;
                    result.Info = ResultConfig.FailMessage;
                } else {
                    result.Status = ResultConfig.Ok;
                    result.Info = ResultConfig.SuccessfulMessage;
                }
            }

            return Json(result);
        }
    }
}
