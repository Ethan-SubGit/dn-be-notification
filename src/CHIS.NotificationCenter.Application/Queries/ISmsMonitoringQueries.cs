using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.SmsMonitoring;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Application.Models.QueryType;

namespace CHIS.NotificationCenter.Application.Queries
{
    public interface ISmsMonitoringQueries
    {


        /// <summary>
        /// SMS ExecusionLog테이블 검색
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="searchText"></param>
        /// <param name="emplyeeId"></param>
        /// <returns></returns>
        Task<SmsSendLogView> SearchSendLog(DateTime? fromDate, DateTime? toDate, string searchText, string emplyeeId , int skip = 0, int take = 100);

        /// <summary>
        /// SMS SentLog테이블 리턴(세부결과목록 리턴)
        /// </summary>
        /// <param name="smsId"></param>
        /// <returns></returns>
        Task<List<SmsReceiveLogDto>> SearchReceiveLog(string messageDispatchItemId);

        Task<SmsResultPatientPackageView> SearchSmsResultPatient(
            DateTime? fromDateTime, 
            DateTime? toDateTime, 
            int skip, 
            int take, 
            string patientId,
            string employeeId,
            string searchText,
            List<string> serviceCodeFilter,
            SmsResultFilterType smsResultFilterType,
            string searchTelno);

        Task<List<SmsReceiveLogDto>> SearchUnprocessedReceiveLogsForStatistics(int delayMinute);
        Task<List<SmsReceiveLog>> SearchFilteringReceiveLog(string messageDispatchItemId);
        Task<HospitalReadModel> GetHospitalInfo(string hospitalId);
    }
}
