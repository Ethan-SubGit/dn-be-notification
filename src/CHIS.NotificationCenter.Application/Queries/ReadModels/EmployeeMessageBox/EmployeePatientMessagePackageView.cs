using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{
    /// <summary>
    /// 인박스 환자별 보기 리스트 View
    /// </summary>
    public class EmployeePatientMessagePackageView
    {
        public int TotalRecordCount { get; set; }

        public IList<EmployeePatientMessageView> EmployeePatientMessages { get; set; }

        public IList<MessageCountView> MessageCounts { get; set; }

    }
}
