using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class JobPositionReadModel
    {
        #region property
        public string Id { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
        public string TenantId { get; private set; }
        public string HospitalId { get; private set; } 
        #endregion
    }
}
