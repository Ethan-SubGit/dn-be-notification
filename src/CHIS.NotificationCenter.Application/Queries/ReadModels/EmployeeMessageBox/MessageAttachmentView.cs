using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{
    public class MessageAttachmentView
    {
        public string   MessageAttachmentId { get; set; }
        public string   ContentType         { get; set; }
        public string   Extension           { get; set; }
        public string   FileKey                 { get; set; }
        public string   OriginalFileName    { get; set; }
        public string   SavedFileName       { get; set; }
        public string   SavedFilePath       { get; set; }
        public decimal  FileSize                { get; set; }
        public string   Url                 { get; set; }
    }
}
