using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository;
using shortid;
using shortid.Configuration;

namespace PJW.Web
{
    /// <summary>
    /// 一些通用的參數
    /// </summary>
    public class Common
    {
        
        /// <summary>
        /// 取得一筆任務需求等級
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWorkLogPriority(int key)
        {            
            return Enum.GetName(typeof(WorkLogPriority), key);
        }
        /// <summary>
        /// 取得所有任務需求等級
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllWorkLogPriority()
        {
            return Enum.GetNames(typeof(WorkLogPriority)).ToList();
        }

        /// <summary>
        /// 取得一筆任務狀態
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWorkLogStatus(int key)
        {
            return Enum.GetName(typeof(WorkLogStatus), key);
        }
        /// <summary>
        /// 取得所有任務狀態
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllWorkLogStatus()
        {
            return Enum.GetNames(typeof(WorkLogStatus)).ToList();
        }

        /// <summary>
        /// 取得一筆任務屬性
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetWorkLogType(int key)
        {
            return Enum.GetName(typeof(WorkLogType), key);
        }
        /// <summary>
        /// 取得所有任務屬性
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllWorkLogType()
        {
            return Enum.GetNames(typeof(WorkLogType)).ToList();
        }


        /// <summary>
        /// 任務需求等級
        /// </summary>
        public enum WorkLogPriority
        {            
            一般 = 0,
            重要 = 1,
            急迫 = 2                
        }
        /// <summary>
        /// 任務狀態
        /// </summary>
        public enum WorkLogStatus
        {
            草稿 = 0,
            需求單位主管審核 = 1,
            執行單位主管審核 = 2,
            人員執行中 = 3,
            人員執行回覆 = 4,
            完成結案 = 5
        }
        /// <summary>
        /// 任務屬性
        /// </summary>
        public enum WorkLogType
        {
            暫存 = 0,
            送審中 = 1,
            執行 = 2,
            完成 = 3
        }

        /// <summary>
        /// 識別碼生成
        /// </summary>
        /// <returns></returns>
        public static string CreateUniqueId()
        {
            var options = new GenerationOptions
            {
                UseSpecialCharacters = false,
                Length = 10 //10位字
            };
            string uniqueId = ShortId.Generate(options);

            return uniqueId;
        }

        /// <summary>
        /// 取得我的直屬主管
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<dynamic> GetMyBoss(string userId)
        {

            string strSQL = @"select 
                  a.indx 員工代碼, 
                  a.name 中文姓名, 
                  a.first_name 英文姓名, 
                  a.phone 手機號碼, 
                  a.department_id 部門代碼, 
                  p.nameC 部門中文名稱, 
                  a.team 組別代碼, 
                  h.name 組別中文名稱, 
                  s.medium 直屬主管代碼, 
                  s.name 直屬主管中文名, 
                  s.phone 直屬主管手機號碼, 
                  s.email 直屬主管Email 
                from 
                  administration a 
                  left join department p on p.indx = a.department_id 
                  left join team_name h on h.team = a.team 
                  left join (
                    select 
                      a.indx, 
                      a.department_id, 
                      a.medium, 
                      a1.name, 
                      a1.first_name, 
                      a1.phone, 
                      a1.email 
                    from 
                      Attendance_rule a 
                      left join administration a1 on a.medium = a1.indx 
                    where 
                      a.deletemark = 'n'
                  ) s on s.indx = a.absRule 
                where 
                  1 = 1 
                  and a.deletemark = 'N'
                    and a.indx = " + userId;
            string connString = Configuration.ConfigHelper.GetConfig<string>("ConnectionStrings:IntranetDB");
            DapperBase db = new(connString);
            using IDbConnection connection = db.OpenConnection(); return connection.Query(strSQL).ToList();

        }

        /// <summary>
        /// 取得單位主管
        /// </summary>
        /// <param name="deptNo"></param>
        /// <returns></returns>
        public static List<dynamic> GetDeptBoss(string deptNo)
        {
            string strSQL = @"select 
                    distinct 
	                a.department_id AS deptNo, 
	                b.nameC AS DeptName,
	                c.[name] AS ChgName,
	                c.first_name AS EngName,
	                c.email AS Email
                from 
                    Attendance_rule a 
	                inner join department b on a.department_id = b.indx
                    inner join administration c on a.medium = c.indx 
                where 
                    a.deletemark = 'n'
                and a.department_id = " + deptNo;
            string connString = Configuration.ConfigHelper.GetConfig<string>("ConnectionStrings:IntranetDB");
            DapperBase db = new(connString);
            using IDbConnection connection = db.OpenConnection(); return connection.Query(strSQL).ToList();
        }

        /// <summary>
        /// 取得Intranet User的Mail等資訊
        /// </summary>
        /// <param name="indx"></param>
        /// <returns></returns>
        public static List<dynamic> GetIntraUser(string indx)
        {
            
            string strSQL = $"SELECT A.[email], A.[name] AS ChgName, A.first_name AS EngName, B.nameC AS DeptName " +
                            "FROM Intranet.dbo.administration A " +
                            "LEFT JOIN Intranet.dbo.department B " +
                            "   ON A.department_id = B.indx " +
                            $"WHERE A.indx={indx}";

            string connString = Configuration.ConfigHelper.GetConfig<string>("ConnectionStrings:IntranetDB");
            DapperBase db = new(connString);
            using IDbConnection connection = db.OpenConnection(); return connection.Query(strSQL).ToList();
            
        }
    }

}
