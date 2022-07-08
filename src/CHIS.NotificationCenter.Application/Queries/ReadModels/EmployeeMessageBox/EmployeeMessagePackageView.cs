using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{
    public class EmployeeMessagePackageView
    {
        public IList<EmployeeMessageView> EmployeeMessages { get; set; }

        public IList<MessageCountView> MessageCounts { get; set; }

    }
}
