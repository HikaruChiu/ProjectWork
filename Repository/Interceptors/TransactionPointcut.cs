using System;
using System.Transactions;
using System.Threading.Tasks;
using Autofac.Annotation;
using NLog;


namespace Repository.Interceptors
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnableTransactionScope : Attribute
    {
        public EnableTransactionScope()
        {

        }

        public EnableTransactionScope(int scopeTimeout)
        {
            this.ScopeTimeout = scopeTimeout;
        }

        public EnableTransactionScope(TransactionScopeOption scopeType)
        {
            this.TransactionScopeOption = scopeType;
        }

        public EnableTransactionScope(int scopeTimeout, TransactionScopeOption scopeType)
        {
            this.ScopeTimeout = scopeTimeout;
            this.TransactionScopeOption = scopeType;
        }

        /// <summary>
        /// 單位是秒
        /// </summary>
        public int ScopeTimeout { get; set; }

        /// <summary>
        /// 型別
        /// 預設型別為：Required 當外層有事物則用該事物 沒有的話就建立
        /// RequiresNew 的話是不管外層有沒有事物都會建立一個新事物
        /// Suppress 不參與任何事務
        /// </summary>
        public TransactionScopeOption TransactionScopeOption { get; set; } = TransactionScopeOption.Required;
    }

    /// <summary>
    /// 事物切面，所有是Respository結尾的容器對象的方法上打了EnableTransactionScope的會走進事物  開頭的方法都會走進這個切面
    /// </summary>
    [Pointcut(Class = "*Respository", AttributeType = typeof(EnableTransactionScope))]
    public class TransactionPointcut
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// 事物環繞攔截器
        /// </summary>
        /// <returns></returns>
        [Around]
        public async Task RunWithTransaction(AspectContext aspectContext, AspectDelegate next, EnableTransactionScope option)
        {
            logger.Debug($"start transactionScope on `{aspectContext.TargetMethod.DeclaringType?.FullName + "." + aspectContext.TargetMethod.Name}`");

            var timeOut = option.ScopeTimeout;
            if (timeOut < 1)
            {
                timeOut = 30 * 60;//預設設定半小時
            }
            using (var scope = new TransactionScope(option.TransactionScopeOption, TimeSpan.FromSeconds(timeOut), TransactionScopeAsyncFlowOption.Enabled))
            {
                await next(aspectContext);

                if (Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
                {
                    scope.Complete();
                    logger.Debug($"submit transactionScope on `{aspectContext.TargetMethod.DeclaringType?.FullName + "." + aspectContext.TargetMethod.Name}`");
                }
            }
        }
    }
}