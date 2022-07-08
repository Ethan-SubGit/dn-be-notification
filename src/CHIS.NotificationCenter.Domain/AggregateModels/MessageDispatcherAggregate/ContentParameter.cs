using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate
{
    public class ContentParameter : EntityBase
    {
        public string ParameterValue { get; set; }
        public string MessageDispatchItemId { get; set; }

        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        //20191113 신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; }
        public ContentParameter()
        {
            //
        }
    }
}
