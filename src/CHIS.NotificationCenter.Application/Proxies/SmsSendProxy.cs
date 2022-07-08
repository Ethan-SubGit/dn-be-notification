using CHIS.Framework.Core;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering;
using CHIS.NotificationCenter.Application.Models.ProxyModels.SmsServiceModel;
using CHIS.NotificationCenter.Domain.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Proxies
{
    /// <summary>
    /// TA API를 통해 sms를 전송
    /// </summary>
    public class SmsSendProxy : BSLBase, ISmsSendProxy
    {
        public SmsSendProxy(ICallContext context) : base(context)
        {
        }

        public async Task<SmsResultView> SendSmsToTAApi(SmsSendType p_smsSendType)
        {

            try
            {
                using (ServiceClient client = new ServiceClient(base.Context, "commonservice"))
                {
                    var smsBasicView = client.Post<SmsResultView>($"/sms/v1/sms", p_smsSendType);
                    if (smsBasicView != null)
                    {
                        return smsBasicView;
                    }
                    else
                    {
                        return new SmsResultView();
                    }


                }
            }
            catch (Exception e)
            {
                //throw e;
                return new SmsResultView();
            }

        }

        /// <summary>
        /// TA API로 검색결과를 조회
        /// https://development.c-his.com:16001/sms/v1/sms/0-ESG-201903-3374749-1
        /// </summary>
        /// <param name="p_messageId"></param>
        /// <returns></returns>
        public SmsResultView GetSmsResult(string p_messageId)
        {
            using (ServiceClient client = new ServiceClient(base.Context, "commonservice"))
            {
                try
                {
                    //Dictionary<string, string> param_ = new Dictionary<string, string>();
                    //param_.Add("messageId", "0-ESG-201903-3374749-1");

                    //SmsResultView smsBasicView_ = client.Get<SmsResultView>($"/sms/v1/sms/", param_);
                    //SmsResultView smsBasicView_ = client.Get<SmsResultView>($"/sms/v1/sms?messageId={p_messageId}");
                    SmsResultView smsBasicView_ = client.Get<SmsResultView>($"/sms/v1/sms/{p_messageId}");
                    //object smsBasicView_ = client.Get<object>($"/sms/v1/sms?messageId={p_messageId}");
                    if (smsBasicView_ != null)
                    {
                        return smsBasicView_;
                    }
                    else
                    {
                        //return new SmsResultView();
                        return new SmsResultView()
                        {
                            Status = "ERROR",
                            ErrorMessage = "No Result"
                        };
                    }
                }
                catch (Exception ex)
                {

                    return new SmsResultView()
                    {
                        Status = "ERROR",
                        ErrorMessage = "API ERROR" + ex
                    };
                }
            }
        }

    }
}
