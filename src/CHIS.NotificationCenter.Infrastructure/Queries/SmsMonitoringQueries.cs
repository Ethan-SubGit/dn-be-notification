using CHIS.Framework.Core;
using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CHIS.Framework.Layer;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models;
using CHIS.Share.MedicalAge;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.SmsMonitoring;
using CHIS.Share.Masking;
using CHIS.Framework.Core.Localization;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Application.Models.ProxyModels.HospitalBuilder;

namespace CHIS.NotificationCenter.Infrastructure.Queries
{
    public class MessageSentLogQueries : DALBase, ISmsMonitoringQueries
    {
        private readonly ICallContext _callContext;
        private readonly ITimeManager _timeManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly NotificationCenterContext _notificationCenterContext;


        public MessageSentLogQueries(ICallContext callContext
            , ITimeManager timeManager
            , ILocalizationManager localizationManager
            , NotificationCenterContext notificationCenterContext
            ) : base(callContext)
        {
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            this._localizationManager = localizationManager;
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _notificationCenterContext = notificationCenterContext;
        }

        /// <summary>
        /// 직원 SMS Sent Logview 조회
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="searchText"></param>
        /// <param name="emplyeeId"></param>
        /// <returns></returns>
        public async Task<SmsSendLogView> SearchSendLog(DateTime? fromDate
            , DateTime? toDate, string searchText, string emplyeeId, int skip = 0, int take = 100)
        {
            
            SmsSendLogView view = new SmsSendLogView();

            var query = from smsSendLog in _notificationCenterContext.SmsSendLogs
                        join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                            .Where(p => p.TenantId == _callContext.TenantId
                                && p.HospitalId == _callContext.HospitalId
                            )
                        on smsSendLog.SenderId equals employee.Id into gj
                        //on new { senderId = smsSendLog.SenderId, hospitalId = smsSendLog.HospitalId, tenantId = smsSendLog.TenantId } 
                        //    equals new { senderId = employee.Id, hospitalId = employee.HospitalId, tenantId = employee.TenantId } 
                        //into gj
                        from x in gj.DefaultIfEmpty()
                           where 
                               smsSendLog.TenantId == _callContext.TenantId 
                               && smsSendLog.HospitalId == _callContext.HospitalId
                               && smsSendLog.ReservedTime >= fromDate 
                               && smsSendLog.ReservedTime <= toDate
                               //&& x.TenantId == _callContext.TenantId
                               //&& x.HospitalId == _callContext.HospitalId
                               //&& (smsSendLog.SmsRecipientType == SmsRecipientType.Employee  || smsSendLog.SmsRecipientType == SmsRecipientType.EmployeeDirectMobile)
                               && (new[] { SmsRecipientType.Employee, SmsRecipientType.EmployeeDirectMobile }).Contains(smsSendLog.SmsRecipientType)
                               //&& SmsRecipientType.    
                               //&& (string.IsNullOrEmpty(searchText) || EF.Functions.Like(smsSendLog.Content, "%" + searchText + "%"))

                        orderby smsSendLog.ReservedTime descending
                           select new SmsSendLogDto()
                           {
                               TenantId = smsSendLog.TenantId,
                               HospitalId = smsSendLog.HospitalId,
                               Content = smsSendLog.Content,
                               CallingNumber = smsSendLog.CallingNumber,
                               IsReservedSms = smsSendLog.IsReservedSms,
                               ReservedTime = smsSendLog.ReservedTime,
                               ExecutionTime = smsSendLog.ExecutionTime,
                               SmsProgressStatus = smsSendLog.SmsProgressStatus,
                               MessageDispatchItemId = smsSendLog.MessageDispatchItemId,
                               SmsTraceId = smsSendLog.SmsTraceId,
                               CallStatusCode = smsSendLog.CallStatusCode,
                               ErrorMessage = smsSendLog.ErrorMessage,
                               SenderId = smsSendLog.SenderId,
                               FullName = smsSendLog.SenderId == "System" ? "System" : (x.FullName) ?? "",
                               DepartmentId = smsSendLog.SenderId == "System" ? "-" : (x.DepartmentId) ?? "",
                               DisplayId = smsSendLog.SenderId == "System" ? "-" : (x.DisplayId) ?? "",
                               TotalCount = _notificationCenterContext.SmsReceiveLogs
                                .Where(p => p.MessageDispatchItemId == smsSendLog.MessageDispatchItemId
                                    && p.TenantId == _callContext.TenantId
                                    && p.HospitalId == _callContext.HospitalId
                                ).Count(),
                               SuccessCount = _notificationCenterContext.SmsReceiveLogs
                                .Where(p => p.MessageDispatchItemId == smsSendLog.MessageDispatchItemId 
                                    && p.StatusName == "success"
                                    && p.TenantId == _callContext.TenantId
                                    && p.HospitalId == _callContext.HospitalId
                                 ).Count()
                           };

            if (!string.IsNullOrEmpty(emplyeeId))
            {
                query = query.Where(x => x.SenderId == emplyeeId);
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(x => x.Content.Contains(searchText, StringComparison.Ordinal));
            }

            var list = await query.Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);
            view.SmsSendLogsExtension = list;
            view.TotalRecordCount = await query.CountAsync().ConfigureAwait(false);

