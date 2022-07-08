using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Proxies
{

    public interface IAccessControlProxy
    {
        bool GetPatientOfAgreeToUsePrivacyData(string patientId);
    }
}
