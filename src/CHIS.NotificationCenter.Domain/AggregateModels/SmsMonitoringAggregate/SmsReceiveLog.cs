using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
//using CHIS.Share.NotificationCenter.Enum;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using System.Linq;

namespace CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate
{
    public class SmsReceiveLog : EntityBase, IAggregateRoot
    {
        #region property
        
        public string TenantId { get; set; }
        public string HospitalId { get; set; }



        public SmsRecipientType SmsRecipientType { get; set; }

        public string ActorId { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Content { get; set; }
        public bool IsSuccess { get; set; } // 사용안함, 삭제 예정
        public bool IsAgreeToUsePrivacyData { get; set; }
        public DateTime? SentTimeStamp { get; set; }
        public UtcPack SentTimeStampUtcPack { get; set; }
        public string MessageDispatchItemId { get; set; }
        public string PatientContactRelationShipCode { get; set; }
        public string PatientContactClassificationCode { get; set; }

        #region TA API 리턴코드
        //return TA API
        /// <summary>
        /// 그룹메시지 전송키
        /// </summary>
        public string SmsId { get; set; }

        /// <summary>
        /// 개별 메시지 전송키
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// 국가코드
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 결과받은 시간
        /// </summary>
        public DateTime? CompleteTime { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 통신사 코드
        /// </summary>
        public string telcoCode { get; set; }

        /// <summary>
        /// 결과상태값(에러메시지)
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// 전송요청시간
        /// </summary>
        public DateTime? RequestTime { get; set; }

        /// <summary>
        /// 상태값
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 상태코드
        /// </summary>
        public string StatusCode { get; set; }
        /// <summary>
        /// 합본기록
        /// </summary>
        public List<MergingPatientGround> MergingPatientGrounds { get; set; } = new List<MergingPatientGround>();
        #endregion

        /// <summary>
        /// 발송금지 번호여부(사망환자, 환자합본
        /// </summary>
        public bool IsBlocked { get; set; }
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; }
        #endregion

        public SmsReceiveLog() { }

        public SmsReceiveLog(
            string tenantId
            , string hospitalId
            , SmsRecipientType smsRecipientType
            , string name
            , string mobile
            , string content
            , bool isSuccess
            , bool isAgreeToUsePrivacyData
            //, DateTime? sentTimeStamp
            //, UtcPack sentTimeStampUtcPack
            , string messageDispatchItemId
            , string actorId
            , string patientContactRelationShipCode
            , string patientContactClassificationCode
            , DateTime dataFirstRegisteredDateTimeUtc
            , DateTime dataLastModifiedDateTimeUtc
            , Trace trace = null
            , bool isBlocked = false
            , string statusCode = ""
            , string statusMessage = ""
           )
        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            SmsRecipientType = smsRecipientType;
            Name = name;
            Mobile = mobile;
            Content = content;
            IsSuccess = isSuccess;
            IsAgreeToUsePrivacyData = isAgreeToUsePrivacyData;
            //SentTimeStamp = sentTimeStamp;
            //SentTimeStampUtcPack = sentTimeStampUtcPack;

            MessageDispatchItemId = messageDispatchItemId;
            ActorId = actorId;
            PatientContactRelationShipCode = patientContactRelationShipCode;
            PatientContactClassificationCode = patientContactClassificationCode;
            Trace = trace;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
            IsBlocked = isBlocked;
            StatusCode = statusCode;
            StatusMessage = statusMessage;
        }

        public void UpdateMergingPatientGround(MergingPatientGround mergingPatientGround)
        {
            if (mergingPatientGround == null)
            {
                throw new ArgumentNullException(nameof(mergingPatientGround));
            }

            ActorId = mergingPatientGround.MergingPatientId;
            //MergingPatientGrounds.Add(mergingPatientGround);
            MergingPatientGrounds = MergingPatientGrounds.Append(mergingPatientGround).ToList();
        }

    }
}
