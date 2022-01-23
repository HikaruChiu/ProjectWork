using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PJW.Web.Filter;
using ViewModels.Reuqest;
using ViewModels.Result;
using Dapper;
using System.Data;
using Configuration;
using Repository;

namespace PJW.Web.Controllers
{    
    [API("員工資訊列表")]
    [AuthorizeFilter]
    public class IntraUserController : PJW.Web.Admin.Controllers.BaseController
    {
        //注入
        public readonly MyConnectionConfig _myConnectionConfig;
        public IntraUserController(MyConnectionConfig myConnectionConfig)
        {
            _myConnectionConfig = myConnectionConfig;
        }

        [API("檢視員工列表")]
        public IActionResult Index()
        {
            return View();
        }

        [API("檢視員工詳細資訊")]
        [HttpPost]

        public IActionResult Detail()
        {
            return View();
        }

        /// <summary>
        /// 獲取所有的使用者
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [API("分頁獲取所有的員工資訊")]
        [HttpPost]        
        public async Task<JsonResult> GetIntraUserList([FromForm] IntraUserVm model)
        {
            //這是最後要return 前段的 JsonResult
            SearchResult<List<dynamic>> result = new();

            //實作DapperBase , 設定DB連線
            DapperBase db = new(_myConnectionConfig.IntranetDB);

            //"SELECT * FROM administration WHERE indx > @indx";
            //建立SQL語句
            string strSQL = @"SELECT A.*, B.nameC AS deptName FROM administration A 
                LEFT JOIN department B ON A.department_id = B.indx 
                /**where**/ ";
            SqlBuilder builder = new();
            var selector = builder.AddTemplate(strSQL); // /**where**/ <== 這是WHERE識別標籤
            //WHERE 條件
            //builder.Where("indx >= @indx", new { indx = 500 });
            if (!string.IsNullOrEmpty(model.UserId))
            {
                builder.Where("id like @id", new { id = $"%{model.UserId}%" });                
            }
            if (!string.IsNullOrEmpty(model.UserEmail))
            {
                builder.Where("email like @email", new { email = $"%{model.UserEmail}%" });
            }
            if (!string.IsNullOrEmpty(model.UserName))
            {
                builder.Where("name like @name", new { name = $"%{model.UserName}%" });
            }
            
            //設定排序
            List<SortDescriptor> sortings = new();
            model.OrderBy = string.IsNullOrWhiteSpace(model.OrderBy) ? "indx" : model.OrderBy;
            if (!string.IsNullOrEmpty(model.OrderBy))
            {                
                sortings.Add(new SortDescriptor { 
                    Direction = !string.IsNullOrEmpty(model.OrderSequence) & model.OrderSequence.ToLower() == "desc" ? SortDescriptor.SortingDirection.Descending : SortDescriptor.SortingDirection.Ascending, 
                    Field = model.OrderBy 
                });
            }

            //var sql = selector.RawSql;
            //selector.RawSql 是把上面設定後的組成一個SQL語法
            //帶入分頁資訊
            //開始查詢資料，Tuple是一個很好用的東西 https://igouist.github.io/post/2020/09/csharp-trulp/
            Tuple<IEnumerable<dynamic>, int> data = await db.FindWithOffsetFetch(selector.RawSql, selector.Parameters, model.PageIndex, model.PageSize, sortings);

            //為了顯示密碼做轉換
            //var rec = (from x in data.Item1
            //             select new 
            //             {
            //                 indx = x.indx,
            //                 id = x.id.
            //                 name = x.name,
            //                 email = x.email,
            //                 pwdMD5 = x.pwdMD5,
            //                 realPwd = x.pwdMD5
            //             }).ToList<dynamic>();


            result.Status = ResultConfig.Ok;
            result.Info = ResultConfig.SuccessfulMessage;
            result.Total = data.Item2;
            result.Rows = data.Item1.ToList();

            return Json(result);
        }


    }
}
