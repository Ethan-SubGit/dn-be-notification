using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model
{
    public class EmployeeDto
    {
        public string EmployeeId { get; set; }
        public string DisplayId { get; set; }
        public string PersonId { get; set; }
        public string FullName { get; set; }
        public string OccupationId { get; set; }
        public string OccupationCode { get; set; }
        public string JobPositionId { get; set; }
        public string JobPositionCode { get; set; }
        public string DepartmentId { get; set; }
        public string WorkplaceId { get; set; }
        public DateTime? RetirementDate { get; set; }
        public UtcPack RetirementDateUtc { get; set; }
        public List<ContactPointDto> ContactPoints { get; set; }
        public string Email { get; set; }

        public EmployeeDto() { }

        public EmployeeDto(string employeeId, string displayId, string personId, string fullName
            , string occupationId, string occupationCode, string jobPositionId, string jobPositionCode, string departmentId
            , string workplaceId, DateTime? retirementDate/*, UtcPack retirementDateUtc*/, List<ContactPointDto> contactPoints, string email)
        {
            this.EmployeeId = employeeId;
            this.DisplayId = displayId;
            this.PersonId = personId;
            this.FullName = fullName;
            this.OccupationId = occupationId;
            this.OccupationCode = occupationCode;
            this.JobPositionId = jobPositionId;
            this.JobPositionCode = jobPositionCode;
            this.DepartmentId = departmentId;
            this.WorkplaceId = workplaceId;
            this.RetirementDate = retirementDate;
            //this.RetirementDateUtc = retirementDateUtc;
            this.ContactPoints = contactPoints;
            this.Email = email;
        }
    }
}
