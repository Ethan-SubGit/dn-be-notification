using CHIS.Framework.Core;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering;
using CHIS.NotificationCenter.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Proxies
{
    public class EncounteringProxy : BSLBase, IEncounteringProxy
    {
        public EncounteringProxy(ICallContext context) : base(context)
        {
        }

        public List<EncounterBasicView> GetEncounterBasic(string patientId, string encounterId)
        {
            using (ServiceClient client = new ServiceClient(base.Context, "encountering"))
            {
                try
                {
                    var encouterBasicModel = client.Get<List<EncounterBasicView>>($"/encountering/v0/encounters/basic?patientId={patientId}&id={encounterId}");

                    if (encouterBasicModel != null)
                    {
                        return encouterBasicModel;
                    }
                    else
                    {
                        return new List<EncounterBasicView>();
                    }
                }
                catch (Exception)
                {

                    return new List<EncounterBasicView>();
                }
            }
        }
    }
}
