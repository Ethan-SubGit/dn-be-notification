using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Queries.ReadModels;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.MessageDispatcher;
namespace CHIS.NotificationCenter.Application.Queries
{
    public interface IMessageDispatcherQueries
    {
        Task<IList<EmployeeRecipientView>> FindAllEmployeeRecipients(List<string> employees);
        Task<IList<EmployeeRecipientView>> GetEmployeeRecipients(MessageDispatchItem messageItem);
        //Task<SmsRecipientDto> GetPatientContact(string patientId, SmsPatientResolveType smsPatientResolveType);
        //Task<IList<SmsRecipientDto>> GetPatientContacts(MessageDispatchItem messageItem);

        Task<SmsRecipientDto> FindEmployeeSmsRecipient(string employeeId);
    }
}
