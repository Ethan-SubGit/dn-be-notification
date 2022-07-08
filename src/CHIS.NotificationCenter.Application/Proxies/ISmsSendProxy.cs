using CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering;
using CHIS.NotificationCenter.Application.Models.ProxyModels.SmsServiceModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Proxies
{
    public interface ISmsSendProxy
    {

        /// <summary>
        /// TA API를 통해 SMS를 발송
        /// </summary>
        /// <param name="p_smsSendType"></param>
        /// <returns></returns>
        Task<SmsResultView> SendSmsToTAApi(SmsSendType p_smsSendType);

        /// <summary>
        /// SMS전송결과를 조회한다. (from TA API)
        /// </summary>
        /// <param name="p_messageId"></param>
        /// <returns></returns>
        //Task<SmsResultView> GetSmsResult(string p_messageId);
        SmsResultView GetSmsResult(string p_messageId);
    }
}
