using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.Enum
{
    public enum SmsRecipientType
    {
        Employee = 0,
        Patient = 1,
        Guardian = 2,
        EmployeeDirectMobile= 7,
        PatientDirectMobile = 8,
        Other = 9, // 사용안함
        
    }
}
