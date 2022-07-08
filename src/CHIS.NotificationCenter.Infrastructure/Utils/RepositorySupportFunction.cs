using CHIS.NotificationCenter.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Infrastructure.Utils
{
    public class RepositorySupportFunction
    {
        public async static Task<T> CreateOrModifyEntity<T>(NotificationCenterContext dbContext, T entity) where T : EntityBase
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            var update = await dbContext.Set<T>().Where(u => u.Id == entity.Id).FirstOrDefaultAsync().ConfigureAwait(false);

            if (update == null)
            {
                return dbContext.Set<T>().Add(entity).Entity;
            }
            else
            {
                dbContext.Entry(update).CurrentValues.SetValues(entity);
                dbContext.Entry(update).State = EntityState.Modified;
                return update;
            }
        }
    }
}
