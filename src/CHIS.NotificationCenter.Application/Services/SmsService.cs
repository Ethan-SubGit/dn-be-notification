using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Models;
using System.Linq;
using CHIS.Framework.Core;
using CHIS.Framework.Core.Localization;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Application.Proxies;
using CHIS.NotificationCenter.Application.Models.ProxyModels.SmsServiceModel;
using CHIS.NotificationCenter.Application.Models.ProxyModels.HospitalBuilder;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.Enum;
using Dapper;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using Newtonsoft.Json;

namespace CHIS.NotificationCenter.Application.Services
{
    /// <summary>
    /// SMS 서비스 정리
    /// TO-DO : 배치 실행시 배치에 해당하는 사항을 명확하게 인지하여 
    ///         또다른 배치에 의해 중복실행되지 않도록 개선 검토
    /// 
    /// </summary>
    public class SmsService : ISmsService
    {
        /// <summary>
        /// 네이버 SMS API를 직접 호출하는 방식에서 TA API를 이용하는 방식으로 변경(2019/3/13)
        /// </summary>
        //private readonly INaverSmsInterfaceService _naverSmsInterfaceService; deprecated
        private readonly ITimeManager _timeManager;
        private readonly ICallContext _callContext;
        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        private readonly ISmsSendProxy _smsSendProxy;
        private readonly IPatientInformationQueries _patientInformationQueries;
        
        private readonly IMessagingService _messagingService;
        private readonly ISmsMonitoringQueries _smsMonitoringQueries;


        /// <summary>
        /// SMS 메시지 발송
        /// </summary>
        /// <param name="callContext"></param>
        /// <param name="timeManager"></param>
        /// <param name="messageSentLogRepository"></param>
        public SmsService(
            ICallContext callContext
            , ITimeManager timeManager
            , ISmsMonitoringRepository smsMonitoringRepository
            , ISmsSendProxy smsSendProxy
            , IPatientInformationQueries patientInformationQueries
            , IMessagingService messagingService
            , ISmsMonitoringQueries smsMonitoringQueries)
        {
            _smsSendProxy = smsSendProxy ?? throw new ArgumentNullException(nameof(smsSendProxy));
            //_naverSmsInterfaceService = naverSmsInterfaceService ?? throw new ArgumentNullException(nameof(naverSmsInterfaceService));
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
           
            _patientInformationQueries = patientInformationQueries ?? throw new ArgumentNullException(nameof(patientInformationQueries));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
            _smsMonitoringQueries = smsMonitoringQueries ?? throw new ArgumentNullException(nameof(smsMonitoringQueries));
        }
        /// <summary>
        /// 발신번호 조회 메소드
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCallingNumber()
        {
            string callingNumber = "027478640";

            HospitalReadModel hospitalModel = await _smsMonitoringQueries.GetHospitalInfo(hospitalId: _callContext.HospitalId).ConfigureAwait(false);

            if (hospitalModel != null)
            {
                AddressContentDto addressDto = JsonConvert.DeserializeObject<AddressContentDto>(hospitalModel.AddressContent) ;
                callingNumber = addressDto.BusinessPhoneNumber.Replace("-", "", StringComparison.Ordinal);
            }

            return callingNumber;
        }

