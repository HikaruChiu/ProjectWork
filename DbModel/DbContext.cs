using AntData.ORM.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AntData.ORM.Linq;
using Configuration;
using Infrastructure.Logging;
using Infrastructure.Web;

namespace DbModel
{
    public class DbContext
    {
        /// <summary>
        /// 目前後臺系統採用的是什麼數據庫
        /// </summary>
        private static readonly string dbType;
        /// <summary>
        /// 目前後臺系統採用的數據庫對應的匹配名稱
        /// </summary>
        private static readonly string dbMappingName;

        private static readonly List<Tuple<string, string>> dbMappingNameList = new List<Tuple<string, string>>();
        static DbContext()
        {

            try
            {
                //讀取目前採用的是什麼數據庫
                dbMappingName = ConfigHelper.GetConfig<string>("DbType");
                if (string.IsNullOrEmpty(dbMappingName))
                {
                    throw new ArgumentException("DbType");
                }

                foreach (var databaseSetting in AntData.ORM.Common.Configuration.DBSettings.DatabaseSettings)
                {
                    foreach (var connectionStringItem in databaseSetting.ConnectionItemList)
                    {
                        dbMappingNameList.Add(new Tuple<string, string>(databaseSetting.Provider, connectionStringItem.Name));
                    }
                }


                //配置檔案配置的db資訊 可能有多個
                var dbProvider = AntData.ORM.Common.Configuration.DBSettings.DatabaseSettings.FirstOrDefault(r => r.Name.Equals(dbMappingName));

                //根據邏輯名稱來區分

                dbType = dbProvider?.Provider;
            }
            catch (Exception)
            {
                dbType = "SqlServer";
            }

        }
        public static DbContext<AntEntity> DB
        {
            get
            {
                DbContext<AntEntity> db;
                if (dbType.ToLower().Contains("mysql"))
                {
                    db = new MysqlDbContext<AntEntity>(dbMappingName);
                }
                else
                {
                    db = new SqlServerlDbContext<AntEntity>(dbMappingName);
                }

#if DEBUG
                db.IsEnableLogTrace = true;
                db.OnLogTrace = OnCustomerTraceConnection;
#endif
                return db;
            }
        }


        /// <summary>
        /// 根據db型別和db名稱獲取執行content
        /// </summary>
        public static DbContext<EmptyEntity> EmptyDb(string _dbMappingName)
        {
            var dbProvider = AntData.ORM.Common.Configuration.DBSettings.DatabaseSettings.FirstOrDefault(r => r.Name.Equals(_dbMappingName));
            var _dbType = dbProvider?.Provider;
            if (string.IsNullOrEmpty(_dbType)) return null;

            DbContext<EmptyEntity> db;
            if (_dbType.ToLower().Contains("mysql"))
            {
                db = new MysqlDbContext<EmptyEntity>(_dbMappingName);
            }
            else
            {
                db = new SqlServerlDbContext<EmptyEntity>(_dbMappingName);
            }

#if DEBUG
            db.IsEnableLogTrace = true;
            db.OnLogTrace = OnCustomerTraceConnection;
#endif
            return db;
        }

        /// <summary>
        /// 獲取配置檔案中配置的所有邏輯數據庫名稱
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<string, string>> GetAllDbMappingList() => dbMappingNameList;
        /// <summary>
        /// 記錄sql
        /// </summary>
        /// <param name="customerTraceInfo"></param>
        private static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            try
            {
                string sql = Environment.NewLine + "#####################################################" + Environment.NewLine + customerTraceInfo.SqlText;
                try
                {
                    sql = customerTraceInfo.CustomerParams.Aggregate(customerTraceInfo.SqlText,
                        (current, item) => current.Replace(item.Key, item.Value == null || item.Value.Value == null ? "" : item.Value.Value.ToString()));
                }
                catch (Exception)
                {
                    //ignore
                }
                sql += Environment.NewLine;
                foreach (var detail in customerTraceInfo.RunTimeList)
                {

                    var sencond = (int)detail.Duration.TotalSeconds;
                    var time = sencond + "秒";
                    if (sencond < 1)
                    {
                        time = detail.Duration.TotalMilliseconds + "豪秒";
                    }
                    sql += $"Server：{detail.Server},DB名稱：{detail.DbName}, 執行時間：{time}" + Environment.NewLine + "#####################################################" + Environment.NewLine;
                    LogHelper.Info("SQL", sql);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }
    }


    public class EmptyEntity : IEntity
    {


        private readonly DataConnection con;

        public DataConnection DbContext
        {
            get { return this.con; }
        }

        public EmptyEntity(DataConnection con)
        {
            this.con = con;
        }

        public IQueryable<T> Get<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }

}
