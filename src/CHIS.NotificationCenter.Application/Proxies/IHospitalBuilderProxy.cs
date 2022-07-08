using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models.ProxyModels.HospitalBuilder;
namespace CHIS.NotificationCenter.Application.Proxies
{

    public interface IHospitalBuilderProxy
    {
        HospitalModel GetHospital(string hospitalId);
    }
}
