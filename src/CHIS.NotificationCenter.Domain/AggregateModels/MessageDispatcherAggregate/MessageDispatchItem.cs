using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.Events;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using System.Linq;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate
{
    public class MessageDispatchItem : EntityBase, IAggregateRoot
    {


        public string TenantId { get; set; }
        public string HospitalId { get; set; }

        public NotificationServiceType ServiceType { get; set; }
        public string ServiceCode { get; set; }
        public string MessagePriority { get; set; } //삭제예정
        public string SenderId { get; set; }

        public DateTime? SentTimeStamp { get; set; }
        public UtcPack SentTimeStampUtcPack { get; set; }

        public bool IsUsingPredefinedContent { get; set; }
        public string Title { get; set; } //삭제 예정
        public string Content { get; set; }

        public List<ContentParameter> ContentParameters { get; set; }

        public bool IsDispatched { get; set; } // 삭제예정

        public bool IsCanceled { get; set; }

        public string PatientId { get; set; }

        public string EncounterId { get; set; }

        public string IntegrationType { get; set; }
        public string IntegrationAddress { get; set; }
        public string IntegrationParameter { get; set; }


        public string SmsContentByInbox { get; set; }
        public string ReferenceId { get; set; }

        public CommunicationNoteMessageDeliveryOption CommunicationNoteMessageDeliveryOption { get; set; }

        public bool IsReservedSms { get; set; }
        public DateTime? ReservedSmsDateTime { get; set; }
        public UtcPack ReservedSmsDateTimeUtcPack { get; set; }

        //신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; }

        public List<AssignedEmployeeRecipient> AssignedEmployeeRecipients { get; set; }
        public List<AssignedDepartmentPolicy> AssignedDepartmentPolicies { get; set; }
        public List<AssignedEncounterPolicy> AssignedEncounterPolicies { get; set; }
        //public List<AssignedInstantSmsRecipient> AssignedInstantSmsRecipients { get; set; }
        //public List<AssignedPatientSmsRecipient> AssignedPatientSmsRecipients { get; set; }
        public List<MessageAttachment> MessageAttachments { get; set; }

        public List<MergingPatientGround> MergingPatientGroundsList { get; set; } = new List<MergingPatientGround>();

        public MessageDispatchItem()
        {
            MergingPatientGroundsList = new List<MergingPatientGround>();
        }
        /// <summary>
        /// Inbox Message Constructor
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="serviceCode"></param>
        /// <param name="messagePriority"></param>
        /// <param name="senderId"></param>
        /// <param name="isUsingPredefinedContent"></param>
        /// <param name="isSendSmsByInbox"></param>
        /// <param name="content"></param>
        /// <param name="patientId"></param>
        /// <param name="encounterId"></param>
        /// <param name="integrationType"></param>
        /// <param name="integrationAddress"></param>
        /// <param name="integrationParameter"></param>
        public MessageDispatchItem(
             string tenantId
          , string hospitalId
          , NotificationServiceType serviceType
          , string serviceCode
          , string senderId
          , bool isUsingPredefinedContent
          , string title
          , string content
          , string smsContentByInbox
          , string patientId
          , string encounterId
          , string integrationType
          , string integrationAddress
          , string integrationParameter
          , DateTime sentTimeStamp
          , UtcPack sentTimeStampUtcPack
          , string referenceId
          , DateTime dataFirstRegisteredDateTimeUtc
          , DateTime dataLastModifiedDateTimeUtc
          , Trace trace = null
            )
        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            ServiceType = serviceType;
            ServiceCode = serviceCode;

            SenderId = senderId;
            IsUsingPredefinedContent = isUsingPredefinedContent;
            SmsContentByInbox = smsContentByInbox;
            Title = title;
            Content = content;
            ContentParameters = new List<ContentParameter>();
            IntegrationType = integrationType;
            IntegrationAddress = integrationAddress;
            IntegrationParameter = integrationParameter;
            PatientId = patientId;
            EncounterId = encounterId;
            AssignedEmployeeRecipients = new List<AssignedEmployeeRecipient>();
            AssignedDepartmentPolicies = new List<AssignedDepartmentPolicy>();
            AssignedEncounterPolicies = new List<AssignedEncounterPolicy>();
            IsDispatched = false;
            SentTimeStamp = sentTimeStamp;
            SentTimeStampUtcPack = sentTimeStampUtcPack;

            ReferenceId = referenceId;
            //AssignedInstantSmsRecipients = new List<AssignedInstantSmsRecipient>();
            //AssignedPatientSmsRecipients = new List<AssignedPatientSmsRecipient>();

            MessageAttachments = new List<MessageAttachment>();
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
            Trace = trace;
        }

        /// <summary>
        /// SMS Message Constructor
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="serviceCode"></param>
        /// <param name="senderId"></param>
        /// <param name="isUsingPredefinedContent"></param>
        /// <param name="content"></param>
        public MessageDispatchItem(
            string tenantId
          , string hospitalId
          , NotificationServiceType serviceType
          , string serviceCode
          , string senderId
          , bool isUsingPredefinedContent
          , string content
          , string encounterId
          , string patientId
          , DateTime sentTimeStamp
          , UtcPack sentTimeStampUtcPack
          , bool isReservedSms
          , DateTime? reservedSmsDateTime
          , UtcPack reservedSmsDateTimeUtcPack
          , string referenceId
          , DateTime dataFirstRegisteredDateTimeUtc
          , DateTime dataLastModifiedDateTimeUtc
          , Trace trace = null
        )
        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            ServiceType = serviceType;
            ServiceCode = serviceCode;
            SenderId = senderId;
            IsUsingPredefinedContent = isUsingPredefinedContent;
            Content = content;

            IsReservedSms = isReservedSms;
            ReservedSmsDateTime = reservedSmsDateTime;
            ReservedSmsDateTimeUtcPack = reservedSmsDateTimeUtcPack;


            EncounterId = encounterId;
            PatientId = patientId;

            ContentParameters = new List<ContentParameter>();
            AssignedEmployeeRecipients = new List<AssignedEmployeeRecipient>();
            AssignedDepartmentPolicies = new List<AssignedDepartmentPolicy>();
            AssignedEncounterPolicies = new List<AssignedEncounterPolicy>();
            IsDispatched = false;
            SentTimeStamp = sentTimeStamp;
            SentTimeStampUtcPack = sentTimeStampUtcPack;

            //AssignedInstantSmsRecipients = new List<AssignedInstantSmsRecipient>();
            //AssignedPatientSmsRecipients = new List<AssignedPatientSmsRecipient>();

            MessageAttachments = new List<MessageAttachment>();

            ReferenceId = referenceId;
            Trace = trace;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;

        }

        public void AddContentParameter(ContentParameter contentParameter)
        {
            ContentParameters.Add(contentParameter);
        }
        public void AddAssignedEmployeeRecipient(AssignedEmployeeRecipient assignedEmployeeRecipient)
        {
            AssignedEmployeeRecipients.Add(assignedEmployeeRecipient);
        }

        public void AddAssignedDepartmentPolicy(AssignedDepartmentPolicy assignedDepartmentPolicy)
        {
            AssignedDepartmentPolicies.Add(assignedDepartmentPolicy);
        }

        public void AddAssignedEncounterPolicy(AssignedEncounterPolicy assignedEncounterPolicy)
        {
            AssignedEncounterPolicies.Add(assignedEncounterPolicy);
        }

        //public void AddAssignedInstantSmsRecipient(AssignedInstantSmsRecipient assignedInstantSmsRecipient)
        //{
        //    AssignedInstantSmsRecipients.Add(assignedInstantSmsRecipient);
        //}

        //public void AddAssignedPatientSmsRecipient(AssignedPatientSmsRecipient assignedPatientSmsRecipient)
        //{
        //    AssignedPatientSmsRecipients.Add(assignedPatientSmsRecipient);
        //}

        public void AddMessageAttachment(MessageAttachment messageAttachment)
        {
            MessageAttachments.Add(messageAttachment);
        }

        public void AddMessageDispatchStartedDomainEvent()
        {

            if (ServiceType == NotificationServiceType.Inbox || ServiceType == NotificationServiceType.CommunicationNote)
            {
                this.AddDomainEvent(new InboxMessageDispatchStartedDomainEvent(this));
            }
        }

        public void UpdateMergingPatientGround(MergingPatientGround mergingPatientGround)
        {
            if (mergingPatientGround == null)
            {
                throw new ArgumentNullException(nameof(mergingPatientGround));
            }

            PatientId = mergingPatientGround.MergingPatientId;
            //MergingPatientGroundsList.Add(mergingPatientGround);
            MergingPatientGroundsList = MergingPatientGroundsList.Append(mergingPatientGround).ToList();
        }
    }
}
