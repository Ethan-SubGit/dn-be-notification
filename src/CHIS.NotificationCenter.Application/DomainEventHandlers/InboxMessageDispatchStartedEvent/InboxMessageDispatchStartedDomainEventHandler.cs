using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.Events;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.Queries.ReadModels.MessageDispatcher;
using System.Threading;
using System.Linq;
using System.Collections;
using CHIS.NotificationCenter.Application.Models.ProxyModels.HospitalBuilder;
using CHIS.NotificationCenter.Application.Proxies;
using CHIS.NotificationCenter.Application.Queries;

using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.Framework.Core;
using CHIS.Framework.Core.Localization;
using CHIS.Framework.Middleware;
//using CHIS.Share.NotificationCenter.Enum;
//using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using Newtonsoft.Json;

namespace CHIS.NotificationCenter.Application.DomainEventHandlers.InboxMessageDispatchStartedEvent
{
    public class InboxMessageDispatchStartedDomainEventHandler : INotificationHandler<InboxMessageDispatchStartedDomainEvent>
    {

        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly IMessageSpecificationQueries   _messageSpecificationQueries;
        private readonly IEmployeeMessageBoxRepository   _employeeMessageBoxRepository;
        private readonly IMessageDispatcherQueries       _messageDispatcherQueries;
        private readonly IOneSignalInterfaceService      _oneSignalInterfaceService;
        private readonly ILocalizationManager            _localizationManager;
        //private readonly ISmsService _smsService;
        private readonly IUtcService _utcService;
        private readonly ITimeManager _timeManager;
        private readonly ICallContext _callContext;
        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        private readonly ISmsMonitoringQueries _smsMonitoringQueries;
        //private readonly IHospitalBuilderProxy _hospitalBuilderProxy;

        public InboxMessageDispatchStartedDomainEventHandler(
            IMessageSpecificationRepository messageSpecificationRepository,
            IMessageSpecificationQueries messageSpecificationQueries,
            IEmployeeMessageBoxRepository   employeeMessageBoxRepository, 
            IMessageDispatcherQueries       messageDispatcherQueries,
            IOneSignalInterfaceService      oneSignalInterfaceService,
            ILocalizationManager            localizationManager,
            //ISmsService smsService,
            ICallContext callContext,
            IUtcService utcService,
            ITimeManager timeManager,
            ISmsMonitoringRepository smsMonitoringRepository,
            ISmsMonitoringQueries smsMonitoringQueries
            //IHospitalBuilderProxy hospitalBuilderProxy
            )
        {
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _messageSpecificationQueries = messageSpecificationQueries ?? throw new ArgumentNullException(nameof(messageSpecificationQueries));
            _employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            _messageDispatcherQueries = messageDispatcherQueries ?? throw new ArgumentNullException(nameof(messageDispatcherQueries));
            _oneSignalInterfaceService = oneSignalInterfaceService ?? throw new ArgumentNullException(nameof(oneSignalInterfaceService));
            _localizationManager = localizationManager ?? throw new ArgumentNullException(nameof(localizationManager));
            //_smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            _smsMonitoringQueries = smsMonitoringQueries ?? throw new ArgumentNullException(nameof(smsMonitoringQueries));
            //_hospitalBuilderProxy = hospitalBuilderProxy ?? throw new ArgumentNullException(nameof(hospitalBuilderProxy));
        }

