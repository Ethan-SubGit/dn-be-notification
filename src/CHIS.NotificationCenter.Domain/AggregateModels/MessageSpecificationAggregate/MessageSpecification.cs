using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate
{
    public class MessageSpecification : EntityBase, IAggregateRoot
    {
        #region property
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public NotificationServiceType ServiceType { get; set; }
        public string ServiceCode { get; set; }
        public string MessageCategory { get; set; }
        public string Classification { get; set; }
        public string Description { get; set; }
        public string PredefinedContent { get; set; }
        public PostActionType PostActionType { get; set; }
        public bool IsSelectPatientByActiveEncounter { get; set; }
        public bool IsForceToSendInboxSmsMessage { get; set; }
        public bool IsSystemProperty { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAddRecipient { get; set; }

        //public IList<EmployeeRecipient> EmployeeRecipients { get; private set; }
        //public IList<DepartmentPolicy> DepartmentPolicies { get; private set; }
        //public IList<EncounterPolicy> EncounterPolicies { get; private set; }

        public IList<EmployeeRecipient> EmployeeRecipients { get; set; }
        public IList<DepartmentPolicy> DepartmentPolicies { get; set; }
        public IList<EncounterPolicy> EncounterPolicies { get; set; }

        /// <summary>
        /// message template
        /// </summary>
        public IList<MessageTemplate> MessageTemplates { get; private set; }

        /// <summary>
        /// sms인경우에 callbackNo
        /// </summary>
        public string MessageCallbackNoConfigId { get; set; }

        //public IList<EmployeeRecipient> EmployeeRecipients { get; private set; } = new List<EmployeeRecipient>();
        //public IList<DepartmentPolicy> DepartmentPolicies { get; private set; } = new List<DepartmentPolicy>();
        //public IList<EncounterPolicy> EncounterPolicies { get; private set; } = new List<EncounterPolicy>();

        //신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; }
        #endregion


        public MessageSpecification()
        {

        }
        public MessageSpecification(string id) : base(id)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="serviceType"></param>
        /// <param name="serviceCode"></param>
        /// <param name="messageCategory"></param>
        /// <param name="classification"></param>
        /// <param name="description"></param>
        /// <param name="predefinedContent"></param>
        /// <param name="postActionType"></param>
        /// <param name="isSelectPatientByActiveEncounter"></param>
        /// <param name="isForceToSendInboxSmsMessage"></param>
        /// <param name="dataFirstRegisteredDateTimeUtc"></param>
        /// <param name="dataLastModifiedDateTimeUtc"></param>
        /// <param name="trace"></param>
        /// <param name="isSystemProperty"></param>
        /// <param name="isDeleted"></param>
        public MessageSpecification(
            string tenantId
            , string hospitalId
            , NotificationServiceType serviceType
            , string serviceCode
            , string messageCategory
            , string classification
            , string description
            , string predefinedContent
            , PostActionType postActionType
            , bool isSelectPatientByActiveEncounter
            , bool isForceToSendInboxSmsMessage
            , DateTime dataFirstRegisteredDateTimeUtc
            , DateTime dataLastModifiedDateTimeUtc
            , bool isSystemProperty
            , bool isDeleted = false
            , bool isAddRecipient = false
            , string messageCallbackNoConfigId = null
            , Trace trace = null
            )
        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            ServiceType = serviceType;
            ServiceCode = serviceCode;
            MessageCategory = messageCategory;
            Classification = classification;
            Description = description;
            PredefinedContent = predefinedContent;
            PostActionType = postActionType;
            IsSelectPatientByActiveEncounter = isSelectPatientByActiveEncounter;
            IsForceToSendInboxSmsMessage = isForceToSendInboxSmsMessage;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
            Trace = trace;
            IsSystemProperty = isSystemProperty;
            IsDeleted = isDeleted;
            IsAddRecipient = isAddRecipient;
            MessageCallbackNoConfigId = messageCallbackNoConfigId;

            EmployeeRecipients = new List<EmployeeRecipient>();
            DepartmentPolicies = new List<DepartmentPolicy>();
            EncounterPolicies = new List<EncounterPolicy>();
        }

        #region ## 사용여부 확인필요
        /// <summary>
        /// 사용여부 확인필요.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="serviceType"></param>
        /// <param name="serviceCode"></param>
        /// <param name="messageCategory"></param>
        /// <param name="classification"></param>
        /// <param name="description"></param>
        /// <param name="predefinedContent"></param>
        /// <param name="postActionType"></param>
        /// <param name="isSelectPatientByActiveEncounter"></param>
        /// <param name="isForceToSendInboxSmsMessage"></param>
        /// <param name="individualRecipients"></param>
        /// <param name="departmentPolicies"></param>
        /// <param name="encounterPolicies"></param>
        /// <param name="dataFirstRegisteredDateTimeUtc"></param>
        /// <param name="dataLastModifiedDateTimeUtc"></param>
        /// <param name="trace"></param>
        public MessageSpecification(
            string tenantId
            , string hospitalId
            , NotificationServiceType serviceType
            , string serviceCode
            , string messageCategory
            , string classification
            , string description
            , string predefinedContent
            , PostActionType postActionType
            , bool isSelectPatientByActiveEncounter
            , bool isForceToSendInboxSmsMessage

            , IList<EmployeeRecipient> individualRecipients
            , IList<DepartmentPolicy> departmentPolicies
            , IList<EncounterPolicy> encounterPolicies
            , DateTime dataFirstRegisteredDateTimeUtc
            , DateTime dataLastModifiedDateTimeUtc
            , bool isSystemProperty
            , bool isDeleted = false
            , bool isAddRecipient = false
            , Trace trace = null
            )

        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            ServiceType = serviceType;
            ServiceCode = serviceCode;
            MessageCategory = messageCategory;
            Classification = classification;
            Description = description;
            PredefinedContent = predefinedContent;
            PostActionType = postActionType;
            IsSelectPatientByActiveEncounter = isSelectPatientByActiveEncounter;

            EmployeeRecipients = individualRecipients;
            DepartmentPolicies = departmentPolicies;
            EncounterPolicies = encounterPolicies;

            IsForceToSendInboxSmsMessage = isForceToSendInboxSmsMessage;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
            Trace = trace;
            IsSystemProperty = isSystemProperty;
            IsDeleted = isDeleted;
            IsAddRecipient = isAddRecipient;
        }
        #endregion

        #region ## 수신정책 추가/삭제

        public void AddEmployeeRecipient(EmployeeRecipient employeeRecipient)
        {
            EmployeeRecipients.Add(employeeRecipient);
        }
        public void RemoveEmployeeRecipient(EmployeeRecipient employeeRecipient)
        {
            EmployeeRecipients.Remove(employeeRecipient);
        }

        public void AddDepartmentPolicy(DepartmentPolicy departmentPolicy)
        {
            DepartmentPolicies.Add(departmentPolicy);
        }

        public void RemoveDepartmentPolicy(DepartmentPolicy departmentPolicy)
        {
            DepartmentPolicies.Remove(departmentPolicy);
        }
        public void AddEncounterPolicy(EncounterPolicy encounterPolicy)
        {
            EncounterPolicies.Add(encounterPolicy);
        }

        public void RemoveEncounterPolicy(EncounterPolicy encounterPolicy)
        {
            EncounterPolicies.Remove(encounterPolicy);
        } 
        #endregion
    }
}
