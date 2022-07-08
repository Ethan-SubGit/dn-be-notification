using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate
{
    public class EmployeeMessageBox : EntityBase, IAggregateRoot
    {
        #region property
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string EmployeeId { get; set; }
        //public string PersonalSettings { get; set; }

        public IList<EmployeeMessageInstance> EmployeeMessageInstances { get; private set; }

        //20191113 신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion


        public EmployeeMessageBox()
        {
            EmployeeMessageInstances = new List<EmployeeMessageInstance>();
            
        }

        public EmployeeMessageBox(
            string tenantId, string hospitalId, string employeeId, DateTime createUtcTime
           )
        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            EmployeeId = employeeId;
            DataFirstRegisteredDateTimeUtc = createUtcTime;
            DataLastModifiedDateTimeUtc = createUtcTime;

            EmployeeMessageInstances = new List<EmployeeMessageInstance>();

        }

        

    }
}
