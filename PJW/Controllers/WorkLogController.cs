using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using Microsoft.AspNetCore.Mvc;
using Repository;
using ViewModels.Condition;
using ViewModels.Result;
using Dapper;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using MimeKit;
using PJW.Web.Models;
using Microsoft.AspNetCore.Hosting;

namespace PJW.Web.Controllers
{

    public class WorkLogController : BaseController
    {
        public readonly MySectionConfig _mySectionConfig;
        public readonly MyConnectionConfig _myConnectionConfig;

        private readonly IWebHostEnvironment _Environment;

        public WorkLogController(MySectionConfig mySectionConfig, MyConnectionConfig myConnectionConfig, IWebHostEnvironment Environment)
        {
            _mySectionConfig = mySectionConfig;
            _myConnectionConfig = myConnectionConfig;
            _Environment = Environment;
        }
        public IActionResult List1()
        {
            ViewBag.UserToken = UserToken;
            return View();
        }
        public IActionResult List2()
        {
            ViewBag.UserToken = UserToken;
            return View();
        }
        public IActionResult CheckList1()
        {
            //判斷角色非主管不可進入、任務管理者

            ViewBag.UserToken = UserToken;
            return View();
        }
        public IActionResult CheckList2()
        {
            ViewBag.UserToken = UserToken;
            return View();
        }

        public IActionResult ShareSet()
        {
            ViewBag.UserToken = UserToken;
            return View();
        }

        public IActionResult ShareView()
        {
            ViewBag.UserToken = UserToken;
            return View();
        }

        /// <summary>
        /// 取得部門的人員
        /// </summary>
        /// <param name="deptNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetDeptUsers(string deptNo)
        {
            var result = new ResultJsonInfo<List<dynamic>>();
            List<dynamic> data = new();
            DapperBase db = new(_myConnectionConfig.IntranetDB);
            using (IDbConnection connection = db.OpenConnection()) {
                data = (List<dynamic>)await connection.QueryAsync(
                    "SELECT indx, first_name AS EngName, [name] AS ChgName " +
                    "FROM administration with(nolock) " +
                    $"WHERE 1=1 " +
                    "   and deletemark='N' " + 
                    $"  and department_id={deptNo} " +
                    $"  and indx != {UserToken.Indx} " +
                    "ORDER BY indx"
                );
            }
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = data;
            return Json(result);
        }

        /// <summary>
        /// 取得部門資訊
        /// </summary>
        /// <param name="deptNo"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDeptList(string deptNo)
        {
            var result = new ResultJsonInfo<List<dynamic>>();
            List<dynamic> data = new();
            //實作DapperBase , 設定DB連線
            DapperBase db = new(_myConnectionConfig.IntranetDB);
            using (IDbConnection connection = db.OpenConnection()) {
                string strSQL = "SELECT indx AS deptNo, nameC AS deptName ";
                strSQL += "FROM department ";
                strSQL += "WHERE deletemark='N' ";
                if (!string.IsNullOrWhiteSpace(deptNo)) {
                    strSQL += "AND indx = " + deptNo;
                }
                strSQL += "ORDER BY sort";
                data = connection.Query(strSQL).AsList();
            }


            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = data;
            return Json(result);
        }


        /// <summary>
        /// 取得所有任務需求等級
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetAllWorkLogPriority()
        {
            var result = new ResultJsonInfo<dynamic>
            {
                Status = ResultConfig.Ok,
                Info = ResultConfig.SuccessfulMessage,
                Data = Common.GetAllWorkLogPriority()
            };
            return Json(result);
        }

        /// <summary>
        /// 取得所有任務分類選單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetAllWorkLogSort()
        {

            var result = new ResultJsonInfo<List<dynamic>>();
            List<dynamic> data = new List<dynamic>();
            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                data = (List<dynamic>)await connection.QueryAsync(
                    "SELECT work_log_sort_no AS sortNo, work_log_sort_name AS sortName " +
                    "FROM work_log_sort " +
                    $"WHERE work_log_sort_dept_no={UserToken.DeptNo}" + 
                    "ORDER BY work_log_sort_no"
                    );
            }
            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = data;
            return Json(result);
        }

        /// <summary>
        /// 需求工作任務查詢
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="queryKey"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetWorkLogList(string startDate, string endDate, string queryKey, ConditionBase model)
        {
            var result = new SearchResult<List<dynamic>>();
            //實作DapperBase , 設定DB連線
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            //建立SQL語句
            SqlBuilder builder = new();
            string strSQL = "SELECT " +
                "   A.*, " +
                "   B.work_log_sort_no AS sortNo, " +
                "   B.work_log_sort_name AS sortName," + 
                "   C.indx AS deptNo, C.nameC AS deptName " + 
            "FROM work_log A " +
            "LEFT JOIN work_log_sort B " +
            "	ON A.work_log_sort = B.work_log_sort_no " +
            "LEFT JOIN Intranet.dbo.department C " +
            "	ON B.work_log_sort_dept_no = C.indx " +
            "/**where**/"; // /**where**/ <== 這是WHERE參數標籤
            var selector = builder.AddTemplate(strSQL);
            //WHERE 條件
            builder.Where("work_log_adduser_id = @addUserId", new { addUserId = UserToken.Indx }); //只撈出自己的任務
            if (!string.IsNullOrWhiteSpace(startDate)){
                startDate += " 00:00:00";
                builder.Where("work_log_adddate >= @startDate", new { startDate });
            }
            if (!string.IsNullOrWhiteSpace(endDate)) {
                endDate += " 23:59:59";
                builder.Where("work_log_adddate <= @endDate", new { endDate });
            }
            if (!string.IsNullOrWhiteSpace(queryKey)) {
                builder.OrWhere("work_log_subject like @subject", new { subject = $"%{queryKey}%" });
                builder.OrWhere("work_log_contents like @contents", new { contents = $"%{queryKey}%" });
            }
            //設定排序
            List<SortDescriptor> sortings = new();
            model.OrderBy = string.IsNullOrWhiteSpace(model.OrderBy) ? "work_log_no" : model.OrderBy;
            if (!string.IsNullOrEmpty(model.OrderBy)) {
                sortings.Add(new SortDescriptor
                {
                    Direction = !string.IsNullOrEmpty(model.OrderSequence) & model.OrderSequence.ToLower() == "desc" ? SortDescriptor.SortingDirection.Descending : SortDescriptor.SortingDirection.Ascending,
                    Field = model.OrderBy
                });
            }

            //var sql = selector.RawSql;
            //selector.RawSql 是把上面設定後的組成一個SQL語法
            //帶入分頁資訊
            //開始查詢資料，Tuple是一個很好用的東西 https://igouist.github.io/post/2020/09/csharp-trulp/
            Tuple<IEnumerable<dynamic>, int> data = await pjwDB.FindWithOffsetFetch(selector.RawSql, selector.Parameters, model.PageIndex, model.PageSize, sortings);

            IEnumerable<dynamic> rec = data.Item1
                .Select(g => new
                {
                    g.work_log_no,
                    g.work_log_subject,
                    g.sortNo,
                    g.sortName,
                    g.work_log_priority_status,
                    g.work_log_status,
                    g.work_log_type,
                    workLogPriority = Common.GetWorkLogPriority(g.work_log_priority_status),
                    workLogStatus = Common.GetWorkLogStatus(g.work_log_status),
                    workLogType = Common.GetWorkLogType(g.work_log_type),
                    work_log_adddate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", g.work_log_adddate)
                });

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Total = data.Item2;
            result.Rows = rec.ToList();
            return Json(result);
        }

