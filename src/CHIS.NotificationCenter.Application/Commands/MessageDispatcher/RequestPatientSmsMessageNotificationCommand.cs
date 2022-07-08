using CHIS.NotificationCenter.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models.CommonModels;
namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    /// <summary>
    /// 환자 SMS 발송 요청
    /// </summary>
    public class RequestPatientSmsMessageNotificationCommand : IRequest<bool>
    {
        public List<PatientSmsMessageDto> SmsMessages { get; set; }

        public RequestPatientSmsMessageNotificationCommand()
        {
            SmsMessages = new List<PatientSmsMessageDto>();
        }
    }

}
