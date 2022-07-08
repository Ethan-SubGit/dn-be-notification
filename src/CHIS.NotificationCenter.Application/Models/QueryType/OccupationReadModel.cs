using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    /// <summary>
    /// patient contact
    /// </summary>
    public class OccupationReadModel
    {
        #region property

        public string Id { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        //public List<JobPositionMapping> JobPositionMappings { get; set; }
        public string TenantId { get; private set; }
        public string HospitalId { get; private set; }
        #endregion

    }
}
