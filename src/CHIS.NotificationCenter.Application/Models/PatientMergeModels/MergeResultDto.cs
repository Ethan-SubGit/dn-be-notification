using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.PatientMergeModels
{
    public class MergeResultDto
    {
        public string ErrorMessage { get; set; }
        public ICollection<MergeDto> Merges { get; set; } = new List<MergeDto>();

        public MergeResultDto() { }
        public MergeResultDto(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
        }
        public bool HasError()
        {
            return !(string.IsNullOrEmpty(this.ErrorMessage));
        }
    }
}
