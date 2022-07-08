using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.DutyScheduling
{
    public class AssignedNursesModel
    {
        public string DutyPlaceId { get; set; }
        public string EmployeeId { get; set; }
        public bool IsCurrentWorker { get; set; }
    }

}
