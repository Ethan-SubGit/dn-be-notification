using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model
{
    public class OccupationDto
    {
        public string Id { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public List<JobPositionMappingDto> JobPositionMappings { get; set; }


        #region Constructor
        public OccupationDto(string id, string displayCode, string name, bool isActive, List<JobPositionMappingDto> jobPositionMappings)
        {
            this.Id = id;
            this.DisplayCode = displayCode;
            this.Name = name;
            this.IsActive = isActive;
            this.JobPositionMappings = jobPositionMappings;
        }
        #endregion
    }
}
