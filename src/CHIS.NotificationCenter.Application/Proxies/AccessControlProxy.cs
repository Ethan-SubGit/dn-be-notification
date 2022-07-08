using CHIS.Framework.Core;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Models.ProxyModels.AccessControlModel;
namespace CHIS.NotificationCenter.Application.Proxies
{
   /// <summary>
   /// 환자 정보보호여부 체크
   /// </summary>
    public class AccessControlProxy : BSLBase, IAccessControlProxy
    {
        public AccessControlProxy(ICallContext context) : base(context)
        {
        }

        public bool GetPatientOfAgreeToUsePrivacyData(string patientId)
        {
            using (ServiceClient client = new ServiceClient(base.Context, "accesscontrol"))
            {
                try
                {
                    var patientSummary = client.Get<PatientSummaryView>($"/access-control/v0/patient/check?patientId={patientId}");

                    if (patientSummary != null && !string.IsNullOrEmpty(patientSummary.Id))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }
    }
}
