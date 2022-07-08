using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model
{
    public class DepartmentDto
    {
        public string DepartmentId { get; private set; }
        public string DisplayCode { get; private set; }
        public string Name { get; private set; }
        public string Abbreviation { get; private set; }
        public DepartmentTypeCnum DepartmentType { get; set; }
        public bool IsVirtual { get; private set; }
        public bool IsAppointment { get; private set; }
        public string TenantId { get; private set; }
        public string HospitalId { get; private set; }


        #region Constructor

        public DepartmentDto(string departmentId, string displayCode, string name, string abbreviation, DepartmentTypeCnum departmentType, bool isVirtual
            , bool isAppointment, string tenantId, string hospitalId)
        {
            DepartmentId = departmentId;
            DisplayCode = displayCode;
            Name = name;
            Abbreviation = abbreviation;
            DepartmentType = departmentType;
            IsVirtual = isVirtual;
            IsAppointment = isAppointment;
            TenantId = tenantId;
            HospitalId = hospitalId;
        }
        #endregion

        public enum DepartmentTypeCnum
        {
            General,
            Treatment,
            Center,
            Clinic
        }
    }
}
