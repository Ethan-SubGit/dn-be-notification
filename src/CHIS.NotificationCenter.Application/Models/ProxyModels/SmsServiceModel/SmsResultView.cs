using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.SmsServiceModel
{
    public class SmsResultView
    {
        public string SmsId { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public List<SmsMessageType> Messages { get; set; }

        public SmsResultView()
        {
        }

        public SmsResultView(string smsId, string status, string errorMessage
            , List<SmsMessageType> messages )
        {
            SmsId = smsId;
            Status = status;
            ErrorMessage = errorMessage;
            Messages = messages;

        } 
    }
}
