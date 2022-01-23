using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace PJW.Web.Models
{
    public class WorkLogViewModel
    {
        [DisplayName("任務編號")]
        public int WorkLogNo { get; set; }
        [DisplayName("任務識別碼")]
        public string WorkLogUniqueId { get; set; }
        [DisplayName("任務名稱")]
        public string WorkLogSubject { get; set; }
        [DisplayName("任務說明")]
        public string WorkLogContent { get; set; }
        [DisplayName("任務分類編號")]
        public int? WorkLogSortNo { get; set; }
        [DisplayName("任務分類名稱")]
        public string WorkLogSortName { get; set; }
        [DisplayName("部門代號")]
        public int? WorkLogDeptNo { get; set; }
        [DisplayName("部門名稱號")]
        public string WorkLogDeptName { get; set; }
        [DisplayName("任務新增人員編號")]
        public int? WorkLogAddUserId { get; set; }
        [DisplayName("任務新增人員名稱號")]
        public string WorkLogAddUserName { get; set; }
        [DisplayName("任務新增日期")]
        public DateTime? WorkLogAddDate { get; set; }
        [DisplayName("任務指派單位編號")]
        public int? WorkLogAssignDeptId { get; set; }
        [DisplayName("任務指派單位名稱")]
        public string WorkLogAssignDeptName { get; set; }
        [DisplayName("任務指派人員編號")]
        public int? WorkLogAssignId { get; set; }
        [DisplayName("任務指派人員名稱")]
        public string WorkLogAssignName { get; set; }
        [DisplayName("任務開始日期")]
        public DateTime? WorkLogStartDate { get; set; }
        [DisplayName("任務開始時間")]
        public string WorkLogStartTime { get; set; }
        [DisplayName("任務結束日期")]
        public DateTime? WorkLogEndDate { get; set; }
        [DisplayName("任務結束時間")]
        public string WorkLogEndTime { get; set; }
        [DisplayName("任務需求等級")]
        public int WorkLogPriority { get; set; }
        [DisplayName("任務需求等級名稱")]
        public string WorkLogPriorityName { get; set; }
        [DisplayName("任務執行說明")]
        public string WorkLogNote { get; set; }
        [DisplayName("任務狀態")]
        public int WorkLogStatus { get; set; }
        [DisplayName("任務狀態名稱")]
        public string WorkLogStatusName { get; set; }
        [DisplayName("任務屬性")]
        public int WorkLogType { get; set; }
        [DisplayName("任務屬性名稱")]
        public string WorkLogTypeName { get; set; }
        [DisplayName("任務回覆說明")]
        public string WorkLogReplyNote { get; set; }
        [DisplayName("任務評鑑分數")]
        public string WorkLogScore { get; set; }
        [DisplayName("任務分享人員編號")]
        public int? WorkLogShareUserId { get; set; }
        [DisplayName("任務分享人員名稱")]
        public string WorkLogShareUserName { get; set; }

    }
}
