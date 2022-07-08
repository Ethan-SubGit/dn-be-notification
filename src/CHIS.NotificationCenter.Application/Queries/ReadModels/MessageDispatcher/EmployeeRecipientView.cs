using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.MessageDispatcher
{
    public class EmployeeRecipientView
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Mobile { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
    }
}
