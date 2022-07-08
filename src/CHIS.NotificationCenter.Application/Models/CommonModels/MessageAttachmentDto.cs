using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class MessageAttachmentDto
    {
        public string   ContentType     { get; set; }
        public string   Extension       { get; set; }
        public string   FileKey             { get; set; }
        public string   OriginalFileName{ get; set; }
        public string   SavedFileName   { get; set; }
        public string   SavedFilePath   { get; set; }
        public int FileSize { get; set; }
        public string   Url             { get; set; }
    }
}
