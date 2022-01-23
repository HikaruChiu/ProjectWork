using Configuration;
using Repository;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels.Result;
using ViewModels.Reuqest;

namespace PJW.Web
{
    /// <summary>
    /// 來自Intranet 的SSO
    /// </summary>
    public class SSO
    {
        private readonly IAccountRespository AccountRespository;
                
        public SSO(IAccountRespository _accountRespository)
        {
            AccountRespository = _accountRespository;
        }
        public async Task<ResultJsonNoDataInfo> LogOnSSO(LogOnVM info)
        {            
            var result = new ResultJsonNoDataInfo();
            var respositoryResult = await AccountRespository.LogOn(info);
            if (respositoryResult.Item1)
            {

                result.Status = ResultConfig.Ok;
                result.Info = ResultConfig.SuccessfulMessage;
            }
            else
            {
                result.Status = ResultConfig.Fail;
                result.Info = respositoryResult.Item2 ?? ResultConfig.FailMessage;
            }

            return result;
        }
    }
}
