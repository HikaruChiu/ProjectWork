using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModels.Reuqest
{
    public class AddMenuVm
    {
        /// <summary>
        /// 父節點Tid
        /// </summary>
        public long ParentTid { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderRule { get; set; }

        /// <summary>
        /// 樣式
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 是否要在頁面上顯示該選單
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 主鍵
        /// </summary>
        public long Tid { get; set; }
    }
}
