using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class PatientStatusRdo
    {
        public bool IsInHospital { get; set; }
        public bool IsDischargeOrder { get; set; }
        public bool IsDischargeNurseReview { get; set; }
        public bool IsMedicalRecordReview { get; set; }
        public BusinessItemRdo ReviewStatus { get; set; }
        public bool IsDischargeBillPrint { get; set; }
        public bool IsDischarge { get; set; }
        public BusinessItemRdo DischargeProgressStatus { get; set; }

        public PatientStatusRdo() { }
        public PatientStatusRdo(bool isInHospital, bool isDischargeOrder, bool isDischargeNurseReview, bool isMedicalRecordReview, BusinessItemRdo reviewStatus,
            bool isDischargeBillPrint, bool isDischarge, BusinessItemRdo dischargeProgressStatus)
        {
            IsInHospital = isInHospital;
            IsDischargeOrder = isDischargeOrder;
            IsDischargeNurseReview = isDischargeNurseReview;
            IsMedicalRecordReview = isMedicalRecordReview;
            ReviewStatus = reviewStatus;
            IsDischargeBillPrint = isDischargeBillPrint;
            IsDischarge = isDischarge;
            DischargeProgressStatus = dischargeProgressStatus;
        }
    }
}
