
using CHIS.Framework.Core.Extension.Messaging.EventBus.Command;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;
using System;
using System.Collections.Generic;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Commands
{
    public class PatientMergingRespondedIntegrationCommand : IntegrationCommand
    {
        public string PrimaryId { get; set; }
        public DateTime MergingDate { get; set; }
        public string DomainName { get; set; }
        public bool IsCompleted { get; set; }
        public IEnumerable<MergeDto> Merges { get; set; }
        public string ErrorComment { get; set; }
        public string HospitalId { get; set; }
        public PatientMergingRespondedIntegrationCommand(string primaryId, DateTime mergingDate, string domainName
                                                       , bool isCompleted, IEnumerable<MergeDto> merges, string errorComment, string hospitalId)
        {
            this.PrimaryId = primaryId;
            this.MergingDate = mergingDate;
            this.DomainName = domainName;
            this.IsCompleted = isCompleted;
            this.Merges = merges;
            this.ErrorComment = errorComment;
            this.HospitalId = hospitalId;
        }
    }
}