            return view;

        }

        /// <summary>
        /// SMS ID로 세부 전송결과를 조회.
        /// </summary>
        /// <param name="smsId"></param>
        /// <returns></returns>
        public async Task<List<SmsReceiveLogDto>> SearchReceiveLog(string messageDispatchItemId)
        {
            IList<MaskingColumnAuthView> maskingColumnViews
                    = await MaskingColumn.GetMaskingColumns(_callContext, _callContext.EmployeeId).ConfigureAwait(false);

            var list = await _notificationCenterContext.SmsReceiveLogs
                                .Where(p => p.MessageDispatchItemId == messageDispatchItemId
                                        && p.TenantId == _callContext.TenantId 
                                        && p.HospitalId == _callContext.HospitalId )
                                .Select(p => new SmsReceiveLogDto()
                                {
                                    Id = p.Id,
                                    CompleteTime = p.CompleteTime,
                                    Content = p.Content,
                                    ContentType = p.ContentType,
                                    CountryCode = p.CountryCode,
                                    HospitalId = p.HospitalId,
                                    IsSuccess = p.IsSuccess,
                                    MessageDispatchItemId = p.MessageDispatchItemId,
                                    MessageId = p.MessageId,
                                    Mobile = (string.IsNullOrEmpty(p.Mobile)) ? "No Number" : 
                                        p.Mobile.Length > 8 ?
                                    MaskingColumn.SetMasking(maskingColumnViews, MaskingColumnType.Telephone, MaskingTarget.Patient,p.Mobile)
                                        : p.Mobile,
                                    MobileWithNoMasking = (string.IsNullOrEmpty(p.Mobile)) ? "No Number" : p.Mobile,
                                    //p.Mobile, // 마스킹 처리
                                    Name = p.Name,
                                    RequestTime = p.RequestTime,
                                    //SentTimeStamp = p.SentTimeStamp,
                                    SmsId = p.SmsId,
                                    SmsRecipientType = p.SmsRecipientType,
                                    StatusCode = (string.IsNullOrEmpty(p.Mobile)) ? "1" : p.StatusCode,
                                    StatusMessage = (string.IsNullOrEmpty(p.Mobile)) ? "No Number" : p.StatusMessage,
                                    StatusName = p.StatusName,
                                    telcoCode = p.telcoCode,
                                    TenantId = p.TenantId
                                })
                                .ToListAsync().ConfigureAwait(false);
            return list;
        }