        /// <summary>
        /// 에러코드 정의에 따른 에러 메시지 정의
        /// </summary>
        /// <param name="callStatusCode"></param>
        /// <returns></returns>
        private string GetSmsErrorMessage(string statusCode)
        {
            string errorMessage = string.Empty;
            if (string.IsNullOrEmpty(statusCode) || statusCode == "500")
            {
                // DESC : 프록시 서비스 호출에러시 CallStatusCode 가 null이거나 500으로 내부 설정.
                errorMessage = "SMS Proxy Service is not available";
            }
            else if (statusCode == "500-1")
            {
                // DESC : 프록시 서비스 호출에러시 CallStatusCode 가 null이거나 500으로 내부 설정.
                errorMessage = "Internal Server Error";
            }
            else if (statusCode == "404-1" || statusCode == "-1")
            {
                // DESC : 전화번호가 null or empty이면 404-1 로 에러코드 설정
                errorMessage = "Phone Number is not found";
            }
            else if (statusCode == "404-2" || statusCode == "-2")
            {
                // DESC : 개인정보사용동의가 안되어 있으면 404-2 로 에러코드 설정
                errorMessage = "Denied to use privacy information";
            }
            else if (statusCode == "404-3" || statusCode == "-3")
            {
                // DESC : 개인정보사용동의가 안되어 있으면 404-2 로 에러코드 설정
                errorMessage = "Block Patient";
            }
            else if (statusCode == "404-4")
            {
                // DESC : 발신번호를 못가져오면404-3 로 에러코드 설정
                errorMessage = "Calling Number is not found";
            }
            else if (statusCode == "408")
            {
                errorMessage = "Request Time is Expired Date";
            }
            return errorMessage;
        }

        

        /// <summary>
        /// ReceiveLog 업데이트 로직 중복제거를 위해 단일 메소드로 segregation함.
        /// </summary>
        /// <param name="smsResultView"></param>
        /// <param name="smsReceiveLogs"></param>
        private void UpdateReceiveLog(SmsResultView smsResultView, List<SmsReceiveLog> smsReceiveLogs)
        {

            if (smsResultView.Status != null)
            {
                foreach (var messageItem in smsResultView.Messages)
                {
                    var row = smsReceiveLogs.FirstOrDefault(p => p.Mobile == messageItem.To);
                    row.SmsId = messageItem.SmsId;
                    row.CompleteTime = messageItem.CompleteTime;
                    row.ContentType = messageItem.ContentType;
                    row.CountryCode = messageItem.ContryCode;
                    row.MessageId = messageItem.MessageId;
                    row.RequestTime = messageItem.RequestTime;
                    row.StatusCode = messageItem.StatusCode; // 단말 수신 상태 결과 코드 [0 : 성공 ,그외 실패]
                    row.StatusName = messageItem.StatusName; //단말 수신 상태 결과명
                    row.StatusMessage = messageItem.StatusMessage; //단말 수신 상태 결과 메시지
                    row.telcoCode = messageItem.TelcoCode;
                }
            }
        }

