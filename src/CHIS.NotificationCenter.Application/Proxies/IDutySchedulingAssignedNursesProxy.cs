using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models.ProxyModels.DutyScheduling;
namespace CHIS.NotificationCenter.Application.Proxies
{

    public interface IDutySchedulingAssignedNursesProxy
    {
        List<AssignedNursesModel> GetAssignedNurses(string patientId, string encounterId);
    }
}