        /// <summary>
        /// TO-DO : 환자 중심 SMS 전송 모니터링
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="searchText"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public async Task<SmsResultPatientPackageView> SearchSmsResultPatient(
            DateTime? fromDateTime,
            DateTime? toDateTime,
            int skip,
            int take,
            string patientId,
            string employeeId,
            string searchText,
            List<string> serviceCodeFilter,
            SmsResultFilterType smsResultFilterType,
            string searchTelno)
        {
            //string TimeZoneId = _callContext.TimeZoneId;
            DateTime currentDateTimeLocal = _timeManager.GetNow();
            //DateTime currentDateTimeUTC = _timeManager.ConvertToUTC(TimeZoneId, currentDateTimeLocal);
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;


            IList<MaskingColumnAuthView> maskingColumnViews
                = await MaskingColumn.GetMaskingColumns(_callContext, _callContext.EmployeeId).ConfigureAwait(false);

            #region base query
            var query = from msgSpecification in _notificationCenterContext.MessageSpecifications
                            .Where(x => x.ServiceType == Domain.Enum.NotificationServiceType.SMS)
                        join msgDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on msgSpecification.ServiceCode equals msgDispatchItem.ServiceCode

                        join smsSendLog in _notificationCenterContext.SmsSendLogs
                        on msgDispatchItem.Id equals smsSendLog.MessageDispatchItemId

                        join smsReceiveLog in _notificationCenterContext.SmsReceiveLogs
                        on msgDispatchItem.Id equals smsReceiveLog.MessageDispatchItemId

                        join patient in _notificationCenterContext.Query<PatientReadModel>()
                        on smsReceiveLog.ActorId equals patient.Id

                        join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on smsSendLog.SenderId equals employee.Id into employeeProjection
                        from employee in employeeProjection.DefaultIfEmpty()
                            
                            where 

                                 msgSpecification.TenantId == tenantId
                                 && msgSpecification.HospitalId == hospitalId
                                 && msgDispatchItem.HospitalId == hospitalId
                                 && msgDispatchItem.TenantId == tenantId
                                 && smsReceiveLog.TenantId == tenantId
                                 && smsReceiveLog.HospitalId == hospitalId
                                 && patient.TenantId == tenantId
                                 && patient.HospitalId == hospitalId
                                 
                                 && (
                                     (smsSendLog.ReservedTime >= fromDateTime && smsSendLog.ReservedTime <= toDateTime)
                                     ||
                                     (smsSendLog.ExecutionTime >= fromDateTime && smsSendLog.ExecutionTime <= toDateTime)
                                  )
                                 && smsSendLog.TenantId == tenantId
                                 && smsSendLog.HospitalId == hospitalId
                        /*
                        && (string.IsNullOrEmpty(searchText) || EF.Functions.Like(smsSendLog.Content, "%" + searchText + "%"))
                        */
                        orderby smsSendLog.ReservedTime descending
                        select new SmsResultPatientView()
                        {
                            TenantId = msgDispatchItem.TenantId,
                            HospitalId = msgDispatchItem.HospitalId,
                            MessageDispatchItemId = msgDispatchItem.Id,
                            ServiceCode = msgSpecification.ServiceCode,
                            Classification = msgSpecification.Classification,
                            IsReservedSms = smsSendLog.IsReservedSms,
                            Content = smsSendLog.Content,
                            ReservedTime = smsSendLog.ReservedTime,
                            ExecutionTime = smsSendLog.ExecutionTime,
                            SenderId = smsSendLog.SenderId,
                            SenderName = smsSendLog.SenderId == "System" ? "System" : (employee.FullName) ?? "",
                            Mobile = smsReceiveLog.Mobile, // Masking 처리
                            SmsProgressStatus = smsSendLog.SmsProgressStatus,
                            SmsTraceId = smsSendLog.SmsTraceId,
                            CallStatusCode = smsSendLog.CallStatusCode, // 메시징 서버로의 발송 요청 상태 , 200 성공, 그외 실패
                            ErrorMessage = smsSendLog.ErrorMessage,     // 발송 요청 실패 메시지
                            MessageId = smsReceiveLog.MessageId,
                            StatusCode = smsReceiveLog.StatusCode,             // 이통사 전송후 받은 코드 0 성공, 그외 실패
                            StatusName = smsReceiveLog.StatusName,              // 단말 수신상태 결과명 (null 일경우 waiting)
                            StatusMessage = smsReceiveLog.StatusMessage,        // 단말수신상태 결과 메시지
                            PatientId = smsReceiveLog.ActorId,
                            PatientName = patient.PatientFullName,
                            Age = patient.BirthDate.Value == null ? "" : EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patient.BirthDate.Value, currentDateTimeLocal,
                                 new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일"))),
                            DisplayId = patient.PatientDisplayId,
                            Sex = string.Equals(patient.GenderCode, "01", StringComparison.Ordinal) ? "M" :
                                     string.Equals(patient.GenderCode, "02", StringComparison.Ordinal) ? "F" : "",
                            IsAgreeToUsePrivacyData = smsReceiveLog.IsAgreeToUsePrivacyData
                        };
            #endregion

            if (!string.IsNullOrEmpty(patientId))
            {
                // TO-DO : patientId로 검색 필터 로직 넣기
                query = query.Where(x => x.PatientId == patientId);
            }
            if (!string.IsNullOrEmpty(searchTelno))
            {
                query = query.Where(x => x.Mobile.Contains(searchTelno, StringComparison.Ordinal));
            }

            if (serviceCodeFilter != null && serviceCodeFilter.Count > 0)
            //if (serviceCodeFilter != null)
            {
                // TO-DO : SMS 분류 검색 필터 로직 넣기
                query = query.Where(x => serviceCodeFilter.Contains(x.ServiceCode));
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(x => x.Content.Contains(searchText, StringComparison.Ordinal));
            }


            //전송결과 조회 조건절
            query = getResultFilter(smsResultFilterType, query);

            var list = await query.Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);
            var returnList = list.Select(p => new SmsResultPatientView
            {
                Age = p.Age,
                CallStatusCode = p.CallStatusCode,
                Classification = p.Classification,
                Content = p.Content,
                DisplayId = p.DisplayId,
                ErrorMessage = p.ErrorMessage,
                ExecutionTime = p.ExecutionTime,
                HospitalId = p.HospitalId,
                IsAgreeToUsePrivacyData = p.IsAgreeToUsePrivacyData,
                IsReservedSms = p.IsReservedSms,
                MessageDispatchItemId = p.MessageDispatchItemId,
                MessageId = p.MessageId,
                Mobile = (string.IsNullOrEmpty(p.Mobile)) ? "No Number" : p.Mobile.Length > 8 ?
                                                        MaskingColumn.SetMasking(maskingColumnViews, MaskingColumnType.Telephone, MaskingTarget.Patient, p.Mobile)
                                                        : p.Mobile,
                MobileWithNoMasking = (string.IsNullOrEmpty(p.Mobile)) ? "No NUmber" : p.Mobile,
                PatientId = p.PatientId,
                PatientName = p.PatientName,
                ReservedTime = p.ReservedTime,
                SenderId = p.SenderId,
                SenderName = p.SenderName,
                ServiceCode = p.ServiceCode,
                Sex = p.Sex,
                SmsProgressStatus = p.SmsProgressStatus,
                SmsTraceId = p.SmsTraceId,
                StatusCode = p.StatusCode,
                StatusMessage = p.StatusMessage,
                StatusName = p.StatusName,
                TenantId = p.TenantId
            }).ToList();
            int totalRecourdCount = query.Count();
            SmsResultPatientPackageView smsResultPatientPackageView = new SmsResultPatientPackageView();
            smsResultPatientPackageView.SmsResultPatientViews = returnList;
            smsResultPatientPackageView.TotalRecordCount = totalRecourdCount;
            return smsResultPatientPackageView;

        }

        /// <summary>
        /// 결과 조건절 조회
        /// </summary>
        /// <param name="smsResultFilterType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static IQueryable<SmsResultPatientView> getResultFilter(SmsResultFilterType smsResultFilterType, IQueryable<SmsResultPatientView> query)
        {
            switch (smsResultFilterType)
            {
                case SmsResultFilterType.Fail: // 실패
                    query = query.Where(x =>
                                    x.CallStatusCode != "200"
                                    || x.StatusName == "fail"
                                    || x.SmsProgressStatus == SmsProgressStatus.Error);
                    break;
                case SmsResultFilterType.WaitingResult: // 전송중
                    query = query.Where(x => x.CallStatusCode == "200" && string.IsNullOrEmpty(x.StatusName));
                    break;
                case SmsResultFilterType.Success: // 성공
                    query = query.Where(x => x.StatusName == "success");
                    break;
                case SmsResultFilterType.Waiting: // 발송대기 (예약발송)
                    //query = query.Where(x => x.IsReservedSms && x.SmsProgressStatus == SmsProgressStatus.BeforeProgress);
                    query = query.Where(x => x.SmsProgressStatus == SmsProgressStatus.BeforeProgress);
                    break;
                case SmsResultFilterType.All: // 전체
                default:
                    break;

            }

            return query;
        }

        /// <summary>
        /// 결과업데이트 대기중 목록
        /// </summary>
        /// <param name="delayMinute"></param>
        /// <returns></returns>
        public async Task<List<SmsReceiveLogDto>> SearchUnprocessedReceiveLogsForStatistics(int delayMinute)
        {
            DateTime currentLocalDateTime = _timeManager.GetNow().AddMinutes(delayMinute);
            string hospitalId = _callContext.HospitalId;
            string tenantId = _callContext.TenantId;

            var query = from smsReceiveLog in _notificationCenterContext.SmsReceiveLogs
                        join smsSendlog in _notificationCenterContext.SmsSendLogs
                        on smsReceiveLog.MessageDispatchItemId equals smsSendlog.MessageDispatchItemId
                        where smsReceiveLog.SmsId != "" && string.IsNullOrEmpty(smsReceiveLog.StatusName)
                        && !string.IsNullOrEmpty(smsReceiveLog.MessageId)
                        && smsSendlog.SmsProgressStatus != SmsProgressStatus.Error
                        && smsReceiveLog.RequestTime <= currentLocalDateTime
                        && smsReceiveLog.TenantId == tenantId
                        && smsReceiveLog.HospitalId == hospitalId
                        //0122
                        && smsSendlog.TenantId == tenantId
                        && smsSendlog.HospitalId == hospitalId
                        select new SmsReceiveLogDto()
                        {
                            Id = smsReceiveLog.Id,
                            TenantId = smsReceiveLog.TenantId,
                            HospitalId = smsReceiveLog.HospitalId,
                            Mobile = smsReceiveLog.Mobile,
                            Content = smsReceiveLog.Content,
                            ContentType = smsReceiveLog.ContentType,
                            CountryCode = smsReceiveLog.CountryCode,
                            IsSuccess = smsReceiveLog.IsSuccess,
                            IsAgreeToUsePrivacyData = smsReceiveLog.IsAgreeToUsePrivacyData,
                            Name = smsReceiveLog.Name,
                            RequestTime = smsReceiveLog.RequestTime,
                            CompleteTime = smsReceiveLog.CompleteTime,
                            SentTimeStamp = smsReceiveLog.SentTimeStamp,
                            SmsId = smsReceiveLog.SmsId,
                            SmsRecipientType = smsReceiveLog.SmsRecipientType,
                            MessageDispatchItemId = smsReceiveLog.MessageDispatchItemId,
                            MessageId = smsReceiveLog.MessageId,
                            StatusCode = smsReceiveLog.StatusCode,
                            StatusName = smsReceiveLog.StatusName,
                            StatusMessage = smsReceiveLog.StatusMessage,
                            telcoCode = smsReceiveLog.telcoCode,
                            ActorId = smsReceiveLog.ActorId,
                            SenderId =smsSendlog.SenderId,
                            SentTimeStampUtcPack = smsReceiveLog.SentTimeStampUtcPack
                        };

            
            var list = await query.ToListAsync().ConfigureAwait(false);

            
            return list;
        }

        /// <summary>
        /// block된 번호인지체크 ( 환자정보와 연계해서 처리)
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<List<SmsReceiveLog>> SearchFilteringReceiveLog(string messageDispatchItemId)
        {
            
           string tenantId = _callContext.TenantId;
           string hospitalId = _callContext.HospitalId;

            var query = from receiveLog in _notificationCenterContext.SmsReceiveLogs
                        join patientInfo in _notificationCenterContext.Query<PatientReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on receiveLog.ActorId equals patientInfo.Id into receiveLogProjection
                        from receiveLogPj in receiveLogProjection.DefaultIfEmpty()
                        where receiveLog.TenantId == tenantId
                        && receiveLog.HospitalId == hospitalId
                        && receiveLog.MessageDispatchItemId == messageDispatchItemId
                        select new SmsReceiveLog()
                        {
                            ActorId = receiveLog.ActorId,
                            CompleteTime = receiveLog.CompleteTime,
                            Content = receiveLog.Content,
                            ContentType = receiveLog.ContentType,
                            CountryCode = receiveLog.CountryCode,
                            DataFirstRegisteredDateTimeUtc = receiveLog.DataFirstRegisteredDateTimeUtc,
                            DataLastModifiedDateTimeUtc = receiveLog.DataLastModifiedDateTimeUtc,
                            HospitalId = receiveLog.HospitalId,
                            IsAgreeToUsePrivacyData =receiveLog.IsAgreeToUsePrivacyData,
                            
                            IsSuccess = receiveLog.IsSuccess,
                            MergingPatientGrounds = receiveLog.MergingPatientGrounds,
                            MessageDispatchItemId = receiveLog.MessageDispatchItemId,
                            MessageId = receiveLog.MessageId,
                            Mobile = receiveLog.Mobile,
                            Name = receiveLog.Name,
                            PatientContactClassificationCode = receiveLog.PatientContactClassificationCode,
                            PatientContactRelationShipCode = receiveLog.PatientContactRelationShipCode,
                            RequestTime = receiveLog.RequestTime,
                            SentTimeStamp = receiveLog.SentTimeStamp,
                            SentTimeStampUtcPack = receiveLog.SentTimeStampUtcPack,
                            SmsId = receiveLog.SmsId,
                            SmsRecipientType = receiveLog.SmsRecipientType,
                            StatusCode = receiveLog.StatusCode,
                            StatusMessage = receiveLog.StatusMessage,
                            StatusName = receiveLog.StatusName,
                            telcoCode = receiveLog.telcoCode,
                            TenantId = receiveLog.TenantId,
                            Trace = receiveLog.Trace,
                            IsBlocked = receiveLogPj.isClosed ?? false 
                        };

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list;
        }

        /// <summary>
        /// 병원정보 가져오기
        /// </summary>
        /// <param name="hospitalId"></param>
        /// <returns></returns>
        public async Task<HospitalReadModel> GetHospitalInfo(string hospitalId)
        {
            var query = from hospitalInfo in _notificationCenterContext.Query<HospitalReadModel>()
                        where hospitalInfo.Id == hospitalId
                        select new HospitalReadModel()
                        {
                            Id = hospitalInfo.Id,
                            Abbreviation = hospitalInfo.Abbreviation,
                            AddressContent = hospitalInfo.AddressContent,
                            Name = hospitalInfo.Name,
                            RegionId = hospitalInfo.RegionId
                        };
            var returnRow = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            return returnRow;
        }
    }
}
