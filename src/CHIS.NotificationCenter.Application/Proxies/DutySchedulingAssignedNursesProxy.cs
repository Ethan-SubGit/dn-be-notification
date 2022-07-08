using CHIS.Framework.Core;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.ProxyModels.DutyScheduling;
using CHIS.NotificationCenter.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Proxies
{
    /// <summary>
    /// 직원의 근무스케줄 관리
    /// </summary>
    public class DutySchedulingAssignedNursesProxy : BSLBase, IDutySchedulingAssignedNursesProxy
    {
        public DutySchedulingAssignedNursesProxy(ICallContext context) : base(context)
        {
        }

        public List<AssignedNursesModel> GetAssignedNurses(string patientId, string encounterId)
        {
            using (ServiceClient client = new ServiceClient(base.Context, "dutyscheduling"))
            {
                try
                {
                    string getUrl = $"/duty-scheduling/v0/duty-schedules/patients/assigned-nurses/search?patientid={patientId}&encounterId={encounterId}";
                    var assignedNursesModels = client.Get<List<AssignedNursesModel>>(getUrl);

                    if (assignedNursesModels != null)
                    {
                        return assignedNursesModels;
                    }
                    else
                    {
                        return new List<AssignedNursesModel>();
                    }
                }
                catch (Exception ex)
                {

                    return new List<AssignedNursesModel>();
                }
            }
        }
    }
}
