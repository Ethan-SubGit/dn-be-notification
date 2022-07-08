using CHIS.Framework.Core.Extension.Messaging.EventBus.Command;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;
using System;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Commands
{
    public class PatientMergingRetriedIntegrationCommand : IntegrationCommand
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