        /// <summary>
        /// SMS 메시지 전송 및 로그
        /// </summary>
        /// <param name="smsSendLog">SMS 발송 항목</param>
        /// <param name="smsReceiveLogs">SMS 수신자 및 수신결과 로그</param>
        /// <param name="callingNumber">발신번호(미사용)</param>
        /// <returns></returns>
        private async Task SendAndLogSms(SmsSendLog smsSendLog , List<SmsReceiveLog> smsReceiveLogs
            , string callingNumber, bool isResend)
        {
            // DESC : 정보제공 동의여부를 여기서 체크해서 , 
            //        정보제공 동의안한경우 에러로 출력할것. 단 전화번호 직접입력은 정보보호 동의 안하더라도 발송.
            List<string> mobileList = smsReceiveLogs.Where(i => i.Mobile != null && !string.IsNullOrEmpty(i.Mobile) 
                && (i.IsAgreeToUsePrivacyData || i.SmsRecipientType == SmsRecipientType.PatientDirectMobile))
                .Select(i => i.Mobile).ToList();


            DateTime currentDateTime = _timeManager.GetNow();
            smsSendLog.ExecutionTime = currentDateTime;

            SmsSendType smsMessage = null;
            SmsResultView smsProxyService = null;
            
            #region #### SMS/LMS 체크
            // TO-DO : size 체크해서 looping 전송 90byte 기준.
            // DESC : SMS 문자 90 byte 넘을경우 쪼개서 보낼지 , lms로 보낼지 한번더 확인 필요.
            //List<string> smsContent = GetShortString(smsSendLog.Content, 90);

            bool isSmsContent = CheckIsContentLenthSms(smsSendLog.Content);

            string smsTypeString = string.Empty;
            //string smsContentType = string.Empty;
            if (isSmsContent)
            {
                smsTypeString = "sms";

            }
            else
            {
                smsTypeString = "lms";

            } 
            #endregion

            smsMessage = new SmsSendType(type: smsTypeString, contentType: "COMM", country: "82"
                                           , from: smsSendLog.CallingNumber
                                           , to: mobileList.ToList()
                                           , content: smsSendLog.Content, employeeId: "000000");

            smsProxyService = await _smsSendProxy.SendSmsToTAApi(p_smsSendType: smsMessage).ConfigureAwait(false);


            // DESC : Resend 컨셉을 변경.
            // 기존 로그를 덮어쓰는것이 아니고, 새롭게 트랜잭션 진행해야함.

            if (smsProxyService.Status == null)
            {
                // DESC : SMS PROXY 에러발생 케이스
                smsSendLog.SmsTraceId = string.Empty;
                smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
               
                smsSendLog.CallStatusCode = "500";
                smsSendLog.ErrorMessage = GetSmsErrorMessage(smsSendLog.CallStatusCode);
            }
            else if (smsProxyService.Status == "200")
            {
                // DESC : 성공일 케이스
 
                smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Completed;
                smsSendLog.SmsTraceId = smsProxyService.SmsId;
                smsSendLog.CallStatusCode = smsProxyService.Status;
                smsSendLog.ErrorMessage = string.Empty;
            }
            else
            {
                // DESC : 그외 에러
                smsSendLog.SmsTraceId = string.IsNullOrEmpty(smsProxyService.SmsId) ? string.Empty : smsProxyService.SmsId;
                smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                smsSendLog.CallStatusCode = smsProxyService.Status;
                smsSendLog.ErrorMessage = smsProxyService.ErrorMessage;
            }

            // DESC : Receive Log 데이타 업데이트
            UpdateReceiveLog(smsProxyService, smsReceiveLogs);
        }
        /// <summary>
        /// 전송문자열의 길이를 byte단위로 변환해서 SMS여부를 리턴
        /// </summary>
        /// <param name="content">전송문자열</param>
        /// <returns></returns>
        private bool CheckIsContentLenthSms(string content)
        {
            bool isSms = true;
            int i, j;
            double length = 0;
            for (i = 0, j = content.Length; i < j; i++)
            {
                char c = content[i];
                length += (Char.GetUnicodeCategory(c).ToString() == "OtherLetter") ? 2 : 1;
            }

            if (length > 90)
            {
                isSms = false;
            }

            return isSms;
        }

        #region sms문자열 자르기
        /// <summary>
        /// 전송할 sms 나누기
        /// 현재 useless (2020/01/06)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="cutLength"></param>
        /// <returns></returns>
        /*
        private List<string> GetShortString(string content, int cutLength)
        {
            List<string> messageList = new List<string>();
            while (true)
            {
                int i = subStringValue(content, cutLength);
                if (i < 1)
                {
                    break;
                }
                messageList.Add(content.Substring(0, i));
                content = content.Substring(i);
            }

            return messageList;
        }
        */

        /// <summary>
        /// 잘나낼 문자열 위치 가져오기
        /// LMS를 SMS로 분리, 전송코드
        /// 현재 미사용
        /// </summary>
        /// <param name="content"></param>
        /// <param name="cutLength"></param>
        /// <returns></returns>
        /*
        private int subStringValue(string content, int cutLength)
        {
            int i, j;
            double length = 0;
            for (i = 0, j = content.Length; i < j; i++)
            {
                char c = content[i];
                length += (Char.GetUnicodeCategory(c).ToString() == "OtherLetter") ? 2 : 1;
                if (length >= cutLength)
                {
                    break;
                }
            }
            return i;
        }
        */
        #endregion

