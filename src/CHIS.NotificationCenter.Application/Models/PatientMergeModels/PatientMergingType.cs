using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.PatientMergeModels
{
    public enum PatientMergingType
    {
        Merging = 0,
        Retrial = 1,
        Rollback = 2,
        retrialrollback = 3
    }
}
