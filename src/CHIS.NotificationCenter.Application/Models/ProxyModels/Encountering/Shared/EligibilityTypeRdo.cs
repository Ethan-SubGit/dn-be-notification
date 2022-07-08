using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class EligibilityTypeRdo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public BusinessItemRdo EligibilityClassification { get; set; }
        public bool IsEligibilityManagement { get; set; }
        public bool IsValidDataRow { get; set; }

        public EligibilityTypeRdo() { }
        public EligibilityTypeRdo(string code, string name, BusinessItemRdo eligibilityClassification, bool isEligibilityManagement, bool isValidDataRow)
        {
            Code = code;
            Name = name;
            EligibilityClassification = eligibilityClassification;
            IsEligibilityManagement = isEligibilityManagement;
            IsValidDataRow = isValidDataRow;
        }
    }
}
