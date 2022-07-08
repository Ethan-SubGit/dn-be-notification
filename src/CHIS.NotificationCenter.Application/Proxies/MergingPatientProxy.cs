using CHIS.Framework.Core;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;

namespace CHIS.NotificationCenter.Application.Proxies
{
    public class MergingPatientProxy : BSLBase, IMergingPatientProxy
    {
        private const string MERGING_SERVICE = "medicalrecordmerging";

        public MergingPatientProxy(ICallContext context) : base(context)
        {
        }
        public async Task<ResponsePdo> FindPatientMergingResultAsync(string primaryId, string classificationId)
        {
            try
            {
                using (ServiceClient client = new ServiceClient(base.Context, MERGING_SERVICE))
                {
                    var result = await client.GetAsync<ResponsePdo>("/medical-record-merging/v0/patient-mergings/result-responses?priamryId=" +
                                                                     primaryId + "&targetClassificationId=" + classificationId).ConfigureAwait(false);

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
