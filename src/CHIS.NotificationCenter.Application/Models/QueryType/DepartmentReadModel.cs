using CHIS.NotificationCenter.Application.Models.DepartmentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class DepartmentReadModel
    {
        #region property
        public string Id { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public DepartmentType DepartmentType { get; set; }
        public bool IsVirtual { get; private set; }
        public bool IsAppointment { get; private set; }

        public string TenantId { get; private set; }
        public string HospitalId { get; private set; } 
        #endregion
    }
}
