using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.CommunicationNote
{
    public class CommunicationNoteRecipientView
    {
        public string MessageDispatchItemId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeDisplayId { get; set; }
        public string EmployeeName { get; set; }
        public string OccupationName { get; set; }
        public string DepartmentName { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadTime { get; set; }
    }
}
