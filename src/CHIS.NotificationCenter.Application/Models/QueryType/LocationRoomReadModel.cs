using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class LocationRoomReadModel
    {
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public string DepartmentId { get; set; }
    }
}
