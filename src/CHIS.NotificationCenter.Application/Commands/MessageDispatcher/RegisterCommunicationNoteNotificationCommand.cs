using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Newtonsoft.Json;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using System.Dynamic;
using CHIS.Share.NotificationCenter.Models;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Domain.Enum;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{

    public class RegisterCommunicationNoteNotificationCommand : IRequest<string>
    {
        public string ServiceCode { get; set; }
        public string SenderId { get; set; }
        public bool IsUsingPredefinedContent { get; set; }
        public string Content { get; set; }
        public string PatientId { get; set; }
        public string EncounterId { get; set; }
        
        public CommunicationNoteMessageDeliveryOption CommunicationNoteMessageDeliveryOption { get; set; }
        public bool IsReservedSms { get; set; }
        public DateTime? ReservedSmsDateTime { get; set; }
        public List<Models.CommonModels.ContentParameterDto> ContentParameters { get; set; }
        public List<AssignedEmployeeRecipientDto> AssignedEmployeeRecipients { get; set; }
        public List<AssignedDepartmentPolicyDto> AssignedDepartmentPolicies { get; set; }
        public List<AssignedEncounterPolicyDto> AssignedEncounterPolicies { get; set; }
        public List<MessageAttachmentDto> MessageAttachments { get; set; }
    }
}
