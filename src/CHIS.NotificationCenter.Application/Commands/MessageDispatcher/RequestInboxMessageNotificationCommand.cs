using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Newtonsoft.Json;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using System.Dynamic;
using CHIS.NotificationCenter.Application.Models.CommonModels;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    public class RequestInboxMessageNotificationCommand : IRequest<string>
    {

        
        public string ServiceCode { get; set; }
        public string SenderId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SmsContentByInbox { get; set; }
        public string PatientId { get; set; }
        public string EncounterId { get; set; }

        public string IntegrationType { get; set; }
        public IntegrationAddressDto IntegrationAddress { get; set; }
        public ExpandoObject IntegrationParameter { get; set; }
        public string ReferenceId { get; set; }

        public List<ContentParameterDto> ContentParameters { get; set; }
        public List<AssignedEmployeeRecipientDto> AssignedEmployeeRecipients { get; set; }
        public List<AssignedDepartmentPolicyDto> AssignedDepartmentPolicies { get; set; }
        public List<AssignedEncounterPolicyDto> AssignedEncounterPolicies { get; set; }

        public List<MessageAttachmentDto> MessageAttachments { get; set; }
    }




}
