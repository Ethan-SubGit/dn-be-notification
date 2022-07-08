using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model
{
    public class JobPositionMappingDto
    {
        public string JobPositionMappingId { get; set; }
        public string OccupationId { get; set; }
        public string JobPositionId { get; set; }
        public bool IsValidDataRow { get; set; }

        #region Constructor
        public JobPositionMappingDto(string jobPositionMappingId, string occupationId, string jobPositionId)
        {
            this.JobPositionMappingId = jobPositionMappingId;
            this.OccupationId = occupationId;
            this.JobPositionId = jobPositionId;
        }
        #endregion
    }
}
