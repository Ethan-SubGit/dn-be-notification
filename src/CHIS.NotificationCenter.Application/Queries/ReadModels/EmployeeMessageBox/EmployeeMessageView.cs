using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
namespace CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox
{
    public class EmployeeMessageView
    {
        public string    EmployeeId                       { get; set; }
        public string    MessageInstanceId                { get; set; }
        public DateTime? HandleTime                       { get; set; }
        public bool      IsHandled                        { get; set; }
        public bool      IsInbound                        { get; set; }
        public string    MessageDispatchItemId            { get; set; }
        public string    ServiceCode                      { get; set; }
        public string    MessageCategory                  { get; set; }
        public string    Classification                   { get; set; }
        public string    MessagePriority                  { get; set; }
        public bool      IsReaded                         { get; set; }
        public DateTime? ReadTime                         { get; set; }
        public string    Title                            { get; set; }
        public string    Content                          { get; set; }
        public string    SenderId                         { get; set; }
        public string    SenderName                       { get; set; }
        public string    IntegrationType                  { get; set; }
        public string    IntegrationAddress               { get; set; }
        public string    IntegrationParameter             { get; set; }
        public DateTime? SentTimeStamp                    { get; set; }
        public bool IsSelectPatientByActiveEncounter { get; set; }
        public PostActionType PostActionType { get; set; }
        public bool IsCanceled { get; set; }
        public string Recipient { get; set; }
        public PatientInformationView PatientInformation { get; set; }
        public List<MessageAttachmentView> MessageAttachments { get; set; } = new List<MessageAttachmentView>();

    }
}