        //private Share.NotificationCenter.Enum.SmsRecipientType ConvertSmsRecipientTypeToSmsPatientResolveType(SmsRecipientType smsRecipientType)
        //{
        //    SmsRecipientType resolvedType = SmsRecipientType.Patient;
        //    if (smsRecipientType == SmsRecipientType.Guardian)
        //    {
        //        resolvedType = SmsRecipientType.Guardian;
        //    }
        //    else if (smsRecipientType == SmsRecipientType.PatientNotRegistered)
        //    {
        //        resolvedType = SmsRecipientType.PatientNotRegistered;
        //    }

        //    return resolvedType;
        //}


        /// <summary>
        /// 정보보호 대상여부 업데이트
        /// 현재 사용하지 않음 (2020/01/06)
        /// </summary>
        /// <param name="recipientList"></param>
        /// <returns></returns>
        
        private void CheckUsePrivacyDataAggrement(List<SmsReceiveLog> recipientList )
        {
            // DESC : 재전송 시점 전화번호 업데이트 및 정보제공 동의가 변경되었을 수 있으므로
            //          전화번호 및 정보제공 여부 업데이트 해야함.
            //Models.CommonModels.SmsRecipientDto smsRecipientDto = null;

            var list = recipientList.Where(p => p.SmsRecipientType == SmsRecipientType.Patient);
            foreach (var item in list)
            {
                string patientId = item.ActorId;
                item.IsBlocked = false;
                item.IsAgreeToUsePrivacyData = true;

                try
                {
                    var patientInfo = _patientInformationQueries.RetrievePatientInfomationWithPatientId(patientId);

                    if (patientInfo.IsDeath == true || patientInfo.IsSmsReceptionRejection == true)
                    {
                        item.IsBlocked = true;
                        item.IsAgreeToUsePrivacyData = (patientInfo.IsSmsReceptionRejection == true) ? false : true;
                    }
                }
                catch (Exception)
                {
                    item.IsBlocked = true;
                    item.IsAgreeToUsePrivacyData = false;
                }
            }

            #region 이전소스 백업2020/02/17
            /*
                foreach (SmsReceiveLog log in recipientList)
                {

                    //수신자가 환자 본인인 경우 sms수신동의여부를 체크.
                    if (log.SmsRecipientType == SmsRecipientType.Patient)
                    {
                        string patientId = log.ActorId;
                        log.IsBlocked = false;
                        log.IsAgreeToUsePrivacyData = true;

                        try
                        {
                            var patientInfo = _patientInformationQueries.RetrievePatientInfomationWithPatientId(patientId);


                            if (patientInfo.IsDeath == true || patientInfo.IsSmsReceptionRejection == true)
                            {
                                log.IsBlocked = true;
                                log.IsAgreeToUsePrivacyData = (patientInfo.IsSmsReceptionRejection == true) ? false : true;
                            }

                            //if (patientInfo.IsSmsReceptionRejection == true)
                            //{
                            //    log.IsAgreeToUsePrivacyData = false;
                            //    log.IsBlocked = true;
                            //}
                        }
                        catch (Exception e)
                        {
                            //throw new Exception(e.ToString());
                            log.IsBlocked = true;
                            log.IsAgreeToUsePrivacyData = false;
                        }
                    }

                }
                */ 
            #endregion
        }
        


