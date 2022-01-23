using AntData.ORM.Data;
using AntData.ORM.Mapping;
using Autofac.Annotation;
using DbModel;
using Infrastructure.CodeGen;
using Infrastructure.Logging;
using Newtonsoft.Json;
using Repository.Interface;
using ServicesModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using ViewModels.Reuqest;

namespace Repository
{
    /// <summary>
    /// 公共處理
    /// </summary>
    [Component]
    public class CommonRespository : BaseRepository, ICommonRespository
    {

        /// <summary>
        /// 給codegen使用
        /// </summary>
        private static List<CodeGenTable> _dbTableCache = null;


        #region SQL

        /// <summary>
        /// 執行sql語句返回DataTable
        /// 支援多數據庫配置
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable SelectSqlExcute(string db, string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return new DataTable();
            }
            db = db.Split('[')[0];
            return this.EmptyDB(db).QueryTable(sql);
        }

        /// <summary>
        /// 執行sql語句返回受影響條數
        /// 支援多數據庫配置
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public Tuple<int, string> SQLExcute(string db, string sql)
        {
            int result = -1;
            if (string.IsNullOrEmpty(sql))
            {
                return new Tuple<int, string>(-1, Tip.BadRequest);
            }

            db = db.Split('[')[0];
            try
            {
                this.EmptyDB(db).UseTransaction(con =>
                {
                    result = con.Execute(sql);
                    return true;
                });

            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(-1, ex.Message);
            }
            if (result == -1)
            {
                return new Tuple<int, string>(result, "請使用Select按鈕查詢！");
            }
            return new Tuple<int, string>(result, string.Empty);
        }

        #endregion

        /// <summary>
        /// 獲取所有的Table和Columns
        /// 從db裡面獲取最新的數據 要不要快取?
        /// </summary>
        /// <returns></returns>
        public string GetDbTablesAndColumns(string dbName)
        {
            var arr = dbName.Split('[');
            var db = arr[0];
            var provider = arr[1].Replace("]", "");
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            //mysql 和 sqlserver 不一樣
            List<string> tables = provider.Equals("Mysql") ? this.EmptyDB(db).Query<string>("show tables").ToList() :
                this.EmptyDB(db).Query<string>("select name from sys.Tables where type ='U'").ToList();
            foreach (var table in tables)
            {
                var columns = getAllFields(db, provider, table);
                result.Add(table, columns);
            }
            var _dbTableAndColumnsCache = JsonConvert.SerializeObject(result);
            return _dbTableAndColumnsCache;
        }

        /// <summary>
        /// 獲取所有的表
        /// </summary>
        /// <returns></returns>
        public List<CodeGenTable> GetDbTables()
        {
            if (_dbTableCache != null)
            {
                return _dbTableCache;
            }
            _dbTableCache = this.GetDbTabless();
            //可能會配置有多個db
            return _dbTableCache;
        }

        /// <summary>
        /// 獲取表下面所有的欄位
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<CodeGenField> GetDbTablesColumns(string dbName, string tableName)
        {
            var key = (string.IsNullOrEmpty(dbName) ? "" : dbName + ".") + tableName;
            if (_dbColumnsCache.TryGetValue(key, out var cache)) return cache;
            cache = this.GetDbModels(dbName, tableName);
            _dbColumnsCache.TryAdd(key, cache);
            return cache;
        }

        /// <summary>
        /// 自動產生程式碼
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public byte[] CodeGen(CodeGenVm model)
        {
            return GeneratorCodeHelper.CodeGenerator(model.TableName, model.Columns);
        }

        /// <summary>
        /// 獲取appsettings.json裡面配置的所有字串
        /// </summary>
        /// <returns></returns>
        public List<string> GetDbs()
        {
            var allData = DbModel.DbContext.GetAllDbMappingList().Select(r => r.Item2 + "[" + (r.Item1.Contains("Mysql") ? "Mysql" : "Sqlserver") + "]");//獲取數據
            return allData.ToList();
        }


        /// <summary>
        /// 獲取所有的DBTable
        /// </summary>
        private List<CodeGenTable> GetDbTabless()
        {
            var result = new List<CodeGenTable>();
            try
            {

                var modelAss = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => (assembly.GetName().Name.Equals("DbModel")));
                if (modelAss == null)
                {
                    throw new ArgumentException("assemblys");
                }
                var types = modelAss.GetExportedTypes();
                var targetClass = (from t in types
                                   where t.BaseType == typeof(LinqToDBEntity) &&
                                       !t.IsAbstract &&
                                       !t.IsInterface
                                   select t).ToArray();


                foreach (var tt in targetClass)
                {
                    var tart = tt.GetCustomAttribute<TableAttribute>();
                    if (tart == null)
                    {
                        continue;
                    }

                    var comment = tart.Comment;
                    if (string.IsNullOrEmpty(comment))
                    {
                        comment = string.Empty;
                        LogHelper.Debug("GetDbTabless", tart.Name + "表的Comment為空!!");
                    }
                    result.Add(new CodeGenTable
                    {
                        DbName = tart.Db,
                        Name = tt.Name,
                        TableName = tart.Name,
                        Comment = comment.Replace(",", "").Replace("→", "")
                    });
                }
                return result.OrderBy(r => r.Name).ToList();
            }
            catch (Exception ex)
            {

                LogHelper.Warn("GetDbTabless", "可能有表的Comment為空導致", ex);
            }
            return result;
        }


        /// <summary>
        /// 獲取表裡面所有的欄位
        /// </summary>
        /// <param name="db"></param>
        /// <param name="provider"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<string> getAllFields(string db, string provider, string tableName)
        {
            if (provider.Equals("Mysql"))
            {
                var columns = this.EmptyDB(db).Query<string>(" SHOW COLUMNS FROM " + tableName).ToList();
                return columns;
            }
            else
            {
                var sql = @"select COLUMN_NAME
                from INFORMATION_SCHEMA.COLUMNS
                    where TABLE_NAME = @Name ";

                return this.EmptyDB(db).Query<string>(sql, new { Name = tableName }).ToList();
            }
        }

    }
}
