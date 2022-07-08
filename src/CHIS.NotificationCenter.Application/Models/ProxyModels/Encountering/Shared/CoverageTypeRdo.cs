using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class CoverageTypeRdo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<EligibilityTypeProfileRdo> EligibilityTypes { get; set; }
        public bool IsEligibilityManagement { get; set; }
        public bool IsValidDataRow { get; set; }

        public CoverageTypeRdo() { }
        public CoverageTypeRdo(string code, string name, List<EligibilityTypeProfileRdo> eligibilityTypes, bool isEligibilityManagement, bool isValidDataRow)
        {
            Code = code;
            Name = name;
            EligibilityTypes = eligibilityTypes;
            IsEligibilityManagement = isEligibilityManagement;
            IsValidDataRow = isValidDataRow;
        }
    }

    public class EligibilityTypeProfileRdo
    {
        public string EligibilityTypeCode { get; set; }
        public EligibilityTypeProfileRdo(string eligibilityTypeCode)
        {
            EligibilityTypeCode = eligibilityTypeCode;
        }
    }
}