        public async Task Handle(InboxMessageDispatchStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            // TO-DO : Inbox Message 처리시 SMS 발송여부 체크하여 처리하는 부분 
            //                           EmployeeId       = EM.Id
            //,  EmployeeName = EM.FullName
            //,  Mobile = EM.Mobile
            // TO-DO : 직원부터 리스트 업하고 전화번호를 찾는 구조로 바꿀것. 직원의 부서정보 및 Occupation, Jobposition이 없으면 Inbox 메시지가 발송안됨
            //         수신직원 리스트업 하는부분 로직 체크할것.
            //         SMS연동시 직원 전화번호 없으면 에러발생. 내용 확인 할것. => 조치 완료.


            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            //DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            //DateTime? reservedSmsDateTime = currentLocalDateTime;

            #region ## 수신자, 송신자 정보입력
            // DESC : 수신정책 포함하여 수신자 추출
            var EmployeeList = await _messageDispatcherQueries.GetEmployeeRecipients(notification.MessageDispatchItem).ConfigureAwait(false);

            var employeesForNotification = EmployeeList.Select(i => new EmployeeRecipientDto()
            {
                EmployeeId = i.EmployeeId,
                EmployeeName = i.EmployeeName,
                Mobile = i.Mobile,
                Inbound = true
            }).ToList();

            // DESC : 수신자 EmployeeId 중복제거
            employeesForNotification = employeesForNotification.Distinct().ToList();

            // DESC : 보낸사람 sentbox 처리
            // TODO : System 사용자 처리 로직
            if (!string.IsNullOrEmpty(notification.MessageDispatchItem.SenderId) && notification.MessageDispatchItem.SenderId != "System")
            {
                employeesForNotification.Add(new EmployeeRecipientDto { EmployeeId = notification.MessageDispatchItem.SenderId, EmployeeName = string.Empty, Mobile = string.Empty, Inbound = false });
            } 
            #endregion


            // DESC : EmployeeMessageBox Distinct 하게 리스트업 후 EmployeeMessageBox별 Message 생성.
            List<string> Employees = employeesForNotification.Select(i => i.EmployeeId).Distinct().ToList();
            List<EmployeeMessageBox> employeeMessageBoxes = await _employeeMessageBoxRepository.Retrieve(Employees).ConfigureAwait(false);

            MessageSpecification messageSpecification =
                _messageSpecificationRepository.FindByServiceCode(notification.MessageDispatchItem.ServiceCode);

            string content = (notification.MessageDispatchItem.IsUsingPredefinedContent) ? messageSpecification.PredefinedContent : notification.MessageDispatchItem.Content;

            #region ## content replace
            if (notification.MessageDispatchItem.ContentParameters.Count > 0)
            {
                var replaceArr = notification.MessageDispatchItem.ContentParameters.Select(p => p.ParameterValue).ToArray();

                content = string.Format(content, replaceArr);

            } 
            #endregion

            //var list = notification.MessageDispatchItem.ContentParameters.Select(i => i.ParameterValue.ToString());

            // DESC : CommunicationNote에서 SMS 만 보낼경우 처리
            if (
                    notification.MessageDispatchItem.ServiceType == NotificationServiceType.Inbox 
                    ||
                    (
                        notification.MessageDispatchItem.ServiceType == NotificationServiceType.CommunicationNote
                        && notification.MessageDispatchItem.CommunicationNoteMessageDeliveryOption != CommunicationNoteMessageDeliveryOption.SmsOnly
                    )
                )
            {
                #region ## send to target employee (송신자와 수신자 모두)
                foreach (var employee in employeesForNotification)
                {
                    bool isEmployeeMessageBoxExist = true;
                   
                    EmployeeMessageBox employeeMessageBox = employeeMessageBoxes.FirstOrDefault(a =>
                        a.TenantId == tenantId
                        && a.HospitalId == hospitalId
                        && a.EmployeeId == employee.EmployeeId);

                    if (employeeMessageBox == null)
                    {
                        isEmployeeMessageBoxExist = false;
                        employeeMessageBox = new EmployeeMessageBox(
                            tenantId,
                            hospitalId,
                            employee.EmployeeId,
                            _timeManager.GetUTCNow()
                            );
                    }


                    employeeMessageBox.EmployeeMessageInstances.Add(new EmployeeMessageInstance()
                    {
                        IsInbound = employee.Inbound,
                        MessageDispatchItemId = notification.MessageDispatchItem.Id,
                        Title = notification.MessageDispatchItem.Title,
                        Content = content,
                        TenantId = tenantId,
                        HospitalId = hospitalId,
                        EmployeeId = employee.EmployeeId,
                        SentTimeStamp = notification.MessageDispatchItem.SentTimeStamp,
                        ServiceCode = notification.MessageDispatchItem.ServiceCode,
                        DataFirstRegisteredDateTimeUtc = _timeManager.GetUTCNow()
                    });

                    //메시지박스 사서함
                    if (isEmployeeMessageBoxExist)
                    {
                        //업데이트 시간
                        employeeMessageBox.DataLastModifiedDateTimeUtc = _timeManager.GetUTCNow();
                        _employeeMessageBoxRepository.Update(employeeMessageBox);
                    }
                    else
                    {
                        _employeeMessageBoxRepository.Create(employeeMessageBox);
                    }
                } 
                #endregion


                await SendOneSignalPushMessage(notification, messageSpecification.MessageCategory, EmployeeList).ConfigureAwait(false);
            }
 

            await SendSmsMessage(notification
                //, messageSpecification.MessageCategory
                , messageSpecification.IsForceToSendInboxSmsMessage, content, employeesForNotification).ConfigureAwait(false);
            
            #region 인박스/쪽지 원시그널 푸쉬 연동 인터페이스

            // TO-DO : One-Signal Push Notification 로직 확인. (인박스 /쪽지 구분)
            //string pushNotificationTitle = string.Empty;
            //string pushNotificationBody = string.Empty;
            //Dictionary<string, string> dic = new Dictionary<string, string>();

            //if (notification.MessageDispatchItem.ServiceType == NotificationServiceType.Inbox)
            //{
            //    pushNotificationTitle = $"{_localizationManager.GetShortName("tempKey", "새 인박스 메시지")}({GetMessageCategoryName(messageSpecification.MessageCategory)})";
            //    pushNotificationBody = _localizationManager.GetShortName("tempKey", "새 인박스 메시지가 도착했습니다.");
            //    dic["serviceType"] = "Inbox";

            //}
            //else if (notification.MessageDispatchItem.ServiceType == NotificationServiceType.CommunicationNote)
            //{
            //    pushNotificationTitle = _localizationManager.GetShortName("tempKey", "새 쪽지 메시지");
            //    pushNotificationBody = _localizationManager.GetShortName("tempKey", "새 쪽지 메시지가 도착했습니다.");
            //    dic["serviceType"] = "communication-note";

            //}
            //dic["patientId"] = notification.MessageDispatchItem.PatientId;
            //dic["messageCategory"      ] = messageSpecification.MessageCategory;
            //dic["messageProcessedYN"   ] = "N";
            //dic["messageDispatchItemId"] = notification.MessageDispatchItem.Id;


            //await _oneSignalInterfaceService.SendMessage(
            //    PushMessageType.NewMessage, pushNotificationTitle, pushNotificationBody, dic, EmployeeList.Select(i => i.EmployeeId).ToList()).ConfigureAwait(false);

            #endregion

            #region Inbox SMS 연동
            // TO-DO : Inbox에서 Sms 전송 처리 확인.
            // Inbox 전송시 sms 포함 할것인지에 대한 attribute 추가 할것.

            // TO-DO : 직원부터 리스트 업하고 전화번호를 찾는 구조로 바꿀것.
            //if (notification.MessageDispatchItem.ServiceType == NotificationServiceType.Inbox && messageSpecification.IsForceToSendInboxSmsMessage)
            //{

            //    if (notification.MessageDispatchItem.SmsContentByInbox.Trim().Length > 0)
            //    {
            //        content = notification.MessageDispatchItem.SmsContentByInbox;
            //    }

            //    List<SmsRecipientDto> smsRecipientList = employeesForNotification.Where(i => i.Inbound).Select(i => new SmsRecipientDto()
            //    {
            //        SmsRecipientType = Domain.Enum.SmsRecipientType.Employee,
            //        Mobile = i.Mobile,
            //        Name = i.EmployeeName,
            //        ActorId = i.EmployeeId
                    
            //    }).ToList();

            //    _smsMonitoringRepository.CreateSmsSendLog(new SmsSendLog(
            //        tenantId: _callContext.TenantId
            //       ,hospitalId: _callContext.HospitalId 
            //       ,content: content
            //       ,callingNumber: string.Empty //"027478640" 삭제검토
            //       ,isReservedSms: notification.MessageDispatchItem.IsReservedSms
            //       ,reservedTime: reservedSmsDateTime
            //       , executionTime: currentLocalDateTime
            //       , smsProgressStatus: SmsProgressStatus.BeforeProgress
            //       ,messageDispatchItemId: notification.MessageDispatchItem.Id
            //       ,callStatusCode: string.Empty
            //       ,senderId: notification.MessageDispatchItem.SenderId
            //       ,smsTraceId: string.Empty
            //       ,errorMessage: string.Empty 
            //       ,smsRecipientType: Domain.Enum.SmsRecipientType.Employee
            //       , serviceCode: notification.MessageDispatchItem.ServiceCode
            //       ));

            //    foreach (SmsRecipientDto smsRecipient in smsRecipientList)
            //    {
            //        _smsMonitoringRepository.CreateSmsReceiveLog(new SmsReceiveLog(
            //             tenantId: _callContext.TenantId
            //            , hospitalId: _callContext.HospitalId
            //            , smsRecipientType: smsRecipient.SmsRecipientType
            //            , name: smsRecipient.Name == null ? "" : smsRecipient.Name
            //            , mobile: smsRecipient.Mobile == null ? "" : smsRecipient.Mobile.Trim().Replace("-", "")
            //            ,content: content
            //            ,isSuccess: false
            //            , isAgreeToUsePrivacyData : true
            //            //, sentTimeStamp: currentLocalDateTime
            //            //, sentTimeStampUtcPack: _utcService.GetUtcPack(currentLocalDateTime)
            //            , messageDispatchItemId: notification.MessageDispatchItem.Id
            //            , actorId: smsRecipient.ActorId
            //            , patientContactRelationShipCode : string.Empty
            //            , patientContactClassificationCode : string.Empty
            //        ));
            //    }
            //}
            #endregion
        }


