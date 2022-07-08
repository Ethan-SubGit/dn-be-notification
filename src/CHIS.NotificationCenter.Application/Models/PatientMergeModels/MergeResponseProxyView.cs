using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.PatientMergeModels
{
    public class MergeResponseProxyView
    {
        public DateTime MergingDate { get; set; }
        public string DomainName { get; set; }
        public bool IsCompleted { get; set; }
        public string ErrorContent { get; set; }
        public List<MergeDto> Merges { get; set; }
    }
}