        /// <summary>
        /// 以狀態來取得工作紀錄
        /// </summary>
        /// <param name="workLogStatus"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="queryKey"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<JsonResult> GetWorkLogListByStatus(string workLogStatus, string startDate, string endDate, string queryKey, ConditionBase model)
        {
            var result = new SearchResult<List<dynamic>>();
            //實作DapperBase , 設定DB連線
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            //建立SQL語句
            SqlBuilder builder = new();
            string strSQL = "SELECT " +
                "   A.*, " +
                "   B.work_log_sort_no AS sortNo, " +
                "   B.work_log_sort_name AS sortName," +
                "   C.indx AS deptNo, C.nameC AS deptName " +
            "FROM work_log A " +
            "LEFT JOIN work_log_sort B " +
            "	ON A.work_log_sort = B.work_log_sort_no " +
            "LEFT JOIN Intranet.dbo.department C " +
            "	ON B.work_log_sort_dept_no = C.indx " +
            "LEFT JOIN Intranet.dbo.administration D " +
            "	ON A.work_log_adduser_id = D.indx " +
            "/**where**/"; // /**where**/ <== 這是WHERE參數標籤
            var selector = builder.AddTemplate(strSQL);
            //WHERE 條件            
            switch (workLogStatus) {
                case "1":
                    builder.Where("D.department_id = @deptNo", new { deptNo = UserToken.DeptNo }); //需求單位的瑞
                    break;
                case "2":
                    builder.Where("work_log_assign_dept_id = @deptNo", new { deptNo = UserToken.DeptNo }); //執行單位的任務
                    break;
                case "3":
                    builder.Where("work_log_assign_id = @assignUserId", new { assignUserId = UserToken.Indx }); //取得指派給自己的任務
                    break;
                case "12": //給需求主管和執行主管做分享用
                    builder.OrWhere("D.department_id = @deptNo", new { deptNo = UserToken.DeptNo });
                    builder.OrWhere("work_log_assign_dept_id = @deptNo", new { deptNo = UserToken.DeptNo });
                    break;
                default:
                    builder.Where("work_log_adduser_id = @addUserId", new { addUserId = UserToken.Indx }); //只撈出自己的需求任務
                    break;
            }
            
            if (!string.IsNullOrWhiteSpace(startDate)) {
                startDate += " 00:00:00";
                builder.Where("work_log_adddate >= @startDate", new { startDate });
            }
            if (!string.IsNullOrWhiteSpace(endDate)) {
                endDate += " 23:59:59";
                builder.Where("work_log_adddate <= @endDate", new { endDate });
            }
            if (!string.IsNullOrWhiteSpace(queryKey)) {
                builder.OrWhere("work_log_subject like @subject", new { subject = $"%{queryKey}%" });
                builder.OrWhere("work_log_contents like @contents", new { contents = $"%{queryKey}%" });
            }
            //設定排序
            List<SortDescriptor> sortings = new();
            model.OrderBy = string.IsNullOrWhiteSpace(model.OrderBy) ? "work_log_no" : model.OrderBy;
            if (!string.IsNullOrEmpty(model.OrderBy)) {
                sortings.Add(new SortDescriptor
                {
                    Direction = !string.IsNullOrEmpty(model.OrderSequence) & model.OrderSequence.ToLower() == "desc" ? SortDescriptor.SortingDirection.Descending : SortDescriptor.SortingDirection.Ascending,
                    Field = model.OrderBy
                });
            }

            //var sql = selector.RawSql;
            //selector.RawSql 是把上面設定後的組成一個SQL語法
            //帶入分頁資訊
            //開始查詢資料，Tuple是一個很好用的東西 https://igouist.github.io/post/2020/09/csharp-trulp/
            Tuple<IEnumerable<dynamic>, int> data = await pjwDB.FindWithOffsetFetch(selector.RawSql, selector.Parameters, model.PageIndex, model.PageSize, sortings);

            IEnumerable<dynamic> rec = data.Item1
                .Select(g => new
                {
                    g.work_log_no,
                    g.work_log_subject,
                    g.sortNo,
                    g.sortName,
                    g.work_log_priority_status,
                    g.work_log_status,
                    g.work_log_type,
                    workLogPriority = Common.GetWorkLogPriority(g.work_log_priority_status),
                    workLogStatus = Common.GetWorkLogStatus(g.work_log_status),
                    workLogType = Common.GetWorkLogType(g.work_log_type),
                    work_log_adddate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", g.work_log_adddate)
                });

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Total = data.Item2;
            result.Rows = rec.ToList();
            return Json(result);
        }

