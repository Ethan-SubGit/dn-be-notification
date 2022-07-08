using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class DoctorView
    {
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public string Code { get; set; }
        public string TypeName { get; set; }
        public string Name { get; set; }
        public DepartmentRdo Department { get; set; }
        //public Boolean IsAppointment { get; set; }

        public DoctorView()
        {

        }

        public DoctorView(string id, string displayId, string code, string typeName, string name, DepartmentRdo department)
        {
            Id = id;
            DisplayId = displayId;
            Code = code;
            TypeName = typeName;
            Name = name;
            Department = department;
        }
    }
}
