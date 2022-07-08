using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.MessageSpecification;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Enum;

namespace CHIS.NotificationCenter.Application.Queries
{
    public interface IMessageSpecificationQueries
    {
        Task<IList<MessageSpecificationView>> RetrieveMessageSpecifications(int serviceType);
        //Task<IList<DepartmentPolicyView>> FindRecipientPolicies(string messageSpecificationId);
        //Task<IList<EmployeeRecipientView>> FindAllEmployeeRecipients(string messageSpecificationId);
        Task<IList<MessageSpecificationView>> SearchInboxMessageSpecifications();
        Task<IList<MessageSpecificationView>> SearchSmsMessageSpecifications(string messageCategory);
        Task<dynamic> FindMessageSpecification(string messageSpecificationId);
        Task<dynamic> FindMessageSpecificationByServiceCode(string serviceCode);
        Task<IList<dynamic>> RetrieveRecipientPolicyProtocol(int type);
        Task<IList<MessageTemplateView>> RetrieveMessageTemplatesByServiceCode(string serviceCode);
        Task<MessageTemplateView> retrieveMessageTemplateById(string id);
        //Task<MessageTemplateViewByServiceType> RetrievesMessageTemplateByServiceType(string serviceType);
        Task<IList<MessageTemplateViewByServiceType>> RetrievesMessageTemplateByServiceType(NotificationServiceType serviceType);

        Task<MessageCallbackNoConfig> RetrieveMessageCallbackNo(string id);
        Task<IList<MessageCallbackNoConfig>> RetrievesMessageCallbackNo();

        Task<bool> DuplicateCallbackNoCheck(string callbackNo);

        Task<string> GetCallbackNoByServiceCode(string serviceCode);
    }
}
