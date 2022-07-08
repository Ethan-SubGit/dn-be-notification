using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.SmsServiceModel
{
    public class SmsMessageType
    {
        public string SmsId { get; set; }
        public string MessageId { get; set; }
        public string ContryCode { get; set; }
        public DateTime? CompleteTime { get; set; }
        public string From { get; set; }
        public string Status { get; set; }
        public string ContentType { get; set; }

        public string TelcoCode { get; set; }
        public string StatusMessage { get; set; }
        public string Content { get; set; }
        public string To { get; set; }
        public DateTime? RequestTime { get; set; }
        public string StatusName { get; set; }
        public string StatusCode { get; set; }
    }
}
