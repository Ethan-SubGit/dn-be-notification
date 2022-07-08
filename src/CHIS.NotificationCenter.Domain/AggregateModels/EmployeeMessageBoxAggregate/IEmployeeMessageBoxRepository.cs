using CHIS.NotificationCenter.Domain.SeedWork;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate
{
    public interface IEmployeeMessageBoxRepository   : IRepository<EmployeeMessageBox>
    {
        Task<List<EmployeeMessageBox>> Retrieve(List<string> employeeList);

        Task<EmployeeMessageBox> Retrieve(string employeeMessageBoxId);

        Task<EmployeeMessageBox> RetrieveByEmployeeId(string employeeId);

        Task<List<EmployeeMessageInstance>> RetrieveUnhandledEmployeeMessageInstancesByMessageDispatchItemId(string messageDispatchItemId);

        Task<List<EmployeeMessageBox>> RetrieveEmployeeMessageBoxList(string messageDispatchItemId);

        Task<EmployeeMessageInstance> RetrieveEmployeeMessageInstance(string id);

        Task<EmployeeMessageInstance> RetrieveEmployeeMessageInstance(string employeeMessageBoxId, string messageDispatchItemId, bool isHandled, bool isInbound);

        EmployeeMessageBox Create(EmployeeMessageBox employeeMessageBox);

        EmployeeMessageBox Update(EmployeeMessageBox employeeMessageBox);


        EmployeeMessageInstance UpdateEmployeeMessageInstance(EmployeeMessageInstance employeeMessageInstance);

        int GetUnhandledEmployeeMessageInstanceCountByMessageDispatchItemId(string messageDispatchItemId);
        
    }
}
