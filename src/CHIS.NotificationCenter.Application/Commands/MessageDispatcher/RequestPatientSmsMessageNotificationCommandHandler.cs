using CHIS.Framework.Core;
using CHIS.Framework.Core.Claims;
using CHIS.Framework.Core.Configuration;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Enum;
//using CHIS.Share.NotificationCenter.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Application.Proxies;
using CHIS.Share.AuditTrail.Services;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    /// <summary>
    /// SMS 발송
    /// </summary>
    public class RequestPatientSmsMessageNotificationCommandHandler : IRequestHandler<RequestPatientSmsMessageNotificationCommand, bool>
    {
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly IMessageSpecificationQueries _messageSpecificationQueries;
        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        private readonly IPatientInformationQueries _patientInformationQueries;
        private readonly ISmsMonitoringQueries _smsMonitoringQueries;
        //private readonly IAccessControlProxy _accessControlProxy;
        private readonly IUtcService _utcService;
        private readonly ITimeManager _timeManager;
        private readonly ICallContext _callContext;
        //private readonly ISmsService _smsService;
        //private readonly ILoggingService _loggingService;
        //private readonly Share.NotificationCenter.Services.ISmsSendService _smsSendService;

        // Using DI to inject infrastructure persistence Repositories
        public RequestPatientSmsMessageNotificationCommandHandler(
            IMessageDispatchItemRepository messageDispatchItemRepository
            , IMessageSpecificationRepository messageSpecificationRepository
            , IMessageSpecificationQueries messageSpecificationQueries
            //, IAccessControlProxy accessControlProxy
            , ISmsMonitoringRepository smsMonitoringRepository
            , IPatientInformationQueries patientInformationQueries
            , ISmsMonitoringQueries smsMonitoringQueries
            , ICallContext callContext
            , IUtcService utcService
            , ITimeManager timeManager

            )
        {
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _messageSpecificationQueries = messageSpecificationQueries ?? throw new ArgumentException(nameof(messageSpecificationQueries));
            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            _patientInformationQueries = patientInformationQueries ?? throw new ArgumentNullException(nameof(patientInformationQueries));
            _smsMonitoringQueries = smsMonitoringQueries ?? throw new ArgumentNullException(nameof(smsMonitoringQueries));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));

        }

        public async Task<bool> Handle(RequestPatientSmsMessageNotificationCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            DateTime currentUtcDateTime = _timeManager.GetUTCNow();

           


            #region message specification 조회
            //수신된 메세지 내역중 서비스코드와 컨텐츠 값을 미리 조회.
            //smsMessages의 서비스코드와 컨텐츠는 동일하다고 전제.
            #region message format
            /*
                {
                    "smsMessages": [
                    {
                        "serviceCode": "SMS00001",
                            "senderId": "GUID_10001",
                            "isUsingPredefinedContent": false,
                            "content": "SMS 박보검 TEST",
                            "isReservedSms": false,
                            "reservedSmsDateTime": "",
                            "patientId": "6502031a-2849-4fba-b502-b496eb71ad52",
                            "smsRecipientType": "PatientDirectMobile",
                            "contactRelationShipCode": "99",
                            "contactClassificationCode": "",
                            "mobile": "0100000001",
                            "contentParameters": []
                    },
                   {
                        "serviceCode": "SMS00001",
                        "senderId": "GUID_10001",
                        "isUsingPredefinedContent": false,
                        "content": "SMS 박보검 TEST",
                        "isReservedSms": false,
                        "reservedSmsDateTime": "",
                        "patientId": "6502031a-2849-4fba-b502-b496eb71ad52",
                        "smsRecipientType": "PatientDirectMobile",
                        "contactRelationShipCode": "99",
                        "contactClassificationCode": "",
                        "mobile": "0100000002",
                        "contentParameters": []
    }
                    ]
                }
                */
            #endregion

            var requestMessageSpecificationRow = request.SmsMessages.FirstOrDefault();
            //string serviceCode = (string.IsNullOrEmpty(requestMessageSpecificationRow.ServiceCode)) ? "SMS00001" : requestMessageSpecificationRow.ServiceCode;
            string serviceCode = requestMessageSpecificationRow.ServiceCode ?? "SMS0001";
            bool isUsingPredefinedContent = requestMessageSpecificationRow.IsUsingPredefinedContent;
            var messageSpecificationRow = _messageSpecificationRepository.FindByServiceCode(serviceCode);

            #region ## 발신번호 조회
            string callingNumber = "027478640";
            #region #병원정보에서 조회(현재 미사용)
            //HospitalReadModel hospitalModel = await _smsMonitoringQueries.GetHospitalInfo(hospitalId: _callContext.HospitalId).ConfigureAwait(false);
            //if (hospitalModel != null)
            //{
            //    AddressContentDto addressDto = JsonConvert.DeserializeObject<AddressContentDto>(hospitalModel.AddressContent);
            //    callingNumber = addressDto.BusinessPhoneNumber.Replace("-", "", StringComparison.Ordinal);
            //}
            //string serviceCode = request.SmsMessages.First().ServiceCode.Trim().ToString(); 
            #endregion
            callingNumber = await _messageSpecificationQueries.GetCallbackNoByServiceCode(serviceCode).ConfigureAwait(false);
            #endregion


            string content = getMessageContent(requestMessageSpecificationRow, isUsingPredefinedContent, messageSpecificationRow);
            #endregion

            DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            foreach (var smsMessage in request.SmsMessages)
            {

                DateTime? reservedSmsDateTime = smsMessage.ReservedSmsDateTime ?? currentLocalDateTime;


                MessageDispatchItem messageDispatchItem = new MessageDispatchItem(
                    tenantId: tenantId
                    , hospitalId: hospitalId
                    , serviceType: Domain.Enum.NotificationServiceType.SMS
                    //, serviceCode: smsMessage.ServiceCode
                    , serviceCode: serviceCode
                    , senderId: smsMessage.SenderId
                    , isUsingPredefinedContent: false
                    //, content: smsMessage.Content
                    , content: content
                    , encounterId: string.Empty
                    , patientId: smsMessage.PatientId ?? ""
                    , sentTimeStamp: currentLocalDateTime
                    , sentTimeStampUtcPack: _utcService.GetUtcPack(currentLocalDateTime)
                    , isReservedSms: smsMessage.IsReservedSms
                    , reservedSmsDateTime: reservedSmsDateTime
                    , reservedSmsDateTimeUtcPack: _utcService.GetUtcPack(reservedSmsDateTime)
                    , referenceId: string.Empty
                    , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                    , dataLastModifiedDateTimeUtc: currentUtcDateTime
                    , trace: null
                    );


                setMessageContent(tenantId, hospitalId, currentUtcDateTime, smsMessage, messageDispatchItem);


                // DESC : 지정된 patient 수신자를 넣어줘야될거 같음.
                // messageDispatchItem.AddAssignedPatientSmsRecipient()

                string messageDispatchItemId = _messageDispatchItemRepository.Create(messageDispatchItem).Id;

                // DESC : SMS 전송을 위해 전화번호 정보 가져옴.
                //        전화번호가 없을경우 환자정보만 있음.
                SmsRecipientDto smsRecipient = null;
                smsRecipient = await getRecipient(smsMessage).ConfigureAwait(false);

                // DESC : 로직 한번더 정리할것.
                string mobile = string.Empty;
                //string recipientName = string.Empty;
                bool isAgreeToUsePrivacyData = false;

                #region 전송대상자
                if (smsRecipient == null)
                {
                    continue;
                }

                switch (smsMessage.SmsRecipientType)
                {
                    case SmsRecipientType.Guardian:
                    case SmsRecipientType.Patient:
                        //mobile = smsRecipient.Mobile == null ? "" : smsRecipient.Mobile.Trim().Replace("-", "", StringComparison.Ordinal);
                        mobile = smsRecipient.Mobile == null ? "" : Regex.Replace(smsRecipient.Mobile.Trim(), @"[^0-9]", "");
                        /*******************
                         * SMS수신동의여부 처리필요
                         * 수신동의여부는 실제 batch 로 전송하기전 단계에서 체크하므로 이 단계에서는 
                         * 동의여부를 체크할 필요없음
                        */
                        /*
                       isAgreeToUsePrivacyData = _accessControlProxy.GetPatientOfAgreeToUsePrivacyData(smsMessage.PatientId);
                       */
                        isAgreeToUsePrivacyData = true;
                        break;
                    default: // 전화번호 직접입력 케이스
                        mobile = smsMessage.Mobile;
                        isAgreeToUsePrivacyData = true;
                        break;
                }

                #region 전송문구
                if (smsMessage.IsUsingPredefinedContent)
                {
                    var messageSpecification = _messageSpecificationRepository.FindByServiceCode(smsMessage.ServiceCode);
                    smsMessage.Content = messageSpecification.PredefinedContent;
                }
                if (smsMessage.ContentParameters != null && smsMessage.ContentParameters.Count > 0)
                {
                    smsMessage.Content = string.Format(smsMessage.Content, smsMessage.ContentParameters.Select(i => i.ParameterValue).ToArray());

                } 
                #endregion

                //sendLog, receiveLog 입력
                saveSMSMessage(currentUtcDateTime, smsMessage, currentLocalDateTime, reservedSmsDateTime
                    , messageDispatchItemId, smsRecipient, mobile, isAgreeToUsePrivacyData, callingNumber);
                #endregion




                // 배치에서 sms 전송하는것이 코드 role 입장에서는 좋음.
                // 배치에서 전송하도록 변경처리 (2019/12/20)
                //if (!smsMessage.IsReservedSms)
                //{
                //    await _smsService.SendSmsMessage(messageDispatchItem.Id).ConfigureAwait(false);
                //}

            }
            await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);
            return true;
        }

       
        /// <summary>
        /// sendLog, receiveLog 입력
        /// </summary>
        /// <param name="currentUtcDateTime"></param>
        /// <param name="smsMessage"></param>
        /// <param name="currentLocalDateTime"></param>
        /// <param name="reservedSmsDateTime"></param>
        /// <param name="messageDispatchItemId"></param>
        /// <param name="smsRecipient"></param>
        /// <param name="mobile"></param>
        /// <param name="isAgreeToUsePrivacyData"></param>
        private void saveSMSMessage(DateTime currentUtcDateTime, PatientSmsMessageDto smsMessage
            , DateTime currentLocalDateTime, DateTime? reservedSmsDateTime, string messageDispatchItemId
            , SmsRecipientDto smsRecipient, string mobile, bool isAgreeToUsePrivacyData, string callingNumber)
        {
            //발신번호나 수신번호가 잘못된 경우, 발송이 되지않도록 입력
            string _errorMessage = string.Empty;
            string _callStatusCode = string.Empty;
            string _statusCode = string.Empty;
            string _statusMessage = string.Empty;
            var _smsProgressStatus = SmsProgressStatus.BeforeProgress;
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(callingNumber))
            {
                _errorMessage = "Phone Number is not found";
                _callStatusCode = "404-1";
                _statusCode = "-1";
                _statusMessage = "Phone Number is not found";
                _smsProgressStatus = SmsProgressStatus.Error;

            }

            _smsMonitoringRepository.CreateSmsSendLog(
                new SmsSendLog(
                    tenantId: _callContext.TenantId
                    , hospitalId: _callContext.HospitalId
                    , content: smsMessage.Content
                    , callingNumber: callingNumber // DESC : 삭제검토
                    , isReservedSms: smsMessage.IsReservedSms
                    , reservedTime: smsMessage.IsReservedSms ? reservedSmsDateTime : currentLocalDateTime
                    , executionTime: currentLocalDateTime
                    //, smsProgressStatus: smsMessage.IsReservedSms ? SmsProgressStatus.BeforeProgress : SmsProgressStatus.InProgress
                    //, smsProgressStatus: SmsProgressStatus.BeforeProgress
                    , smsProgressStatus: _smsProgressStatus
                    , messageDispatchItemId: messageDispatchItemId// notification.MessageDispatchItem.Id
                    , smsTraceId: string.Empty
                    , callStatusCode: _callStatusCode
                    , errorMessage: _errorMessage
                    , senderId: smsMessage.SenderId
                    , smsRecipientType: SmsRecipientType.Patient // 환자, 보호자 중에 하나 선택할수 있도록 로직 변경.
                    , serviceCode: smsMessage.ServiceCode // SMS Sender 구분?
                    , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                    , dataLastModifiedDateTimeUtc: currentUtcDateTime
                    , trace: null
            ));

            _smsMonitoringRepository.CreateSmsReceiveLog(
                new SmsReceiveLog(
                    tenantId: _callContext.TenantId
                    , hospitalId: _callContext.HospitalId
                    , smsRecipientType: smsRecipient.SmsRecipientType
                    , name: smsRecipient.Name == null ?
                            "" : smsRecipient.Name
                    , mobile: mobile
                    , content: smsMessage.Content
                    , isSuccess: false
                    , isAgreeToUsePrivacyData: isAgreeToUsePrivacyData
                    //, sentTimeStamp: currentLocalDateTime
                    //, sentTimeStampUtcPack: _utcService.GetUtcPack(currentLocalDateTime)
                    , messageDispatchItemId: messageDispatchItemId //notification.MessageDispatchItem.Id
                    , actorId: smsRecipient.ActorId
                    , patientContactRelationShipCode: smsRecipient.PatientContactRelationShipCode
                    , patientContactClassificationCode: smsRecipient.PatientContactClassificationCode
                    , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                    , dataLastModifiedDateTimeUtc: currentUtcDateTime
                    , trace: null
                    , statusMessage: _statusMessage
                    , statusCode: _statusCode
            ));
        }

        /// <summary>
        /// 수신대상자 설정
        /// </summary>
        /// <param name="smsMessage"></param>
        /// <param name="smsRecipient"></param>
        /// <returns></returns>
        private async Task<SmsRecipientDto> getRecipient(PatientSmsMessageDto smsMessage)
        {
            SmsRecipientDto smsRecipient = null;
            if (!string.IsNullOrEmpty(smsMessage.ContactRelationShipCode)
                                && !string.IsNullOrEmpty(smsMessage.ContactClassificationCode)
                                )
            {
                smsRecipient = await _patientInformationQueries.FindPatientContact(patientId: smsMessage.PatientId
                    , relationShipCode: smsMessage.ContactRelationShipCode
                    , classificationCode: smsMessage.ContactClassificationCode)
                 .ConfigureAwait(false);
            }
            else
            {
                smsRecipient = await _patientInformationQueries.FindPatientContact(
                    patientId: smsMessage.PatientId
                    , smsRecipientType: smsMessage.SmsRecipientType)
                    .ConfigureAwait(false);
            }

            return smsRecipient;
        }

        #region sms 전송컨텐츠
        /// <summary>
        /// sms 컨텐츠  생성
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="currentUtcDateTime"></param>
        /// <param name="smsMessage"></param>
        /// <param name="messageDispatchItem"></param>
        private static void setMessageContent(string tenantId, string hospitalId, DateTime currentUtcDateTime, PatientSmsMessageDto smsMessage, MessageDispatchItem messageDispatchItem)
        {
            if (smsMessage.ContentParameters != null)
            {
                foreach (ContentParameterDto contentParameter in smsMessage.ContentParameters)
                {
                    messageDispatchItem.AddContentParameter(new ContentParameter()
                    {
                        ParameterValue = contentParameter.ParameterValue,
                        TenantId = tenantId,
                        HospitalId = hospitalId,
                        DataFirstRegisteredDateTimeUtc = currentUtcDateTime,
                        DataLastModifiedDateTimeUtc = currentUtcDateTime,
                        Trace = null
                    });
                }
            }
        }

        /// <summary>
        /// sms 전송 컨텐츠 
        /// </summary>
        /// <param name="reqMsgSpecRow"></param>
        /// <param name="isDefinedContent"></param>
        /// <param name="msgSpecRow"></param>
        /// <returns></returns>
        private string getMessageContent(PatientSmsMessageDto reqMsgSpecRow, bool isDefinedContent, Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification msgSpecRow)
        {
            string content = (isDefinedContent) ?
                                                    msgSpecRow.PredefinedContent
                                                    : reqMsgSpecRow.Content;

            //추가된 파라미터중 템플릿 id가 있고 사전정의내용 true면 전송할 content를 대체한다.
            if (!string.IsNullOrEmpty(reqMsgSpecRow.MessageTemplateId) && isDefinedContent)
            {
                var templateRow = _messageSpecificationRepository.RetrieveTemplate(reqMsgSpecRow.MessageTemplateId);
                content = templateRow.ContentTemplate;
            }

            var contentParameters = reqMsgSpecRow.ContentParameters;
            if (contentParameters != null && contentParameters.Count > 0)
            {
                var replaceArr = contentParameters.Select(p => p.ParameterValue).ToArray();
                content = string.Format(content, replaceArr);
            }

            return content;
        } 
        #endregion

    }
}
