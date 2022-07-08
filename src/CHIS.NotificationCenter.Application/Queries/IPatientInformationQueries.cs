using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Models.QueryType;

namespace CHIS.NotificationCenter.Application.Queries
{
    public interface IPatientInformationQueries
    {
        Task<PatientInformationView> RetrievePatientInfomation(string patientId, string encounterId);
        Task<PatientInformationView> RetrievePatientInfomationV2(string patientId, string encounterId);
        Task<SmsRecipientDto> FindPatientContact(string patientId, SmsRecipientType smsRecipientType);
        Task<List<SmsRecipientDto>> FindPatientContact(string patientId);
        Task<SmsRecipientDto> FindPatientContact(string patientId, string relationShipCode, string classificationCode);
        Task<List<SmsRecipientDto>> SearchPatientContacts(List<string> patientIds);
        Task<List<EncounterParticipantDto>> RetrievePatientParticipant(string encounterId);

        PatientReadModel RetrievePatientInfomationWithPatientId(string patientId);
        Task<List<PatientInformationView>> FindPatientListForInbox(string employeeId);
    }
}
