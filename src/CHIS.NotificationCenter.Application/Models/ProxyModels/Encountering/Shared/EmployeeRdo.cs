using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class EmployeeRdo
    {
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public string FullName { get; set; }
        public string DepartmentDisplayCode { get; set; }
        public string DepartmentDisplayName { get; set; }

        public EmployeeRdo() { }
        public EmployeeRdo(string id, string displayId, string fullName, string departmentDisplayCode, string departmentDisplayName)
        {
            Id = id;
            DisplayId = displayId;
            FullName = fullName;
            DepartmentDisplayCode = departmentDisplayCode;
            DepartmentDisplayName = departmentDisplayName;
        }
    }
}
