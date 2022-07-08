using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate
{
    public class DepartmentPolicy : EntityBase
    {
        #region property
        public string ProtocolCode { get; set; }
        public string DepartmentId { get; set; }
        public string OccupationId { get; set; }
        public string JobPositionId { get; set; }
        public string WorkPlaceId { get; set; }
        public string MessageSpecificationId { get; set; }

        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        //20191113 신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion
        public DepartmentPolicy()
        { }

        public DepartmentPolicy(string protocolCode, string departmentId, string occupationId, string jobPositionId, string workPlaceId
            ,string tenantId, string hospitalId, DateTime dataFirstRegisteredDateTimeUtc, DateTime dataLastModifiedDateTimeUtc)
        {
            ProtocolCode = protocolCode;
            DepartmentId = departmentId;
            OccupationId = occupationId;
            JobPositionId = jobPositionId;
            WorkPlaceId = workPlaceId;
            TenantId = tenantId;
            HospitalId = hospitalId;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
        }

        public DepartmentPolicy(string protocolCode)
        {
            ProtocolCode = protocolCode;
        }

        
    }
}
