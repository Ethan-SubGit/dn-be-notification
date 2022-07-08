using CHIS.NotificationCenter.Domain.SeedWork;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate
{
    public interface ISmsMonitoringRepository : IRepository<SmsReceiveLog>
    {
        SmsReceiveLog CreateSmsReceiveLog(SmsReceiveLog smsReceiveLog);

        SmsSendLog CreateSmsSendLog(SmsSendLog smsSendLog);

        Task<SmsReceiveLog> FindSmsReceiveLog(string id);
        Task<List<SmsReceiveLog>> FindSmsReceiveLogByMessageDispatchItemId(string messageDispatchItemId);

        Task<SmsSendLog> FindSmsSendLogByMessageDispatchItemId(string messageDispatchItemId);

        Task<List<SmsSendLog>> PrepareSmsBatchExecution(string batchId);

        Task<List<SmsReceiveLog>> SearchUnprocessedReceiveLogs(int delayMinute);

        Task<List<SmsReceiveLog>> FindPatientMergingTargets(string closingPatientId);
        /// <summary>
        /// 결과가 없는 sms message 리스트.
        /// 현재시간(timeZoneId)기준
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        //Task<List<SmsReceiveLog>> RetriveNoResultMessage(string timeZoneId, int delayMinute);

        /// <summary>
        /// 결과가 없는 sms message 리스트의 messageId
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <param name="stdMinTime"></param>
        /// <returns></returns>
        //Task<List<string>> RetriveNoResultMessageIds(string timeZoneId, int delayMinute);

        ///// <summary>
        ///// 검색범위(messageid)를 지정한 결과조회 목록 
        ///// </summary>
        ///// <param name="messageIds"></param>
        ///// <returns></returns>
        //Task<List<SmsReceiveLog>> SearchReceiveLogs(List<string> messageIds);

    }
}
