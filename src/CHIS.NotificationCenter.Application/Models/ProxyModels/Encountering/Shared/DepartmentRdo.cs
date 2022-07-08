using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class DepartmentRdo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public DepartmentType DepartmentType { get; set; }
        public Boolean IsVirtual { get; set; }
        public Boolean IsAppointment { get; set; }
        public List<EmployeeRdo> Employees { get; set; }

        public DepartmentRdo() { }

        public DepartmentRdo(string id, string code, string name, string abbreviation, DepartmentType departmentType, bool isVirtual, bool isAppointment)
        {
            Id = id;
            Code = code;
            Name = name;
            Abbreviation = abbreviation;
            DepartmentType = departmentType;
            IsVirtual = isVirtual;
            IsAppointment = isAppointment;
        }

        public DepartmentRdo(string id, string code, string name, string abbreviation, DepartmentType departmentType, bool isVirtual, bool isAppointment, List<EmployeeRdo> employees)
        {
            Id = id;
            Code = code;
            Name = name;
            Abbreviation = abbreviation;
            DepartmentType = departmentType;
            IsVirtual = isVirtual;
            IsAppointment = isAppointment;
            Employees = employees;
        }
    }
}
