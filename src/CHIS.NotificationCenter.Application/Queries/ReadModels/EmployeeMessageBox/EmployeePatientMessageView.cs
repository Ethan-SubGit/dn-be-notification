using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{
    /// <summary>
    /// Inbox 환자별 보기  View
    /// </summary>
    public class EmployeePatientMessageView
    {
        public int DocumentUnHandled { get; set; }
        public int DocumentHandled   { get; set; }
        public int DocumentAll       { get; set; }
        public int ExamUnHandled { get; set; }
        public int ExamHandled { get; set; }
        public int ExamAll { get; set; }
        public int OrderUnHandled { get; set; }
        public int OrderHandled { get; set; }
        public int OrderAll { get; set; }

        public PatientInformationView PatientInformation { get; set; }
    }
}
