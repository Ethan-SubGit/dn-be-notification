using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate
{
    public class TestFKSlave : EntityBase, IAggregateRoot
    {
        public string SubCode { get; set; }
        public string SubCodeName { get; set; }
        public string HospitalId { get; set; }
        public string TenantId { get; set; }

        public TestFKSlave()
        {
            //
        }
        public TestFKSlave(string subCode, string subCodeName)
        {
            SubCode = subCode;
            SubCodeName = subCodeName;
        }

    }
}
