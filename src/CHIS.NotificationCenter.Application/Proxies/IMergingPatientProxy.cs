using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;

namespace CHIS.NotificationCenter.Application.Proxies
{
    public interface IMergingPatientProxy
    {
        Task<ResponsePdo> FindPatientMergingResultAsync(string primaryId, string classificationId);
    }
}
