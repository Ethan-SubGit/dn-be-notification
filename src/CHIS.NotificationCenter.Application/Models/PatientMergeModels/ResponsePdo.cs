using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.PatientMergeModels
{
    public class ResponsePdo
    {
        public DateTime MergingDate { get; set; }
        public string DomainName { get; set; }
        public bool IsCompleted { get; set; }
        public string ErrorContent { get; set; }
        public List<MergePdo> Merges { get; set; }
    }

    public class MergePdo
    {
        public string Entity { get; set; }
        public string Id { get; set; }
    }
}
