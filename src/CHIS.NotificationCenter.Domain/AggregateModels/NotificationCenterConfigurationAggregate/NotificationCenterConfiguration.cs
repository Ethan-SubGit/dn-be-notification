using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace CHIS.NotificationCenter.Domain.AggregateModels.NotificationCenterConfigurationAggregate
{
    public class NotificationCenterConfiguration : EntityBase, IAggregateRoot
    {
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string ConfigurationKey { get; set; }
        public string ConfigurationValue { get; set; }
        public NotificationCenterConfiguration()
        { }

        public NotificationCenterConfiguration(
            string tenantId
            , string hospitalId
            , string configurationKey
            , string configurationValue
            )
        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            ConfigurationKey = configurationKey;
            ConfigurationValue = configurationValue;
        }
    }
}
