using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.Enum
{
    public enum SmsProgressStatus
    {
        BeforeProgress  = 0
        , InProgress      = 1
        , Error           = 2
        , Completed       = 3
    }
}
