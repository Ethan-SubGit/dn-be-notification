using CHIS.Framework.Core;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.ProxyModels.HospitalBuilder;
using CHIS.NotificationCenter.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Proxies
{

    public class HospitalBuilderProxy : BSLBase, IHospitalBuilderProxy
    {
        public HospitalBuilderProxy(ICallContext context) : base(context)
        {
        }

        public HospitalModel GetHospital(string hospitalId)
        {
            using (ServiceClient client = new ServiceClient(base.Context, "hospitalbuilder"))
            {
                try
                {
                    var hospitalModel = client.Get<HospitalModel>($"/hospital-builder/v0/regions/hospitals/{hospitalId}");

                    if (hospitalModel != null)
                    {
                        return hospitalModel;
                    }
                    else
                    {
                        return hospitalModel;
                    }
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }
        public List<HospitalModel> GetHospitals(string tenantId)
        {
            using (ServiceClient client = new ServiceClient(base.Context, "hospitalbuilder"))
            {
                try
                {
                    var hospitalModel = client.Get<List<HospitalModel>>($"/hospital-builder/v0/tenants/id={tenantId}/hospitals");

                    if (hospitalModel != null)
                    {
                        return hospitalModel;
                    }
                    else
                    {
                        return new List<HospitalModel>();
                    }
                }
                catch (Exception)
                {

                    return new List<HospitalModel>();
                }
            }
        }
    }
}