        /// <summary>
        /// 取得此任務的回覆說明清單
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetWorkLogReplyList(string workLogNo)
        {
            var result = new SearchResult<List<dynamic>>();
            List<dynamic> workLogReplyData = new List<dynamic>();
            //實作DapperBase , 設定DB連線
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = pjwDB.OpenConnection()) {
                string strSQL = "SELECT " +
                        "   A.*, " +
                        "   B.first_name AS EngName, " +
                        "   B.[name] AS ChgName," +
                        "   C.nameC AS DeptName " +
                    "FROM work_log_reply A " +
                    "LEFT JOIN Intranet.dbo.administration B " +
                    "	ON A.work_log_reply_adduser_id = B.indx " +
                    "LEFT JOIN Intranet.dbo.department C " +
                    "	ON B.department_id = C.indx " +
                    "WHERE 1=1 " +
                    "  AND A.work_log_no=@workLogNo " +
                    "ORDER BY work_log_reply_no desc";
                
                DynamicParameters parameters = new();
                parameters.Add("workLogNo", workLogNo);
                workLogReplyData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
            }


            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Total = workLogReplyData.Count;
            result.Rows = workLogReplyData.ToList();
            return Json(result);
        }

        /// <summary>
        /// 取得任務回覆的檔案清單
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <param name="workLogReplyNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetWorkLogReplyFileList(string workLogReplyNo)
        {
            var result = new SearchResult<List<dynamic>>();
            List<dynamic> workLogReplyFileData = new List<dynamic>();
            //實作DapperBase , 設定DB連線
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = pjwDB.OpenConnection()) {
                string strSQL = "SELECT * " +                        
                    "FROM work_log_reply_file " +                    
                    "WHERE 1=1 " +
                    "  AND work_log_reply_no=@workLogReplyNo";

                DynamicParameters parameters = new();
                parameters.Add("workLogReplyNo", workLogReplyNo);
                workLogReplyFileData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
            }


            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Total = workLogReplyFileData.Count;
            result.Rows = workLogReplyFileData.ToList();
            return Json(result);
        }

        /// <summary>
        /// 取得單筆WorkLog，非詳細
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetWorkLog(string workLogNo)
        {
            var result = new ResultJsonInfo<List<dynamic>>();
            List<dynamic> workLogData = new List<dynamic>();
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = pjwDB.OpenConnection()) {
                string strSQL = "SELECT " +
                    "   A.*, " +
                    "   B.work_log_sort_no AS sortNo, " +
                    "   B.work_log_sort_name AS sortName," +
                    "   C.indx AS deptNo, C.nameC AS deptName " +
                "FROM work_log A " +
                "LEFT JOIN work_log_sort B " +
                "	ON A.work_log_sort = B.work_log_sort_no " +
                "LEFT JOIN Intranet.dbo.department C " +
                "	ON B.work_log_sort_dept_no = C.indx " +
                "WHERE A.work_log_no=@workLogNo ";
                DynamicParameters parameters = new();
                parameters.Add("workLogNo", workLogNo);
                workLogData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
                
            }

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = workLogData;
            return Json(result);
        }

        /// <summary>
        /// 取得詳細資料
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> WorkLogDetail(string workLogNo)
        {
            var result = new ResultJsonInfo<List<dynamic>>();
            List<dynamic> workLogData = new List<dynamic>();
            WorkLogViewModel workLogView = new();
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = pjwDB.OpenConnection()) {
                string strSQL = "SELECT " +
                    "   A.*, " +
                    "   B.work_log_sort_no AS sortNo, " +
                    "   B.work_log_sort_name AS sortName," +
                    "   C.indx AS deptNo, C.nameC AS deptName, " +
                    "   D.indx AS assignDeptNo, C.nameC AS assignDeptName " +
                "FROM work_log A " +
                "LEFT JOIN work_log_sort B " +
                "	ON A.work_log_sort = B.work_log_sort_no " +
                "LEFT JOIN Intranet.dbo.department C " +
                "	ON B.work_log_sort_dept_no = C.indx " +
                "LEFT JOIN Intranet.dbo.department D " +
                "	ON A.work_log_assign_dept_id = D.indx " +
                "WHERE A.work_log_no=@workLogNo ";
                DynamicParameters parameters = new();
                parameters.Add("workLogNo", workLogNo);
                workLogData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
                IEnumerable<dynamic> rec = workLogData
                .Select(g => new WorkLogViewModel
                {
                    WorkLogNo = g.work_log_no,
                    WorkLogSubject = g.work_log_subject,
                    WorkLogContent = g.work_log_contents,
                    WorkLogSortNo = g.sortNo,
                    WorkLogSortName = g.sortName,
                    WorkLogDeptName = g.deptName,
                    WorkLogPriority = g.work_log_priority_status,
                    WorkLogStatus = g.work_log_status,
                    WorkLogType = g.work_log_type,
                    WorkLogAddUserName = g.work_log_adduser_name,
                    WorkLogAssignDeptName = g.assignDeptName,
                    WorkLogAssignName = g.work_log_assign_name,
                    WorkLogPriorityName = Common.GetWorkLogPriority(g.work_log_priority_status),
                    WorkLogStatusName = Common.GetWorkLogStatus(g.work_log_status),
                    WorkLogTypeName = Common.GetWorkLogType(g.work_log_type),
                    WorkLogAddDate = g.work_log_adddate,
                    WorkLogScore = g.work_log_score
                });

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
                result.Data = rec.ToList<dynamic>();
            }

            
            return Json(result);
        }

        /// <summary>
        /// 新增工作任務
        /// </summary>
        /// <param name="addWorkLogFile"></param>
        /// <param name="addSortComb"></param>
        /// <param name="addWorkLogSubject"></param>
        /// <param name="addWorkLogContent"></param>
        /// <param name="addWorkLogPriority"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WorkLogAdd(List<IFormFile> addWorkLogFile,
            string addSortComb, string addWorkLogSubject, string addWorkLogContent, string addWorkLogPriority)
        {

            var result = new ResultJsonInfo<dynamic>();

            //取得預設的檔案上傳路徑
            string fileUploadPath = ConfigHelper.GetConfig<string>("FileUploadPath");
            //產生識別碼(檔案資料夾依據)
            string uniqueId = Common.CreateUniqueId();
            DapperBase db = new(_myConnectionConfig.PJW);
            try {
                
                using (IDbConnection connection = db.OpenConnection()) {
                    //先insert db
                    DynamicParameters parameters = new();
                    int? workLogNo;
                    string strSQL = "INSERT INTO work_log " +
                        "(work_log_uniqueId, work_log_subject, work_log_contents, work_log_sort, work_log_adduser_id, work_log_adduser_name, work_log_priority_status) " +
                        "OUTPUT Inserted.work_log_no " +
                        "VALUES(@UniqueId, @Subject, @Content, @SortNo, @UserId, @UserName, @Priority)";
                    parameters = new DynamicParameters();
                    parameters.Add("UniqueId", uniqueId);
                    parameters.Add("Subject", addWorkLogSubject);
                    parameters.Add("Content", addWorkLogContent);
                    parameters.Add("SortNo", addSortComb);
                    parameters.Add("UserId", UserToken.Indx);
                    parameters.Add("UserName", UserToken.Code);
                    parameters.Add("Priority", addWorkLogPriority);
                    workLogNo = (int)await connection.ExecuteScalarAsync(strSQL, parameters);

                    //再存檔案
                    //檢查目錄是否存在
                    string fileSavePath = $@"{fileUploadPath}\{DateTime.Now:yyyyMMdd}\{uniqueId}";

                    if (!Directory.Exists(fileSavePath)) {
                        Directory.CreateDirectory(fileSavePath);
                    }

                    foreach (var file in addWorkLogFile) {
                        if (file.Length > 0) {
                            var path = $@"{fileSavePath}\{file.FileName}";
                            using (var stream = new FileStream(path, FileMode.Create)) {
                                await file.CopyToAsync(stream);
                                //檔案資訊也要存到資料庫                                
                                strSQL = "INSERT INTO work_log_file " +
                                    "(work_log_no, work_log_uniqueId, work_log_file_adduser_id, work_log_file_adduser_name, work_log_file_original, work_log_file_name, work_log_file_full) " +
                                    "VALUES(@WorkLogNo, @UniqueId, @UserId, @UserName, @FileOriginal, @FileName, @FileFullName)";
                                parameters = new DynamicParameters();
                                parameters.Add("WorkLogNo", workLogNo);
                                parameters.Add("UniqueId", uniqueId);
                                parameters.Add("UserId", UserToken.Indx);
                                parameters.Add("UserName", UserToken.Code);
                                parameters.Add("FileOriginal", file.FileName);
                                parameters.Add("FileName", file.FileName);
                                parameters.Add("FileFullName", path);
                                await connection.ExecuteScalarAsync(strSQL, parameters);
                            }
                        }
                    }
                }

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            } catch (Exception ex) {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessage;
                result.Data = ex.Message;
            }
            return Json(result);
        }
        /// <summary>
        /// 修改工作任務
        /// </summary>
        /// <param name="editWorkLogNo"></param>
        /// <param name="editSortComb"></param>
        /// <param name="editWorkLogSubject"></param>
        /// <param name="editWorkLogContent"></param>
        /// <param name="editWorkLogPriority"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> WorkLogEdit(string editWorkLogNo, string editSortComb, string editWorkLogSubject, string editWorkLogContent, string editWorkLogPriority)
        {
            var result = new ResultJsonInfo<dynamic>();
            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                string strSQL = @"UPDATE work_log SET " +
                    "work_log_sort = @editSortComb, " + 
                    "work_log_subject = @editWorkLogSubject, " +
                    "work_log_contents = @editWorkLogContent, " +
                    "work_log_priority_status = @editWorkLogPriority " +
                    "WHERE work_log_no = @editWorkLogNo ";
                DynamicParameters parameters = new();
                parameters.Add("editSortComb", editSortComb);
                parameters.Add("editWorkLogSubject", editWorkLogSubject);
                parameters.Add("editWorkLogContent", editWorkLogContent);
                parameters.Add("editWorkLogPriority", editWorkLogPriority);
                parameters.Add("editWorkLogNo", editWorkLogNo);

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

        /// <summary>
        /// 刪除任務
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> WorkLogDelete(string workLogNo)
        {
            var result = new ResultJsonNoDataInfo();
            
            if (string.IsNullOrEmpty(workLogNo)) {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessage;
                return Json(result);
            }

            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                                
                string strSQL = "SELECT * FROM work_log WHERE work_log_no = @workLogNo";
                DynamicParameters parameters = new();
                parameters.Add("workLogNo", workLogNo);
                List<dynamic> workLogData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
                foreach(dynamic wk in workLogData) {
                    
                    //1.先把實體檔案刪除
                    strSQL = "SELECT * FROM work_log_file WHERE work_log_no = @workLogNo";
                    parameters = new DynamicParameters();
                    parameters.Add("workLogNo", workLogNo);
                    List<dynamic> workLogFileData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
                    foreach (dynamic file in workLogFileData) {
                        FileInfo fi = new(file.work_log_file_full);
                        if (fi.Exists) fi.Delete();
                        DirectoryInfo currFolder = fi.Directory;
                        if (currFolder.GetFiles().Length == 0) { //判斷已刪除之後檔案的資料夾是否為空
                            currFolder.Delete(); //刪除空資料夾
                        }
                    }

                    //2.再把檔案資料庫刪除
                    strSQL = $"DELETE work_log_file WHERE work_log_no = @workLogNo";
                    parameters = new DynamicParameters();
                    parameters.Add("workLogNo", workLogNo);
                    await connection.ExecuteAsync(strSQL, parameters);

                    //3.最後再把工作任務資料庫刪除
                    strSQL = $"DELETE work_log WHERE work_log_no = @workLogNo";
                    parameters = new DynamicParameters();
                    parameters.Add("workLogNo", workLogNo);
                    int effect = await connection.ExecuteAsync(strSQL, parameters);
                    if (effect == 0) {
                        result.Status = ResultConfig.Fail;
                        result.Info = ResultConfig.FailMessage;
                    } else {
                        result.Status = ResultConfig.Ok;
                        result.Info = ResultConfig.SuccessfulMessage;
                    }
                }
                
            }

            return Json(result);
        }

        /// <summary>
        /// 執行回覆
        /// </summary>
        /// <param name="workLogReplyFile"></param>
        /// <param name="workLogNo"></param>
        /// <param name="workLogReplyNote"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> WorkLogReply(List<IFormFile> workLogReplyFile,string workLogNo, string workLogReplyNote)
        {
            var result = new ResultJsonInfo<dynamic>();
            //取得預設的檔案上傳路徑
            string fileUploadPath = ConfigHelper.GetConfig<string>("FileUploadPath");
            //產生識別碼(檔案資料夾依據)
            string uniqueId = Common.CreateUniqueId();
            DapperBase db = new(_myConnectionConfig.PJW);
            try {

                using (IDbConnection connection = db.OpenConnection()) {
                    //先insert db
                    DynamicParameters parameters = new();
                    int? workLogReplyNo;
                    string strSQL = "INSERT INTO work_log_reply " +
                        "(work_log_no, work_log_reply_uniqueId, work_log_reply_note, work_log_reply_adduser_id, work_log_reply_adduser_name) " +
                        "OUTPUT Inserted.work_log_reply_no " +
                        "VALUES(@WorkLogNo, @UniqueId, @Note, @UserId, @UserName)";
                    parameters = new DynamicParameters();
                    parameters.Add("WorkLogNo", workLogNo); //任務編號
                    parameters.Add("UniqueId", uniqueId);
                    parameters.Add("Note", workLogReplyNote);
                    parameters.Add("UserId", UserToken.Indx);
                    parameters.Add("UserName", UserToken.Code);
                    workLogReplyNo = (int)await connection.ExecuteScalarAsync(strSQL, parameters);

                    //再存檔案
                    //檢查目錄是否存在
                    string fileSavePath = $@"{fileUploadPath}\{DateTime.Now:yyyyMMdd}\Reply\{uniqueId}";

                    if (!Directory.Exists(fileSavePath)) {
                        Directory.CreateDirectory(fileSavePath);
                    }

                    foreach (var file in workLogReplyFile) {
                        if (file.Length > 0) {
                            var path = $@"{fileSavePath}\{file.FileName}";
                            using (var stream = new FileStream(path, FileMode.Create)) {
                                await file.CopyToAsync(stream);
                                //檔案資訊也要存到資料庫                                
                                strSQL = "INSERT INTO work_log_reply_file " +
                                    "(work_log_reply_no, work_log_reply_uniqueId, work_log_reply_file_adduser_id, work_log_reply_file_adduser_name, work_log_reply_file_original, work_log_reply_file_name, work_log_reply_file_full) " +
                                    "VALUES(@WorkLogReplyNo, @UniqueId, @UserId, @UserName, @FileOriginal, @FileName, @FileFullName)";
                                parameters = new DynamicParameters();
                                parameters.Add("WorkLogReplyNo", workLogReplyNo);
                                parameters.Add("UniqueId", uniqueId);
                                parameters.Add("UserId", UserToken.Indx);
                                parameters.Add("UserName", UserToken.Code);
                                parameters.Add("FileOriginal", file.FileName);
                                parameters.Add("FileName", file.FileName);
                                parameters.Add("FileFullName", path);
                                await connection.ExecuteScalarAsync(strSQL, parameters);
                            }
                        }
                    }
                }

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            } catch (Exception ex) {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessage;
                result.Data = ex.Message;
            }
            return Json(result);

        }

        /// <summary>
        /// 取得任務附件檔
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetWorkLogFile(string workLogNo)
        {
            var result = new ResultJsonInfo<List<dynamic>>();
            List<dynamic> workLogFileData = new List<dynamic>();
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = pjwDB.OpenConnection()) {
                string strSQL = "SELECT * " +
                                "FROM work_log_file " +
                                "WHERE work_log_no=@workLogNo ";
                DynamicParameters parameters = new();
                parameters.Add("workLogNo", workLogNo);
                workLogFileData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
            }


            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Data = workLogFileData;
            return Json(result);
        }
                
        /// <summary>
        /// 下載工作任務附件
        /// </summary>
        /// <param name="workLogFileNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string workLogFileNo)
        {
            
            DapperBase db = new(_myConnectionConfig.PJW);
            try {
                using (IDbConnection connection = db.OpenConnection()) {
                    string strSQL = "SELECT * FROM work_log_file WHERE work_log_file_no = @fileNo";
                    DynamicParameters parameters = new();
                    parameters.Add("fileNo", workLogFileNo);
                    List<dynamic> workLogFileData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
                    foreach (dynamic file in workLogFileData) {
                        FileInfo fi = new(file.work_log_file_full);
                        if (fi.Exists) {
                            FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                            return File(stream, "application/octet-stream", file.work_log_file_name); //MME 格式 可上網查 此為通用設定
                            
                        } else {

                            return Content("查無此檔案");
                        }
                    }
                }
            } catch (Exception) {
                return Content("查無此檔案");
            }

            return Content("查無此檔案");
            
        }

        /// <summary>
        /// 下載回覆任務附件
        /// </summary>
        /// <param name="workLogReplyFileNo"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadReplyFile(string workLogReplyFileNo)
        {

            DapperBase db = new(_myConnectionConfig.PJW);
            try {
                using (IDbConnection connection = db.OpenConnection()) {
                    string strSQL = "SELECT * FROM work_log_reply_file WHERE work_log_reply_file_no = @fileNo";
                    DynamicParameters parameters = new();
                    parameters.Add("fileNo", workLogReplyFileNo);
                    List<dynamic> workLogReplyFileData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
                    foreach (dynamic file in workLogReplyFileData) {
                        FileInfo fi = new(file.work_log_reply_file_full);
                        if (fi.Exists) {
                            FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                            return File(stream, "application/octet-stream", file.work_log_reply_file_name); //MME 格式 可上網查 此為通用設定

                        } else {

                            return Content("查無此檔案");
                        }
                    }
                }
            } catch (Exception) {
                return Content("查無此檔案");
            }

            return Content("查無此檔案");

        }
        /// <summary>
        /// 任務附件刪除
        /// </summary>
        /// <param name="workLogFileNo"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> WorkLogFileDelete(string workLogFileNo)
        {
            var result = new ResultJsonNoDataInfo();
            
            if (string.IsNullOrEmpty(workLogFileNo)) {
                result.Status = ResultConfig.Fail;
                result.Info = ResultConfig.FailMessage;
                return Json(result);
            }

            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                
                //先把實體檔案刪除
                string strSQL = "SELECT * FROM work_log_file WHERE work_log_file_no = @fileNo";
                DynamicParameters parameters = new();
                parameters.Add("fileNo", workLogFileNo);
                List<dynamic> workLogFileData = (List<dynamic>)await connection.QueryAsync(strSQL, parameters);
                foreach (dynamic file in workLogFileData) {
                    FileInfo fi = new(file.work_log_file_full);
                    if (fi.Exists) fi.Delete();
                    DirectoryInfo currFolder = fi.Directory;
                    if (currFolder.GetFiles().Length == 0) { //判斷已刪除之後檔案的資料夾是否為空
                        currFolder.Delete(); //刪除空資料夾
                    }
                }

                //再把資料庫刪除
                strSQL = $"DELETE work_log_file WHERE work_log_file_no = @fileNo";
                parameters = new DynamicParameters();
                parameters.Add("fileNo", workLogFileNo);

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

        /// <summary>
        /// 送審 / 執行的狀態更新
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <param name="workLogStatus"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> WorkLogFlow(WorkLogViewModel workLogModel)
        {
            var result = new ResultJsonInfo<List<dynamic>>();
            string strSQL;
            //workLogModel 是前端Post過來的
            //workLogVm適從資料庫去撈的
            WorkLogViewModel workLogVm = WorkLogVm(workLogModel.WorkLogNo.ToString());

            DapperBase db = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = db.OpenConnection()) {
                DynamicParameters parameters = new();
                parameters.Add("workLogNo", workLogModel.WorkLogNo);

                List<dynamic> myBoss = new();
                List<dynamic> deptBoss = new();

                List<dynamic> listData = new();

                string mailTo = "";
                string deptName = "";
                SendEmail mail = new();
                MimeMessage sendMessage = new();

                switch (workLogModel.WorkLogStatus) {
                    case 0: //暫存或被退回
                        strSQL = "UPDATE work_log SET work_log_status=@workLogStatus, work_log_type=0 WHERE work_log_no=@workLogNo";
                        parameters.Add("workLogNo", workLogModel.WorkLogNo);
                        parameters.Add("workLogStatus", workLogModel.WorkLogStatus);
                        await connection.ExecuteAsync(strSQL, parameters);
                        result.Status = ResultConfig.Ok;
                        result.Info = ResultConfig.SuccessfulMessage;
                        //寄EMAIL給需求人員 
                        listData = Common.GetIntraUser(workLogVm.WorkLogAddUserId.ToString());
                        foreach (dynamic d in listData) {
                            mailTo = d.email;
                            deptName = d.DeptName;
                        }
                        mailTo = ""; //TEST
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務退回通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>{workLogVm.WorkLogAddUserName}您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>部門主管已對此工作任務做退回動作。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +                                    
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        //如果是執行單位主管退回，另外通知需求單位主管
                        if (!string.IsNullOrEmpty(workLogVm.WorkLogAssignDeptId.ToString())) {
                            myBoss = Common.GetMyBoss(workLogVm.WorkLogAddUserId.ToString());
                            mailTo = myBoss[0].直屬主管Email;
                            mailTo = ""; //TEST
                            deptName = myBoss[0].部門中文名稱;
                            sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務退回通知";
                            sendMessage.Body = new TextPart("html")
                            {
                                Text = $@"<h3>長官您好：</h3>" +
                                        $@"<h3>請登入專案任務管理系統<br/>您所指派的工作任務已被執行單位主管做退回動作。</h3>" +
                                        $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                        $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                        $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +                                        
                                        $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                        $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                            };
                            await mail.SendAsync(sendMessage, mailTo);
                        }

                        break;
                    case 1: //需求單位主管審核
                        strSQL = "UPDATE work_log SET work_log_status=@workLogStatus, work_log_type=1 WHERE work_log_no=@workLogNo";
                        parameters.Add("workLogNo", workLogModel.WorkLogNo);
                        parameters.Add("workLogStatus", workLogModel.WorkLogStatus);
                        await connection.ExecuteAsync(strSQL, parameters);
                        result.Status = ResultConfig.Ok;
                        result.Info = ResultConfig.SuccessfulMessage;
                        //寄EMAIL給需求單位主管 
                        myBoss = Common.GetMyBoss(UserToken.Indx);
                        mailTo = myBoss[0].直屬主管Email;
                        mailTo = ""; //TEST
                        deptName = myBoss[0].部門中文名稱;
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務需求審核通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>長官您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>請審核由 {myBoss[0].中文姓名}({myBoss[0].英文姓名}) 送過來的工作任務。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"
                        };
                        await mail.SendAsync(sendMessage, mailTo);
                        break;
                    case 2: //執行單位主管審核
                        strSQL = "UPDATE work_log SET work_log_status=@workLogStatus, work_log_assign_dept_id=@workAssignDeptId, work_log_type=1 WHERE work_log_no=@workLogNo";
                        parameters.Add("workLogNo", workLogModel.WorkLogNo);
                        parameters.Add("workAssignDeptId", workLogModel.WorkLogAssignDeptId);
                        parameters.Add("workLogStatus", workLogModel.WorkLogStatus);
                        await connection.ExecuteAsync(strSQL, parameters);
                        result.Status = ResultConfig.Ok;
                        result.Info = ResultConfig.SuccessfulMessage;
                        //寄EMAIL給執行單位主管主管 
                        deptBoss = Common.GetDeptBoss(workLogModel.WorkLogAssignDeptId.ToString());
                        foreach (dynamic boss in deptBoss) {
                            mailTo = boss.Email;
                            mailTo = ""; //TEST
                            deptName = boss.DeptName;
                            sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務執行審核通知";
                            sendMessage.Body = new TextPart("html")
                            {
                                Text = $@"<h3>長官您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>請審核由需求單位 {workLogVm.WorkLogDeptName} 送過來的工作任務。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"
                            };
                            await mail.SendAsync(sendMessage, mailTo);
                        }
                        break;
                    case 3: //人員執行中
                        strSQL = @"UPDATE work_log SET " +
                            "  work_log_assign_dept_id=@workLogAssignDeptId, " +
                            "  work_log_assign_id=@workLogAssignId, " +
                            "  work_log_assign_name=@workLogAssignName, " +
                            "  work_log_startdate=@workLogStartDate, " +
                            "  work_log_starttime=@workLogStartTime, " +
                            "  work_log_enddate=@workLogEndDate, " +
                            "  work_log_endtime=@workLogEndTime, " +
                            "  work_log_status=@workLogStatus, " +
                            "  work_log_type=2 " +
                            "WHERE work_log_no=@workLogNo";

                        parameters.Add("workLogNo", workLogModel.WorkLogNo);
                        parameters.Add("workLogAssignDeptId", workLogModel.WorkLogAssignDeptId);
                        parameters.Add("workLogAssignId", workLogModel.WorkLogAssignId);
                        parameters.Add("workLogAssignName", workLogModel.WorkLogAssignName);
                        parameters.Add("workLogStartDate", workLogModel.WorkLogStartDate);
                        parameters.Add("workLogStartTime", workLogModel.WorkLogStartTime);
                        parameters.Add("workLogEndDate", workLogModel.WorkLogEndDate);
                        parameters.Add("workLogEndTime", workLogModel.WorkLogEndTime);
                        parameters.Add("workLogStatus", workLogModel.WorkLogStatus);
                        await connection.ExecuteAsync(strSQL, parameters);
                        result.Status = ResultConfig.Ok;
                        result.Info = ResultConfig.SuccessfulMessage;

                        //寄EMAIL給指派人員
                        //取得被指派人員的Email
                        strSQL = $"SELECT A.[email], B.nameC AS DeptName " +
                            "FROM Intranet.dbo.administration A " +
                            "LEFT JOIN Intranet.dbo.department B " +
                            "   ON A.department_id = B.indx " +
                            $"WHERE A.indx={workLogModel.WorkLogAssignId}";
                        listData = (List<dynamic>)await connection.QueryAsync(strSQL);
                        foreach(dynamic d in listData) { 
                            mailTo = d.email;
                            deptName = d.DeptName;
                        }

                        mailTo = ""; //TEST
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務指派通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>{workLogModel.WorkLogAssignName}您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>由 {UserToken.Code} 指派給您的工作任務。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"
                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        break;
                    case 4: //執行回覆
                        strSQL = "UPDATE work_log SET work_log_status=@workLogStatus, work_log_type=2 WHERE work_log_no=@workLogNo";
                        parameters.Add("workLogNo", workLogModel.WorkLogNo);
                        parameters.Add("workLogStatus", workLogModel.WorkLogStatus);
                        await connection.ExecuteAsync(strSQL, parameters);
                        result.Status = ResultConfig.Ok;
                        result.Info = ResultConfig.SuccessfulMessage;

                        //寄EMAIL給該任務所有相關人員
                        //需求人員、需求單位主管、執行人員、執行單位主管、任務分享人員

                        //需求人員 *********************************************************************************                        
                        listData = Common.GetIntraUser(workLogVm.WorkLogAddUserId.ToString());
                        foreach (dynamic d in listData) {
                            mailTo = d.email;
                            deptName = d.DeptName;
                        }
                        mailTo = ""; //TEST
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務回覆通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>{workLogVm.WorkLogAddUserName}您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>執行人員已對工作任務做回覆說明。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        //需求單位主管 *************************************************************************
                        myBoss = Common.GetMyBoss(workLogVm.WorkLogAddUserId.ToString());
                        mailTo = myBoss[0].直屬主管Email;
                        mailTo = ""; //TEST
                        deptName = myBoss[0].部門中文名稱;
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務回覆通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>長官您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>執行人員已對工作任務做回覆說明。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        //執行人員*************************************************************************                        
                        listData = Common.GetIntraUser(workLogVm.WorkLogAssignId.ToString());
                        foreach (dynamic d in listData) {
                            mailTo = d.email;
                            deptName = d.DeptName;
                        }
                        mailTo = ""; //TEST
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務回覆通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>{workLogVm.WorkLogAssignName}您好：</h3>" +
                                    $@"<h3>您已經對工作任務做回覆說明，會通知相關需求與執行單位的主管與人員。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        //執行單位主管 *************************************************************************
                        myBoss = Common.GetMyBoss(workLogVm.WorkLogAssignId.ToString());
                        mailTo = myBoss[0].直屬主管Email;
                        mailTo = ""; //TEST
                        deptName = myBoss[0].部門中文名稱;
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務回覆通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>長官您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>您所屬的執行人員已對工作任務做回覆說明。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        break;

                    case 5: //結案
                        strSQL = "UPDATE work_log SET work_log_status=@workLogStatus, work_log_score=@workLogScore, work_log_type=3 WHERE work_log_no=@workLogNo";
                        parameters.Add("workLogNo", workLogModel.WorkLogNo);
                        parameters.Add("workLogStatus", workLogModel.WorkLogStatus);
                        parameters.Add("workLogScore", workLogModel.WorkLogScore);
                        await connection.ExecuteAsync(strSQL, parameters);
                        result.Status = ResultConfig.Ok;
                        result.Info = ResultConfig.SuccessfulMessage;
                        //寄EMAIL給該任務所有相關人員
                        //需求人員、需求單位主管、執行人員、執行單位主管、任務分享人員

                        //需求人員 *********************************************************************************                        
                        listData = Common.GetIntraUser(workLogVm.WorkLogAddUserId.ToString());
                        foreach (dynamic d in listData) {
                            mailTo = d.email;
                            deptName = d.DeptName;
                        }
                        mailTo = ""; //TEST
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務結案通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>{workLogVm.WorkLogAddUserName}您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>您部門的主管已對工作任務做結案動作。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        //需求單位主管 *************************************************************************
                        myBoss = Common.GetMyBoss(workLogVm.WorkLogAddUserId.ToString());
                        mailTo = myBoss[0].直屬主管Email;
                        mailTo = ""; //TEST
                        deptName = myBoss[0].部門中文名稱;
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務結案通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>長官您好：</h3>" +
                                    $@"<h3>您已對工作任務做結案動作。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        //執行人員*************************************************************************                        
                        listData = Common.GetIntraUser(workLogVm.WorkLogAssignId.ToString());
                        foreach (dynamic d in listData) {
                            mailTo = d.email;
                            deptName = d.DeptName;
                        }
                        mailTo = ""; //TEST
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務結案通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>{workLogVm.WorkLogAssignName}您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>恭喜您！您的工作任務已被需求單位主管做結案動作。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        //執行單位主管 *************************************************************************
                        myBoss = Common.GetMyBoss(workLogVm.WorkLogAssignId.ToString());
                        mailTo = myBoss[0].直屬主管Email;
                        mailTo = ""; //TEST
                        deptName = myBoss[0].部門中文名稱;
                        sendMessage.Subject = $@"[{deptName}][{workLogModel.WorkLogNo}]工作任務結案通知";
                        sendMessage.Body = new TextPart("html")
                        {
                            Text = $@"<h3>長官您好：</h3>" +
                                    $@"<h3>請登入專案任務管理系統<br/>恭喜您！您的部門的執行工作任務已被需求單位主管做結案動作。</h3>" +
                                    $@"<h3>需求部門：{workLogVm.WorkLogDeptName}</h3>" +
                                    $@"<h3>需求人員：{workLogVm.WorkLogAddUserName}</h3>" +
                                    $@"<h3>執行部門：{workLogVm.WorkLogAssignDeptName}</h3>" +
                                    $@"<h3>執行人員：{workLogVm.WorkLogAssignName}</h3>" +
                                    $@"<h3>任務編號：{workLogModel.WorkLogNo}</h3>" +
                                    $@"<h3>任務名稱：{workLogVm.WorkLogSubject}</h3>"

                        };
                        await mail.SendAsync(sendMessage, mailTo);

                        break;
                    default:

                        break;
                }
            }

            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            return Json(result);
        }

        
        /// <summary>
        /// 取得WorkLog相關資料，傳回模型
        /// </summary>
        /// <param name="workLogNo"></param>
        /// <returns></returns>
        public WorkLogViewModel WorkLogVm(string workLogNo)
        {
            List<dynamic> workLogData = new List<dynamic>();
            WorkLogViewModel workLogView = new();
            DapperBase pjwDB = new(_myConnectionConfig.PJW);
            using (IDbConnection connection = pjwDB.OpenConnection()) {
                string strSQL = "SELECT " +
                    "   A.*, " +
                    "   B.work_log_sort_no AS sortNo, " +
                    "   B.work_log_sort_name AS sortName," +
                    "   C.indx AS deptNo, C.nameC AS deptName, " +
                    "   D.indx AS assignDeptId, D.nameC AS assignDeptName " + 
                "FROM work_log A " +
                "LEFT JOIN work_log_sort B " +
                "	ON A.work_log_sort = B.work_log_sort_no " +
                "LEFT JOIN Intranet.dbo.department C " +
                "	ON B.work_log_sort_dept_no = C.indx " +
                "LEFT JOIN Intranet.dbo.department D " +
                "	ON A.work_log_assign_dept_id = D.indx " +
                "WHERE A.work_log_no=@workLogNo ";
                DynamicParameters parameters = new();
                parameters.Add("workLogNo", workLogNo);
                workLogData = connection.Query(strSQL, parameters).ToList();
                IEnumerable<dynamic> rec = workLogData
                .Select(g => new WorkLogViewModel
                {
                    WorkLogNo = g.work_log_no,
                    WorkLogSubject = g.work_log_subject,
                    WorkLogContent = g.work_log_contents,
                    WorkLogSortNo = g.sortNo,
                    WorkLogSortName = g.sortName,
                    WorkLogDeptName = g.deptName,
                    WorkLogPriority = g.work_log_priority_status,
                    WorkLogStatus = g.work_log_status,
                    WorkLogType = g.work_log_type,
                    WorkLogPriorityName = Common.GetWorkLogPriority(g.work_log_priority_status),
                    WorkLogStatusName = Common.GetWorkLogStatus(g.work_log_status),
                    WorkLogTypeName = Common.GetWorkLogType(g.work_log_type),
                    WorkLogAddDate = g.work_log_adddate,
                    WorkLogAddUserId = g.work_log_adduser_id,
                    WorkLogAddUserName = g.work_log_adduser_name,
                    WorkLogAssignDeptId = g.work_log_assign_dept_id,
                    WorkLogAssignDeptName = g.assignDeptName,
                    WorkLogAssignId = g.work_log_assign_id,
                    WorkLogAssignName = g.work_log_assign_name

                });
                foreach (WorkLogViewModel vm in rec) {
                    workLogView.WorkLogNo = vm.WorkLogNo;
                    workLogView.WorkLogSubject = vm.WorkLogSubject;
                    workLogView.WorkLogContent = vm.WorkLogContent;
                    workLogView.WorkLogSortNo = vm.WorkLogSortNo;
                    workLogView.WorkLogSortName = vm.WorkLogSortName;
                    workLogView.WorkLogDeptName = vm.WorkLogDeptName;
                    workLogView.WorkLogPriority = vm.WorkLogPriority;
                    workLogView.WorkLogStatus = vm.WorkLogStatus;
                    workLogView.WorkLogType = vm.WorkLogType;
                    workLogView.WorkLogPriorityName = vm.WorkLogPriorityName;
                    workLogView.WorkLogTypeName = vm.WorkLogTypeName;
                    workLogView.WorkLogAddDate = vm.WorkLogAddDate;
                    workLogView.WorkLogAddUserId = vm.WorkLogAddUserId;
                    workLogView.WorkLogAddUserName = vm.WorkLogAddUserName;
                    workLogView.WorkLogAssignDeptId = vm.WorkLogAssignDeptId;
                    workLogView.WorkLogAssignDeptName = vm.WorkLogAssignDeptName;
                    workLogView.WorkLogAssignId = vm.WorkLogAssignId;
                    workLogView.WorkLogAssignName = vm.WorkLogAssignName;
                }

                return workLogView;
            }

        }
    }
}
