using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model
{
    public class JobPositionDto
    {
        public string JobPositionId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }


        #region Constructor
        public JobPositionDto(string jobPositionId, string displayCode, string name, Boolean isActive)
        {
            this.JobPositionId = jobPositionId;
            this.DisplayCode = displayCode;
            this.Name = name;
            this.IsActive = isActive;
        }
        #endregion
    }
}
