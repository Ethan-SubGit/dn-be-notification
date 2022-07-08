using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using CHIS.NotificationCenter.Domain.Enum;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class RegisterMessageSpecificationCommand : IRequest<string>
    {
        public NotificationServiceType ServiceType { get; set; }
        public string ServiceCode { get; set; }
        public string MessageCategory { get; set; }
        public string Classification { get; set; }
        public string Description { get; set; }
        public string PredefinedContent { get; set; }
        public bool IsForceToSendInboxSmsMessage { get; set; }
        public PostActionType PostActionType { get; set; }
        public bool IsSelectPatientByActiveEncounter { get; set; }
        public bool? IsSystemProperty { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsAddRecipient { get; set; }
        public string MessageCallbackNoConfigId { get; set; }
        public List<RegisterEmployeeRecipientDto> EmployeeRecipients { get; set; }
        public List<RegisterDepartmentPolicyDto> DepartmentPolicies { get; set; }
        public List<RegisterEncounterPolicyDto> EncounterPolicies { get; set; }
    }


    public class RegisterEmployeeRecipientDto
    {
        public string EmployeeId { get; set; }
    }

    public class RegisterDepartmentPolicyDto
    {
        public string ProtocolCode { get; set; }

        public string DepartmentId { get; set; }

        public string OccupationId { get; set; }

        public string JobPositionId { get; set; }
        public string WorkPlaceId { get; set; }
    }
    public class RegisterEncounterPolicyDto
    {
        public string ProtocolCode { get; set; }
    }

}
