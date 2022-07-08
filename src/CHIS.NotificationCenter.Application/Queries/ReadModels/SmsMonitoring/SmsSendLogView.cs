using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models.CommonModels;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.SmsMonitoring
{
    public class SmsSendLogView
    {
        public IList<SmsSendLogDto> SmsSendLogsExtension { get; set; }
        public int TotalRecordCount { get; set; }
        public SmsSendLogView()
        {
        }
    }



}
