using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.Enum;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class PatientSmsMessageDto
    {
        #region property
        /// <summary>
        /// serviceCode
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// 메시지 템플릿 id
        /// </summary>
        public string MessageTemplateId { get; set; }

        /// <summary>
        /// 발신자
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// 사전정의컨텐츠 사용여부
        /// </summary>
        public bool IsUsingPredefinedContent { get; set; }

        /// <summary>
        /// 전송문구
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 예약여부
        /// </summary>
        public bool IsReservedSms { get; set; }

        /// <summary>
        /// 예약시간
        /// </summary>
        public DateTime? ReservedSmsDateTime { get; set; }

        /// <summary>
        /// 환자ID
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 직원/환자/가족/직접입력
        /// </summary>
        public SmsRecipientType SmsRecipientType { get; set; }

        /// <summary>
        /// 본인과의 관계
        /// </summary>
        public string ContactRelationShipCode { get; set; } // 본인과의 관계

        /// <summary>
        /// 우선순위
        /// </summary>
        public string ContactClassificationCode { get; set; } // 우선순위

        /// <summary>
        /// 핸드폰번호
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 대체키/값
        /// </summary>
        public List<ContentParameterDto> ContentParameters { get; set; }

        #endregion
        public PatientSmsMessageDto() { }

    }
}
