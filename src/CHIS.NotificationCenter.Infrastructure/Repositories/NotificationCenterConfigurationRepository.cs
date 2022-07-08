using CHIS.NotificationCenter.Domain.AggregateModels.NotificationCenterConfigurationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Linq;

using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;

namespace CHIS.NotificationCenter.Infrastructure.Repositories
{
    public class NotificationCenterConfigurationRepository : INotificationCenterConfigurationRepository
    {
        private readonly NotificationCenterContext _dbContext;
        private readonly ICallContext _callContext;
        public IUnitOfWork UnitOfWork => _dbContext;

        public NotificationCenterConfigurationRepository(NotificationCenterContext dbContext, ICallContext callContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
        }

        public async Task<T> CreateOrModifyEntity<T>(T entity) where T : EntityBase
        {
            return await NotificationCenter.Infrastructure.Utils.RepositorySupportFunction.CreateOrModifyEntity(_dbContext, entity).ConfigureAwait(false);
        }

        public async Task<NotificationCenterConfiguration> FindByConfigurationKey(string configurationKey)
        {
            NotificationCenterConfiguration notificationCenterConfiguration =
                await  _dbContext.NotificationCenterConfiguration.Where(c => c.ConfigurationKey == configurationKey
                && c.TenantId == _callContext.TenantId && c.HospitalId == _callContext.HospitalId
                ).FirstOrDefaultAsync().ConfigureAwait(false);

            return notificationCenterConfiguration;
        }


    }


}