        private string GetMessageCategoryName(string messageCategory)
        {
            switch (messageCategory)
            {
                // TO-DO : 다국어 적용 확인 할것by Tommy 2/26
                case "D": return _localizationManager.GetShortName("tempKey", "기록");//"문서";
                case "R": return _localizationManager.GetShortName("tempKey", "검사결과"); //"검사결과";
                case "O": return _localizationManager.GetShortName("tempKey", "오더"); //"오더";
                default: return "";
            }

        }

        #region #### SendOneSignalPushMessage
        /// <summary>
        /// SendOneSignalPushMessage
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="messageCategory"></param>
        /// <param name="employeeRecipients"></param>
        /// <returns></returns>
        private async Task SendOneSignalPushMessage(InboxMessageDispatchStartedDomainEvent notification
            , string messageCategory
            , IList<Queries.ReadModels.MessageDispatcher.EmployeeRecipientView> employeeRecipients)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            string pushNotificationTitle = string.Empty;
            string pushNotificationBody = string.Empty;
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (messageCategory == "E")
            {
                pushNotificationTitle = _localizationManager.GetShortName("tempKey", "새 쪽지 메시지");
                pushNotificationBody = _localizationManager.GetShortName("tempKey", "새 쪽지 메시지가 도착했습니다.");
                //dic["serviceType"] = "communication-note";

            }
            else
            {

                pushNotificationTitle = $"{_localizationManager.GetShortName("tempKey", "새 인박스 메시지")}({GetMessageCategoryName(messageCategory)})";
                pushNotificationBody = _localizationManager.GetShortName("tempKey", "새 인박스 메시지가 도착했습니다.");
                //dic["serviceType"] = "Inbox";
            }

