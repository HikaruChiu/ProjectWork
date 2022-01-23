using Infrastructure.StaticExt;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels.Condition;

namespace ViewModels.Reuqest
{



    public class CodeGenVm
    {
        /// <summary>
        /// 表名稱
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 欄位列表
        /// </summary>
        public List<string> Columns
        {
            get
            {
                if (string.IsNullOrEmpty(ColumnStr))
                {
                    return new List<string>();
                }
                return ColumnStr.Split(',').ToList();
            }
        }

        /// <summary>
        /// 欄位名稱列表多個逗號隔開
        /// </summary>
        public string ColumnStr { get; set; }
    }




}
