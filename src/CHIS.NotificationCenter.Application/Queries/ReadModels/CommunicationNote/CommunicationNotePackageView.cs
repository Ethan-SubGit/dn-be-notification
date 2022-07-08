using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.CommunicationNote
{
    public class CommunicationNotePackageView
    {
        public IList<CommunicationNoteView> CommunicationNotes { get; set; }

        public int UnreadCount { get; set; }

        public int TotalCount { get; set; }
        //public IList<CommunicationNoteCountView> Counts { get; set; }

    }
}
