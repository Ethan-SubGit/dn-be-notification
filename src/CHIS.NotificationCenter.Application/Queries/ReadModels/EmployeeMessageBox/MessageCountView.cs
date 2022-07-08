using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{


    public class MessageCountView
    {
        public string MessageCategory { get; set; }
        public bool IsHandled { get; set; }
        public int MessageCount { get; set; }
    }
   
}
