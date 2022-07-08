using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class JobPositionMappingReadModel
    {
        #region property

        public string Id { get; set; }
        public string OccupationId { get; set; }
        public string JobPositionId { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public bool IsValidDataRow { get; set; } 
        #endregion
    }
}
