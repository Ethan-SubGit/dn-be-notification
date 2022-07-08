using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Application.Queries
{
    public interface IEmployeeMessageBoxQueries
    {
        Task<IList<MessageCountView>> RetrieveMessageCountByEmployee(string employeeId, string searchText, string patientId);
        Task<IList<MessageCountView>> SearchGrandTotalMessageCount(string employeeId);
        Task<IList<MessageCountView>> SearchMessageCount(
            string employeeId,
            //string messageCategory,
            string patientId,
            string searchText,
            string periodFilter,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            List<string> filterByServiceCodes,
            string departmentId
            );

        //Task<EmployeeMessagePackageView> RetrieveInboxMessagesByPagination(string employeeId, string messageCategory, string patientId, string searchText
        //    , int filterOption, int skip, int take, string exclusionMessageInstanceId, string inclusionMessageDispatchItemId);
        Task<EmployeeMessagePackageView> SearchMessages(
            string employeeId, 
            string messageCategory, 
            string patientId, 
            string searchText,
            string periodFilter,
            int handleStatusFilter , 
            string exclusionMessageInstanceId, 
            DateTime? fromDateTime, 
            DateTime? toDateTime,
            List<string> filterByServiceCodes, 
            string departmentId,
            int skip,
            int take
            );
        Task<EmployeePatientMessagePackageView> SearchPatientMessages(
            string employeeId,
            //string messageCategory,
            string patientId,
            string searchText,
            string periodFilter,
            int handleStatusFilter,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            List<string> filterByServiceCodes,
            string departmentId,
            int skip,
            int take
            );


        //Task<EmployeeMessagePackageView> RetrieveInboxMessagesByPaginationV2(string employeeId, string messageCategory, string patientId, string searchText
        //    , int handleOption, int skip, int take, string exclusionMessageInstanceId, string inclusionMessageDispatchItemId);

        //Task<EmployeePatientMessagePackageView> RetrieveInboxPatientMessagesList(string employeeId, string searchText, int filterOption, int skip, int take);

        Task<EmployeeMessageView> FindMessage(string messageInstanceId);

        Task<EmployeeMessageView> FindOutboxMessage(string messageDispatchItemId);

        Task<EmployeeMessagePackageView> SearchOutboxMessages(
            string employeeId,
            string messageCategory,
            string patientId,
            string searchText,
            string periodFilter,
            string exclusionMessageInstanceId,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            List<string> filterByServiceCodes,
            string departmentId,
            int skip,
            int take);


        Task<IList<RecipientMessageView>> RetrieveMessagesRecipients(string messageDispatchItemId);

        Task<MessageRecipientStatusView> RetrieveMessageRecipientStatuses(string messageDispatchItemId);

        Task<EmployeeMessageCategory> RetrieveMessageCategory(string employeeMessageInstanceId);
        Task<string> RetrieveMessageDispatchItemIdWithReference(string referenceId, string serviceCode);
    }

}
