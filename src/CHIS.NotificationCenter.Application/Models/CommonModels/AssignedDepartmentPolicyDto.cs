using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class AssignedDepartmentPolicyDto
    {
        public string ProtocolCode  { get; set; }
        public string DepartmentId  { get; set; }
        public string OccupationId  { get; set; }
        public string JobPositionId { get; set; }
    }
}
