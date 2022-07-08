using CHIS.NotificationCenter.Domain.SeedWork;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate
{
    public interface IMessageDispatchItemRepository : IRepository<MessageDispatchItem>
    {
        MessageDispatchItem Create(MessageDispatchItem messageDispatchItem);
        Task<MessageDispatchItem> Retrieve(string messageDispatchItemId);
        Task<MessageDispatchItem> RetrieveWithNoPolicy(string messageDispatchItemId);
        //Task<List<MessageDispatchItem>> PrepareUnProcessedReservedSmsMessageItem(string tImeZoneId);
        Task<MessageDispatchItem> ModifyMessageDispatchItem(MessageDispatchItem item);
        Task<List<MessageDispatchItem>> FindPatientMergingTargets(string closingPatientId);
    }
}
