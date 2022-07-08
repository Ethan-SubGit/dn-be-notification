using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;
using System;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    public class PatientMergingRollbackIntegrationEvent : IntegrationEvent
    {
        public string PrimaryId { get; set; }
        public DateTime SendDate { get; set; }
        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public PatientMergingType Type { get; set; }
        public PatientDto ClosingPatient { get; set; }
        public PatientDto MergingPatient { get; set; }
        public string HospitalId { get; set; }
    }
}
