using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate
{
    public class EncounterPolicy : EntityBase
    {
        #region property
        public string ProtocolCode { get; set; }
        public string MessageSpecificationId { get; set; }

        public string TenantId { get; set; }
        public string HospitalId { get; set; }

        //20191113 신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion

        public EncounterPolicy()
        { }
        public EncounterPolicy(string protocolCode)
        {
            ProtocolCode = protocolCode;
        }

        
    }
}
