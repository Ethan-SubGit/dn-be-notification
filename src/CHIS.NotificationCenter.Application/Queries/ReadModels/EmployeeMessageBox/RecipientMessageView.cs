using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{
    public class RecipientMessageView
    {
        public string    MessageDispatchItemId  { get; set; }
        public string    MessageInstanceId      { get; set; }
        public bool      IsHandled              { get; set; }
        public DateTime? HandleTime             { get; set; }
        public bool      IsReaded               { get; set; }
        public DateTime? ReadTime               { get; set; }
        public string    EmployeeId             { get; set; }
        public string    EmployeeName           { get; set; }
    }
}
