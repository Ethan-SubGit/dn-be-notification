using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class DepartmentsView
    {
        public List<DepartmentRdo> Departments { get; set; } = new List<DepartmentRdo>();

        public DepartmentsView()
        {

        }

        public DepartmentsView(List<DepartmentRdo> departments)
        {
            Departments = departments;
        }
    }
}
