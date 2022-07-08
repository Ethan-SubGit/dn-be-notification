using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class EmployeeRecipientDto
    {
 
        public string EmployeeId { get; set; }
        public string EmployeeDisplayId { get; set; }
        public string EmployeeName { get; set; }
        public string Mobile { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string OccupationId { get; set; }
        public string OccupationName { get; set; }
        public bool Inbound { get; set; }

    }
}
