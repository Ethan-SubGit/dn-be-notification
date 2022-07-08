using CHIS.NotificationCenter.Domain.SeedWork;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate
{
    public interface IMessageSpecificationRepository : IRepository<MessageSpecification>
    {
        #region messageSpecification
        MessageSpecification Create(MessageSpecification messageSpecification);
        MessageSpecification Update(MessageSpecification messageSpecification);
        DepartmentPolicy DeleteDepartmentPolicy(DepartmentPolicy departmentPolicy);
        EncounterPolicy DeleteEncounterPolicy(EncounterPolicy encounterPolicy);
        EmployeeRecipient DeleteEmployeeRecipient(EmployeeRecipient employeeRecipient);
        Task<MessageSpecification> Retrieve(string id);
        MessageSpecification FindByServiceCode(string serviceCode);
        Task<MessageSpecification> FindByServiceCodeWithPolicy(string serviceCode);
        #endregion

        #region template
        MessageTemplate CreateTemplate(MessageTemplate messageTemplate);
        MessageTemplate UpdateTemplate(MessageTemplate messageTemplate);
        MessageTemplate DeleteTemplate(string id);
        MessageTemplate RetrieveTemplate(string id);
        #endregion

        #region callbackNo
        MessageCallbackNoConfig CreateCallbackNo(MessageCallbackNoConfig messageCallbackNo);
        MessageCallbackNoConfig UpdateCallbackNo(MessageCallbackNoConfig messageCallbackNo);
        /// <summary>
        /// 미사용
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MessageCallbackNoConfig DeleteCallbackNo(string id);
        MessageCallbackNoConfig RetrieveCallbackNo(string id); 
        #endregion


    }
}
