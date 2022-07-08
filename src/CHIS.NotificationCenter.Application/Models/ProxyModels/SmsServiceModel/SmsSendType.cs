using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.SmsServiceModel
{
    /// <summary>
    /// sms를 TA API로 보내기위한 
    /// </summary>
    public class SmsSendType
    {
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string Country { get; set; }
        public string From { get; set; }
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string EmployeeId { get; set; }

        public SmsSendType()
        {
        }

        public SmsSendType(string type, string contentType, string country
            ,string from, List<string> to, string content, string employeeId)
        {
            Type = type;
            ContentType = contentType;
            Country = country;
            From = from;
            To = to;
            Content = content;
            EmployeeId = employeeId;
        }



    }
}
