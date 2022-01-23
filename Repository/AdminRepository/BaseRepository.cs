﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using AntData.ORM;
using AntData.ORM.Data;
using AntData.ORM.Mapping;
using Configuration;
using DbModel;
using Infrastructure.Excel;
using Infrastructure.Logging;
using Infrastructure.StaticExt;
using Infrastructure.Web;
using Mapping;
using Repository.Interface;
using Newtonsoft.Json;
using ServicesModel;

namespace Repository
{
    /// <summary>
    /// 基礎倉庫
    /// </summary>
    public class BaseRepository : IRepository
    {
        /// <summary>
        /// db的表的欄位定義快取
        /// </summary>

        protected static readonly ConcurrentDictionary<string, List<CodeGenField>> _dbColumnsCache = new ConcurrentDictionary<string, List<CodeGenField>>();

        /// <summary>
        /// DB
        /// </summary>
        public DbContext<AntEntity> DB => DbModel.DbContext.DB;

        public DbContext<EmptyEntity> EmptyDB(string mappingName) => DbModel.DbContext.EmptyDb(mappingName);

        /// <summary>
        /// automapper
        /// </summary>
        /// <typeparam name="M1">原</typeparam>
        /// <typeparam name="M">目的對象</typeparam>
        /// <param name="source">原對像</param>
        /// <returns></returns>
        public M MapperTo<M1, M>(M1 source)
        {
            return Mapping.AutoMapper.MapperTo<M1, M>(source);
        }

        /// <summary>
        /// DB裡面所有的Entity
        /// </summary>
        public AntEntity Entitys => DB.Tables;

        public int Execute(string sql, params DataParameter[] parameters)
        {
            return this.DB.Execute(sql, parameters);
        }



        /// <summary>
        /// 根據一個DBmodel的對象型別獲取該表對應的欄位集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>

        protected (string, List<Tuple<string, string>>) GetTableFileds<T>(params string[] ignoreColumns) where T : LinqToDBEntity
        {
            var type = typeof(T);
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute == null || string.IsNullOrEmpty(tableAttribute.Name)) return ($"獲取表結構失敗,沒有找到繼承{nameof(LinqToDBEntity)}的{type.Name}類", null); ;

            List<CodeGenField> result = null;
            try
            {
                result = GetDbModels(tableAttribute.Db, type.Name);
            }
            catch (Exception e)
            {
                return ($"獲取表結構失敗,{e.Message}", null); ;
            }

            if (result == null || !result.Any())
            {
                return ("獲取表結構失敗", null);
            }

            //檢視表有沒有主鍵
            if (!result.Any(r => r.IsPrimary))
            {
                return ($"{type.Name}沒有定義主鍵", null);
            }

            if (ignoreColumns != null && ignoreColumns.Any())
            {
                var dic = ignoreColumns.Distinct().ToDictionary(r => r, y => y);
                result = result.Where(r => !dic.ContainsKey(r.FieldName)).ToList();
            }

            var re = new List<Tuple<string, string>>();
            //Environment.NewLine
            foreach (var item in result)
            {
                var comment = item.Comment ?? "";
                if (item.IsPrimary && item.Identity)
                {
                    comment += "[自增主鍵][為空代表新增,有值代表更新]";
                }
                else if (item.IsPrimary)
                {
                    comment += "[主鍵][為空代表新增,有值代表更新]";
                }
                else if (item.Identity)
                {
                    comment += "[自增]";
                }

                if (item.Length > 0)
                {
                    comment += $"[注:最多可錄入字元數：{item.Length}]";
                }

                re.Add(new Tuple<string, string>(item.Name, comment));
            }
            return (null, re);
        }

        protected (string, List<CodeGenField>) GetTableFiledList<T>(params string[] ignoreColumns)
        {
            var type = typeof(T);
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute == null || string.IsNullOrEmpty(tableAttribute.Name)) return ($"獲取表結構失敗,沒有找到繼承{nameof(LinqToDBEntity)}的{type.Name}類", null); ;

            List<CodeGenField> result = null;
            try
            {
                result = GetDbModels(tableAttribute.Db, type.Name);
            }
            catch (Exception e)
            {
                return ($"獲取表結構失敗,{e.Message}", null); ;
            }

