using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.CommunicationNote
{


    public class CommunicationNoteCountView
    {
        public string MessageCategory { get; set; }
        public int ReadCount { get; set; }
        public int TotalCount { get; set; }
    }
   
}
