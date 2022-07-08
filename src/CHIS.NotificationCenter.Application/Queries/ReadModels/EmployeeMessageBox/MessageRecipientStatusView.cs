using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{
    public class MessageRecipientStatusView
    {
        public string MessageDispatchItemId { get; set; }
        public string PatientId { get; set; }
        public string EncounterId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public DateTime? SentDateTime { get; set; }

        public List<RecipientMessageView> RecipientMessageViews { get; set; }

    }


}
