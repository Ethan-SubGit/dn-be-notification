using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.PatientMergeModels
{
    public class MergeDto
    {
        public string Entity { get; set; }
        public string Id { get; set; }

        public MergeDto() { }
        public MergeDto(string entity, string id)
        {
            this.Entity = entity;
            this.Id = id;
        }
    }
}
