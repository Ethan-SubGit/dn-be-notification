using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Queries.ReadModels.CommunicationNote;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Application.Models.CommonModels;
namespace CHIS.NotificationCenter.Application.Queries
{
    public interface ICommunicationNoteQueries
    {

        Task<int> SearchUnreadCount( string employeeId, string searchText);

        Task<CommunicationNotePackageView> SearchReceiveNote(
            string employeeId,
            string patientId,
            string searchText,
            int handleStatusFilter,
            int skip,
            int take);

        Task<CommunicationNoteView> FindReceiveNote(string messageInstanceId);

        Task<CommunicationNoteView> FindSentNote(string messageInstanceId);

        Task<CommunicationNotePackageView> SearchSentNote(string employeeId, string patientId, string searchText, int skip, int take);

        Task<IList<CommunicationNoteRecipientView>> SearchNoteRecipient(string messageDispatchItemId);

        Task<IList<EmployeeRecipientDto>> SearchEmployees(List<string> employeeIds);
    }
}
