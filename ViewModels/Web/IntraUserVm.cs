using ViewModels.Condition;

namespace ViewModels.Reuqest
{

    public class IntraUserVm : ConditionBase
    {
        /// <summary>
        /// 員工帳號
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 員工EMail
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// 員工姓名
        /// </summary>
        public string UserName { get; set; }

    }

}
