using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Domain.AggregateModels.NotificationCenterConfigurationAggregate
{
    public interface INotificationCenterConfigurationRepository : IRepository<NotificationCenterConfiguration>
    {
        Task<T> CreateOrModifyEntity<T>(T entity) where T : EntityBase;

        Task<NotificationCenterConfiguration> FindByConfigurationKey(string configurationKey);
    }
}
