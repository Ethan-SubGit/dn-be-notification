using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;

namespace CHIS.NotificationCenter.Application.Services
{
    public interface ISmsService
    {
        //Task<bool> SendSmsMessage(List<SmsRecipientDto> smsRecipientList, string content,string messageDispatchItemId,bool isReservedSms);

        Task<bool> SendSmsMessage(string messageDispatchItemId);
        Task<bool> ResendPatientSmsMessage(string messageDispatchItemId);
        Task<bool> BatchUpdateSmsReceiveLog(int delayMinute);

    }
}
