using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class EncounterParticipantDto
    {
        public string EncounterId       { get; set; } 
        public string EmployeeId        { get; set; }
        public string EmployeeDisplayId { get; set; }   
        public string EmployeeName      { get; set; }
        public string Contact           { get; set; }
        public string ActorTypeCode     { get; set; }
        public string ActorTypeName     { get; set; }
        public string DepartmentId      { get; set; } = "";
        public string DepartmentName    { get; set; } = "";
        public string JobPositionId     { get; set; } = "";
        public string JobPositionName   { get; set; } = "";
    }
}
