using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    /// <summary>
    /// SMS 전송결과에 대한 통계 업데이트 MQ.
    /// 2020.1. 사용하지 않음으로 변경.
    /// </summary>
    public class SmsResultUpdateIntegrationEvent : IntegrationEvent
    {
        public int RecordCount { get; set; }
        public List<SmsReceiveLogDto> SmsReceiveLogList { get; set; }
        public SmsResultUpdateIntegrationEvent(List<SmsReceiveLogDto> smsReceiveLogList)
        {
            if (smsReceiveLogList == null)
            {
                throw new ArgumentException(nameof(smsReceiveLogList));
            }

            int receiveCount = smsReceiveLogList.Count;
            SmsReceiveLogList = smsReceiveLogList;
            RecordCount = receiveCount;
        }

        
    }
}