            if (result == null || !result.Any())
            {
                return ("獲取表結構失敗", null);
            }

            //檢視表有沒有主鍵
            if (!result.Any(r => r.IsPrimary))
            {
                return ($"{type.Name}沒有定義主鍵", null);
            }

            result.ForEach(r => r.TableName = tableAttribute.Name);

            return (null, result);
        }

        /// <summary>
        /// 獲取所有的DBClass
        /// </summary>
        protected List<CodeGenField> GetDbModels(string dbName, string tableName)
        {
            var modelAss = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => (assembly.GetName().Name.Equals("DbModel")));
            if (modelAss == null)
            {
                throw new ArgumentException("assemblys");
            }
            var types = modelAss.GetExportedTypes();
            //可能有多個相同的class？但是不是同一個db
            var targetClass = (from t in types
                               let taa = t.GetCustomAttribute<TableAttribute>()
                               where t.BaseType == typeof(LinqToDBEntity) &&
                                          !t.IsAbstract
                                          && !t.IsInterface
                                          && taa != null
                                          && taa.Db.Equals(dbName) && t.Name.Equals(tableName)
                               select t).FirstOrDefault();

            if (targetClass == null)
            {
                throw new ArgumentException($"can not found class :{tableName} in dbName:{dbName}");
            }

            var properties = targetClass.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(e => e.CanWrite).ToArray();

            var result = (from item in properties
                          let r = item.GetCustomAttribute<ColumnAttribute>()
                          select new CodeGenField
                          {
                              Name = item.Name,
                              IsPrimary = item.GetCustomAttribute<PrimaryKeyAttribute>() != null,
                              Identity = item.GetCustomAttribute<IdentityAttribute>() != null,
                              FieldName = r.Name,
                              Length = r.Length,
                              Comment = string.IsNullOrEmpty(r.Comment) ? "" : r.Comment.Replace(",", "").Replace("→", "")
                          }).ToList();
            return result.ToList();
        }
    }

    /// <summary>
    /// 基礎泛型倉庫
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : BaseRepository, IRepository<T> where T : class
    {

        /// <summary>
        /// 通用的匯入功能
        /// 只會針對excel有的欄位進行更新或者插入
        /// </summary>
        /// <param name="inputFileStream"></param>
        /// <param name="doAction"></param>
        /// <returns></returns>
        protected Tuple<bool, string> CommonUpload(Stream inputFileStream, Func<T, string> doAction = null)
        {
            var fields = this.GetTableFiledList<T>();
            if (!string.IsNullOrEmpty(fields.Item1))
            {
                return Tuple.Create(false, fields.Item1);
            }

            //主鍵
            var primary = fields.Item2.First(r => r.IsPrimary);

            //讀數據
            var data = inputFileStream.ReadExcelSheetWithHeader<T>();
            if (data == null) return Tuple.Create(false, "讀取Excel失敗");
            var headerList = data.Item1;
            var dataList = data.Item2;
            if (headerList == null || headerList.Count < 1 || dataList == null || dataList.Count < 1) return Tuple.Create(false, "讀取Excel內容為空");

            var havaPrimary = false;
            var haveIdentity = false;
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Where(e => e.CanWrite).ToDictionary(r => r.Name, y => y);
            var doHeaderList = new List<CodeGenField>();
            foreach (var header in headerList)
            {
                var cc = fields.Item2.FirstOrDefault(r => header.Equals(r.Name));
                if (cc == null)
                {
                    return Tuple.Create(false, $"{typeof(T).Name}不存在列:{header}");
                }

                if (cc.Identity)
                {
                    haveIdentity = true;
                }

                if (cc.IsPrimary)
                {
                    havaPrimary = true;
                }
                else
                {
                    doHeaderList.Add(cc);
                }
            }

            if (!havaPrimary)
            {
                return Tuple.Create(false, $"Excel不存在主鍵列:{primary.Name}");
            }

            var dbName = doHeaderList.First().TableName;

            var primaryKey = properties.First(r => r.Key.Equals(primary.Name));
            var updateSuccCount = 0;
            var insertSuccCount = 0;
            var index = 1; //除去頭第2行開始算
            foreach (var item in dataList)
            {
                index++;
                if (doAction != null)
                {
                    var filter = doAction.Invoke((T)item);
                    if (!string.IsNullOrEmpty(filter))
                    {
                        Transaction.Current.Rollback();
                        return Tuple.Create(false, $"處理到第{index}行失敗:{filter}");
                    }
                }

                if (item == null) continue;
                try
                {
                    var primaryValue = primaryKey.Value.GetValue(item);

                    var havaPrimaryValue = primaryValue != null && !string.IsNullOrEmpty(primaryValue.ToString()) &&
                                           !primaryValue.ToString().Equals("0");
                    if (havaPrimaryValue)
                    {
                        if (!haveIdentity)
                        {
                            //檢視是否這個主鍵值是否在數據庫中存在
                            SQL searchSql = $"select count(*) from {dbName} where {primary.FieldName} = @primayValue";
                            searchSql = searchSql["primayValue", primaryValue];
                            var isExsit = this.DB.Query<int>(searchSql).FirstOrDefault() == 1;
                            if (!isExsit)
                            {
                                goto Insert;
                            }
                        }

                        //是自增型的且有值 肯定是要更新的
                        SQL updateSql = $"UPDATE {dbName} set ";
                        var indexForUpdate = 0;

                        foreach (var headerItem in doHeaderList)
                        {
                            updateSql += $" {headerItem.Name}=@p{indexForUpdate} ";
                            if (indexForUpdate != doHeaderList.Count - 1)
                            {
                                updateSql += ",";
                            }

                            updateSql = updateSql[$"p{indexForUpdate}", properties[headerItem.Name].GetValue(item)];
                            indexForUpdate++;
                        }

                        updateSql += $" where {primary.FieldName} = @primayValue";
                        updateSql = updateSql["primayValue", primaryValue];
                        updateSuccCount += this.DB.Execute(updateSql);
                        continue;
                    }
                Insert:

                    insertSuccCount += this.DB.Insert(item, ignoreNullInsert: true);

                }
                catch (Exception e)
                {
                    Transaction.Current.Rollback();
                    return Tuple.Create(true, $"處理到第{index}行失敗:{e.Message}");
                }

            }

            return Tuple.Create(true, $"數據共:{dataList.Count},更新成功:{updateSuccCount}條,插入成功:{insertSuccCount}條");
        }


        #region Impl




        /// <summary>
        /// 目前的Entity
        /// </summary>
        public IQueryable<T> Entity => DB.GetTable<T>();


        public int Save(T entity)
        {
            return this.DB.Insert(entity);
        }

        public long BatchSave(T[] entities)
        {
            return this.DB.BulkCopy(entities).RowsCopied;
        }

        public long InsertWithIdentity(T entity)
        {
            return (long)this.DB.InsertWithIdentity(entity);
        }




        public int Update(T entity)
        {
            return this.DB.Update(entity);
        }


        public int Delete(T entity)
        {
            return this.DB.Delete(entity);
        }

        public int BatchDelete(T[] entities)
        {
            return this.DB.Delete(entities);
        }



        public int Delete(Expression<Func<T, bool>> exp)
        {
            return this.DB.Tables.Get<T>().Delete(exp);
        }

        public T FindSingle(Expression<Func<T, bool>> exp)
        {
            return this.DB.Tables.Get<T>().FirstOrDefault(exp);
        }

        public bool IsExist(Expression<Func<T, bool>> exp)
        {
            return this.DB.Tables.Get<T>().Any(exp);
        }

        public IEnumerable<T1> Query<T1>(string sql, params DataParameter[] parameters)
        {
            return this.DB.Query<T1>(sql, parameters);
        }

        public IEnumerable<T1> Query<T1>(T1 templete, string sql, params DataParameter[] parameters)
        {
            return this.DB.Query<T1>(templete, sql, parameters);
        }



        public T1 Execute<T1>(string sql, params DataParameter[] parameters)
        {
            return this.DB.Execute<T1>(sql, parameters);
        }


        #endregion



    }
}
