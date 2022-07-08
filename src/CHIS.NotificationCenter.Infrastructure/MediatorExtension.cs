using MediatR;
using System.Linq;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Infrastructure
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, NotificationCenterContext ctx)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<EntityBase>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.DomainEvents.Clear());

            ///////////// 등록된 이벤트를 순차적으로 돌리는 방식 /////////////
            foreach (var DomainEvent in domainEvents)
            {
                await mediator.Publish(DomainEvent).ConfigureAwait(false);
            }

            ///////////// 등록된 이벤트를 무작위로 돌리는 로직 /////////////
            //var tasks = domainEvents
            //    .Select(async (domainEvent) => {
            //        await mediator.Publish(domainEvent).ConfigureAwait(false);
            //    });
            //await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}