            dic["patientId"] = notification.MessageDispatchItem.PatientId;
            dic["messageCategory"] = messageCategory;
            dic["messageProcessedYN"] = "N";
            dic["messageDispatchItemId"] = notification.MessageDispatchItem.Id;


            await _oneSignalInterfaceService.SendMessage(pushNotificationTitle, pushNotificationBody, dic, employeeRecipients.Select(i => i.EmployeeId).ToList()).ConfigureAwait(false);

        }
        #endregion


        #region #### SendSmsMessage
        /// <summary>
        /// SendSmsMessage
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="messageCategory"></param>
        /// <param name="isForceToSendInboxSmsMessage"></param>
        /// <param name="resolvedContent"></param>
        /// <param name="employeeRecipients"></param>
        /// <returns></returns>
        private async Task SendSmsMessage(InboxMessageDispatchStartedDomainEvent notification
            //, string messageCategory
            , bool isForceToSendInboxSmsMessage
            , string resolvedContent
            , IList<EmployeeRecipientDto> employeeRecipients)
        {
            
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            DateTime? reservedSmsDateTime = currentLocalDateTime;
            DateTime currentUtcDateTime = _timeManager.GetUTCNow();
            string content = resolvedContent;

            // DESC 1 : INBOX SMS 연동
            //          MessageSpecification에서 SMS 연동 체크했을 경우 SMS 발송.
            // DESC 2 : Communication Note에서 SMS 연동
            if ((notification.MessageDispatchItem.ServiceType == NotificationServiceType.Inbox && isForceToSendInboxSmsMessage)
                ||
                (notification.MessageDispatchItem.ServiceType == NotificationServiceType.CommunicationNote
                    && notification.MessageDispatchItem.CommunicationNoteMessageDeliveryOption != CommunicationNoteMessageDeliveryOption.MessageOnly)
                )
            {

                if (!string.IsNullOrEmpty(notification.MessageDispatchItem.SmsContentByInbox) && notification.MessageDispatchItem.SmsContentByInbox.Trim().Length > 0)
                {
                    content = notification.MessageDispatchItem.SmsContentByInbox;
                }

                List<SmsRecipientDto> smsRecipientList = employeeRecipients.Where(i => i.Inbound).Select(i => new SmsRecipientDto()
                {
                    SmsRecipientType = Domain.Enum.SmsRecipientType.Employee,
                    Mobile = i.Mobile,
                    Name = i.EmployeeName,
                    ActorId = i.EmployeeId

                }).ToList();

                #region 수신전화번호 조회
                //수신전화번호 조회
                string callingNumber = string.Empty;

                //[1] serviceCode로 회신번호 조회
                callingNumber = await _messageSpecificationQueries.GetCallbackNoByServiceCode(notification.MessageDispatchItem.ServiceCode).ConfigureAwait(false);
                //[2] 회신번호가 없는경우 병원 대표번호 조회
                //HospitalReadModel hospitalModel = await _smsMonitoringQueries.GetHospitalInfo(hospitalId: _callContext.HospitalId).ConfigureAwait(false);
                //if (hospitalModel != null)
                //{
                //    AddressContentDto addressDto = JsonConvert.DeserializeObject<AddressContentDto>(hospitalModel.AddressContent);
                //    callingNumber = addressDto.BusinessPhoneNumber.Replace("-", "", StringComparison.Ordinal);
                //}
                #endregion

                #region ## SMS SendLog save
                _smsMonitoringRepository.CreateSmsSendLog(new SmsSendLog(
                           tenantId: _callContext.TenantId
                          , hospitalId: _callContext.HospitalId
                          , content: content
                          , callingNumber: callingNumber
                          , isReservedSms: notification.MessageDispatchItem.IsReservedSms
                          , reservedTime: reservedSmsDateTime
                          , executionTime: currentLocalDateTime
                          , smsProgressStatus: SmsProgressStatus.BeforeProgress
                          , messageDispatchItemId: notification.MessageDispatchItem.Id
                          , callStatusCode: string.Empty
                          , senderId: notification.MessageDispatchItem.SenderId
                          , smsTraceId: string.Empty
                          , errorMessage: string.Empty
                          , smsRecipientType: Domain.Enum.SmsRecipientType.Employee
                          , serviceCode: notification.MessageDispatchItem.ServiceCode
                          , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                          , dataLastModifiedDateTimeUtc: currentUtcDateTime
                          , trace: null
                          ));

                foreach (SmsRecipientDto smsRecipient in smsRecipientList)
                {
                    _smsMonitoringRepository.CreateSmsReceiveLog(new SmsReceiveLog(
                         tenantId: _callContext.TenantId
                        , hospitalId: _callContext.HospitalId
                        , smsRecipientType: smsRecipient.SmsRecipientType
                        , name: smsRecipient.Name == null ? "" : smsRecipient.Name
                        , mobile: smsRecipient.Mobile == null ? "" : smsRecipient.Mobile.Trim().Replace("-", "", StringComparison.CurrentCultureIgnoreCase)
                        , content: content
                        , isSuccess: false
                        , isAgreeToUsePrivacyData: true
                        , messageDispatchItemId: notification.MessageDispatchItem.Id
                        , actorId: smsRecipient.ActorId
                        , patientContactRelationShipCode: string.Empty
                        , patientContactClassificationCode: string.Empty
                        , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                        , dataLastModifiedDateTimeUtc: currentUtcDateTime
                        , trace: null
                    ));
                }
                #endregion
            }

        }
        #endregion

    }
}