        /// <summary>
        /// SMS 메시지 전송 서비스 메소드
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<bool> SendSmsMessage(string messageDispatchItemId)
        {

            //string timeZoneId = _timeManager.GetTimeZone(currentDateTime).TimeZoneId;
            //DateTime currentDateTimeUtc = _timeManager.ConvertToUTC(currentDateTime);
            DateTime currentDateTime = _timeManager.GetNow();
            DateTime expireDate = currentDateTime.AddHours(-4);
            // TO-DO : SMS 전송후 성공,실패 로그
            SmsSendLog smsSendLog = null;

            smsSendLog = await _smsMonitoringRepository.FindSmsSendLogByMessageDispatchItemId(messageDispatchItemId).ConfigureAwait(false);


            try
            {
                string callingNumber = smsSendLog.CallingNumber;

                #region ## 회신번호가 없으면 에러처리
                //회신번호가 없으면 에러처리하고 이하 로직은 실행시키지않음.
                if (string.IsNullOrEmpty(callingNumber))
                {
                    // 발신번호가 없으면 전체 에러임.
                    smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                    //전송할 핸드폰 번호갯수가 0인경우
                    // 전화번호가 없거나 개인정보동의 false인 경우
                    smsSendLog.CallStatusCode = "404-4";
                    smsSendLog.ErrorMessage = GetSmsErrorMessage(smsSendLog.CallStatusCode);
                    await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    return true;
                }

                //예약메시지 && 4시간(reservedTime) 초과된 경우, 즉시발송 && 4시간(executionTime) 초과된경우 실패처리
                if ((smsSendLog.IsReservedSms && smsSendLog.ReservedTime <= expireDate) 
                    || (!smsSendLog.IsReservedSms && smsSendLog.ExecutionTime <= expireDate) )
                {
                    // 발신번호가 없으면 전체 에러임.
                    smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                    //전송할 핸드폰 번호갯수가 0인경우
                    // 전화번호가 없거나 개인정보동의 false인 경우
                    smsSendLog.CallStatusCode = "408";
                    smsSendLog.ErrorMessage = GetSmsErrorMessage(smsSendLog.CallStatusCode);
                    await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    return true;
                }
                #endregion



                
                smsSendLog.SmsProgressStatus = SmsProgressStatus.InProgress;

                List<SmsReceiveLog> recipientList = await _smsMonitoringRepository.FindSmsReceiveLogByMessageDispatchItemId(messageDispatchItemId).ConfigureAwait(false);



                //개인정보동의, 사망환자 체크 후 isBlocked, 동의여부필드 업데이트
                //추후 sms수신동의에 대한 내용으로 변경처리필요 (2019/12/05)
                // await CheckUsePrivacyDataAggrement(recipientList).ConfigureAwait(false);
                CheckUsePrivacyDataAggrement(recipientList);


                //전화번호 없는경우, 개인정보수신동의 없는 경우 유효성 체크
                ValidateSmsReceiver(recipientList);

                //sms 수신동의, 직접등록, 사망 또는 block된 사용자는 전송금지
                List<string> mobileList = recipientList.Where(i => i.Mobile != null && !string.IsNullOrEmpty(i.Mobile)
                    && (i.IsAgreeToUsePrivacyData || i.SmsRecipientType == SmsRecipientType.PatientDirectMobile)
                    && !i.IsBlocked
                    )
                    .Select(i => i.Mobile).Distinct().ToList();

                if (mobileList.Count > 0)
                {
                    if (!smsSendLog.IsReservedSms && smsSendLog.SmsProgressStatus == Domain.Enum.SmsProgressStatus.InProgress)
                    {
                        smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.InProgress;
                        //await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
                        // DESC : 즉시발송이면서 처리전인 경우 발송함.
                        await SendAndLogSms(smsSendLog, recipientList, callingNumber, false).ConfigureAwait(false);

                    }
                    else if (smsSendLog.IsReservedSms && smsSendLog.SmsProgressStatus == Domain.Enum.SmsProgressStatus.InProgress
                        && smsSendLog.ReservedTime <= currentDateTime)
                    {
                        await SendAndLogSms(smsSendLog, recipientList, callingNumber, false).ConfigureAwait(false);
                    }
                }
                else
                {
                    smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                    //전송할 핸드폰 번호갯수가 0인경우
                    smsSendLog.CallStatusCode = "404-1";
                    smsSendLog.ErrorMessage = GetSmsErrorMessage(smsSendLog.CallStatusCode);
                }

            }
            catch (Exception ex)
            {
                if (smsSendLog != null)
                {
                    smsSendLog.SmsProgressStatus = SmsProgressStatus.Error;
                    smsSendLog.CallStatusCode = "500-1";
                    smsSendLog.ErrorMessage = GetSmsErrorMessage(smsSendLog.CallStatusCode);
                }
            }


            // DESC : 에러를 찍고나서 에러안나는 번호만 전송.

            await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// SMS 발송전 발리데이션 에러시 Receive에 기록함.
        /// 1.전화번호가 없는경우 발리데이션 receivelog의 status code = -1
        /// 1.개인정보사용 동의 없는 경우 receivelog의 status code = -2
        /// </summary>
        /// <param name="smsReceiveLogs"></param>
        private void ValidateSmsReceiver(List<SmsReceiveLog> smsReceiveLogs)
        {

            foreach (SmsReceiveLog log in smsReceiveLogs)
            {
                // DESC : 전화번호가 없을경우
                if (string.IsNullOrEmpty(log.Mobile))
                {
                    //executionLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                    //executionLog.CallStatusCode = "404-1";
                    //executionLog.ErrorMessage = GetSmsErrorMessage(executionLog.CallStatusCode);
                    //continue;
                    log.StatusCode = "-1";
                    log.StatusMessage = GetSmsErrorMessage(log.StatusCode);
                    continue;
                }

                // DESC : 개인정보 동의 없을 경우
                //if (!log.IsAgreeToUsePrivacyData)
                //{
                //    //executionLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                //    ////전송할 핸드폰 번호갯수가 0인경우
                //    //// 전화번호가 없거나 개인정보동의 false인 경우
                //    //executionLog.CallStatusCode = "404-2";
                //    //executionLog.ErrorMessage = GetSmsErrorMessage(executionLog.CallStatusCode);
                //    //continue;
                //    log.StatusCode = "-2";
                //    log.StatusMessage = GetSmsErrorMessage(log.StatusCode);
                //}

                //사망환자 또는 block
                if (log.IsBlocked)
                {
                    log.StatusCode = "-3";
                    log.StatusMessage = GetSmsErrorMessage(log.StatusCode);
                }


            }
        }


        /// <summary>
        /// 환자 SMS 메시지 재전송
        /// SmsSendLog와 SmsReceiveLog는 1:1로 매핑한다.
        /// 발신번호 : "027478640" (테스트용)
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<bool> ResendPatientSmsMessage(string messageDispatchItemId)
        {
            string callingNumber = await GetCallingNumber().ConfigureAwait(false);
            DateTime currentDateTime = _timeManager.GetNow();
            //string timeZoneId = _timeManager.GetTimeZone(currentDateTime).TimeZoneId;
            //DateTime currentDateTimeUtc = _timeManager.ConvertToUTC(currentDateTime);

            // TO-DO : SMS 전송후 성공,실패 로그
            SmsSendLog smsSendLog = await _smsMonitoringRepository.FindSmsSendLogByMessageDispatchItemId(messageDispatchItemId).ConfigureAwait(false);

            // TO-DO : 보호자인경우 contactRelationShipCode , contactClassificationCode 확인해서 수신자 설정필요. 
            List<SmsReceiveLog> recipientList = await _smsMonitoringRepository.FindSmsReceiveLogByMessageDispatchItemId(messageDispatchItemId).ConfigureAwait(false);


            // DESC : 재전송 시점 전화번호 업데이트 및 정보제공 동의가 변경되었을 수 있으므로
            //          전화번호 및 정보제공 여부 업데이트 해야함.
            //await CheckUsePrivacyDataAggrement(recipientList).ConfigureAwait(false);  

            ValidateSmsReceiver(recipientList);

            List<string> mobileList = recipientList.Where(i => i.Mobile != null && !string.IsNullOrEmpty(i.Mobile) && i.IsAgreeToUsePrivacyData).Select(i => i.Mobile).ToList();


            // DESC : 에러를 찍고나서 에러안나는 번호만 전송.
            if (mobileList.Count > 0)
            {
                if (string.IsNullOrEmpty(callingNumber))
                {
                    // 발신번호가 없으면 전체 에러임.
                    smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                    smsSendLog.ExecutionTime = currentDateTime;
                    //전송할 핸드폰 번호갯수가 0인경우
                    // 전화번호가 없거나 개인정보동의 false인 경우
                    smsSendLog.CallStatusCode = "404-3";
                    smsSendLog.ErrorMessage = GetSmsErrorMessage(smsSendLog.CallStatusCode);
                    await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
                    return true;
                }
                else
                {
                    smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.InProgress;
                    await SendAndLogSms(smsSendLog, recipientList, callingNumber, true).ConfigureAwait(false);
                }

                //smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.ResendInProgress;
                //await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
                //await SendAndLogSms(smsSendLog, recipientList, callingNumber, true).ConfigureAwait(false);
            }
            else
            {
                smsSendLog.SmsProgressStatus = Domain.Enum.SmsProgressStatus.Error;
                smsSendLog.ExecutionTime = currentDateTime;
                //전송할 핸드폰 번호갯수가 0인경우
                smsSendLog.CallStatusCode = "404-1";
                smsSendLog.ErrorMessage = GetSmsErrorMessage(smsSendLog.CallStatusCode);
            }
            await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }


        /// <summary>
        /// SMS 메시지 통신사 수신결과 조회 및 로그 업데이트
        /// </summary>
        /// <param name="messageIdList"></param>
        /// <returns></returns>
        public async Task<bool> BatchUpdateSmsReceiveLog(int delayMinute)
        {
            List<SmsReceiveLogDto> smsReceiveLogDtoList = await _smsMonitoringQueries.SearchUnprocessedReceiveLogsForStatistics(delayMinute: delayMinute).ConfigureAwait(false);

            
            foreach (var log in smsReceiveLogDtoList)
            {
                var serviceProxy = _smsSendProxy.GetSmsResult(log.MessageId);
                var updateReceiveLogRow = await _smsMonitoringRepository.FindSmsReceiveLog(log.Id).ConfigureAwait(false);

                //IB G/W Report Code 
                if (serviceProxy == null)
                {
                    // api 조회결과가 에러인경우
                    // TO-DO : 에러케이스에 대해 분석하여 분기처리 필요함.
                    log.telcoCode = string.Empty;
                    log.StatusCode = "500";
                    log.StatusMessage = GetSmsErrorMessage(log.StatusCode);
                    log.StatusName = "fail";
                }
                else //if (srvs.Status != "ERROR")
                {
                    var resultMessage = serviceProxy.Messages.FirstOrDefault();
                    log.telcoCode = resultMessage.TelcoCode;
                    log.StatusCode = resultMessage.StatusCode; // IB G/W Report Code(이통사 전송 후 받은 결과코드) , 0 성공, 그외 실패
                    log.StatusName = resultMessage.StatusName; // 단말 수신상태 결과명 (success)
                    log.StatusMessage = resultMessage.StatusMessage; //단말 수신 상태 결과 메시지 (성공)
                    log.CompleteTime = resultMessage.CompleteTime;
                    log.CountryCode = resultMessage.ContryCode;
                }

                updateReceiveLogRow.telcoCode = log.telcoCode;
                updateReceiveLogRow.StatusCode = log.StatusCode;
                updateReceiveLogRow.StatusMessage = log.StatusMessage;
                updateReceiveLogRow.StatusName = log.StatusName;
                updateReceiveLogRow.CompleteTime = log.CompleteTime;
                updateReceiveLogRow.CountryCode = log.CountryCode;
                updateReceiveLogRow.DataLastModifiedDateTimeUtc = _timeManager.GetUTCNow();

            }

            //저장하거나 업데이트 할 항목이 있는 경우만 실행
            if (smsReceiveLogDtoList.Count > 0)
            {
                #region 2020.1 MQ 사용하지 않음으로 변경.
                /*
                SmsResultUpdateIntegrationEvent smsResultUpdateIntegrationEvent = new SmsResultUpdateIntegrationEvent(smsReceiveLogDtoList);

                await _messagingService.PrepareMessageAsync(smsResultUpdateIntegrationEvent, null).ConfigureAwait(false);
                await _smsMonitoringRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);
                */
                #endregion

                await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            }
            
            //await _smsMonitoringRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
            return true;
        }
    }
}
