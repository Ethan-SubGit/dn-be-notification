using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
using CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.CommunicationNote
{
    public class CommunicationNoteView
    {
        #region property
        public string EmployeeId { get; set; }
        public string MessageInstanceId { get; set; }
        public bool IsInbound { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadTime { get; set; }
        public string MessageDispatchItemId { get; set; }
        public string ServiceCode { get; set; }
        public string MessageCategory { get; set; }
        public string Classification { get; set; }
        public string MessagePriority { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string SenderDisplayId { get; set; }
        public string SenderName { get; set; }
        public string OccupationName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? SentTimeStamp { get; set; }
        public bool IsCanceled { get; set; }

        public PatientInformationView PatientInformation { get; set; }

        public int RecipientsReadCount { get; set; }

        /// <summary>
        /// 수신자 수
        /// </summary>
        public int RecipientsCount { get; set; }

        /// <summary>
        /// 수신자 대표 1명
        /// </summary>
        public string RecipientName { get; set; }
        public IList<CommunicationNoteRecipientView> CommunicationNoteRecipientViews { get; set; }

        public int AttachmentsCount { get; set; }
        public List<MessageAttachmentView> MessageAttachments { get; set; } 
        #endregion

    }
}
