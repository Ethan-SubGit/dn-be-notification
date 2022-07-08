using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.SmsMonitoring
{
    public class SmsResultPatientPackageView
    {
        public List<SmsResultPatientView> SmsResultPatientViews { get; set; }
        public int TotalRecordCount { get; set; }
    }
}
