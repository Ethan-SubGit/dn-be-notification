using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate
{
    public class TestFKMaster : EntityBase, IAggregateRoot
    {
        public string MasterCode { get; set; }
        public string MasterCodeName { get; set; }
        public DateTime? RegDate { get; set; }
        public string HospitalId { get; set; }
        public string TenantId { get; set; }

        private readonly List<TestFKSlave> _testFKSlaves;
        public IReadOnlyCollection<TestFKSlave> TestFKSlaves => _testFKSlaves;

        public TestFKMaster()
        {
            //
        }
        public TestFKMaster(string masterCode, string masterCodeName, DateTime? regDate)
        {
            MasterCode = masterCode;
            MasterCodeName = masterCodeName;
            RegDate = regDate;
        }
    }
}
