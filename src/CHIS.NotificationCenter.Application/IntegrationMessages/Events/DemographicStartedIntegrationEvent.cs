using System;
using System.Collections.Generic;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    /// <summary>
    /// read model 로 변경하면서 MQ 수신하지 않음.
    /// 2020.1
    /// </summary>
    public class DemographicStartedIntegrationEvent : IntegrationEvent
    {

        #region property

        public string DemographicId { get; set; }
        public string GenderCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public string NationalityCode { get; set; }
        public string MaritalStatusCode { get; set; }
        public string RaceCode { get; set; }
        public string EthnicityCode { get; set; }
        public string ReligionCode { get; set; }
        public string EmploymentStatusCode { get; set; }
        public string OccupationCode { get; set; }
        public string PreferenceLanguageCode { get; set; }
        public PatientIntegrationEvent Patient { get; set; }
        public IdentifierIntegrationEvent Identifier { get; set; }
        public PatientNameIntegrationEvent PatientName { get; set; }
        public List<ContactIntegrationEvent> Contacts { get; set; }
        public PatientPhotoIntegrationEvent Photo { get; set; }
        public ActionEventTypes ActionEventType { get; set; }
        public DeceaseRecordIntegration DeceaseRecord { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; } 
        #endregion
        


        public DemographicStartedIntegrationEvent(string demographicId, string genderCode, DateTime? birthDate, string nationalityCode,
            string maritalStatusCode, string raceCode, string ethnicityCode, string religionCode, string employmentStatusCode, string occupationCode,
            string preferenceLanguageCode, PatientIntegrationEvent patient, IdentifierIntegrationEvent identifier, PatientNameIntegrationEvent patientName,
            List<ContactIntegrationEvent> contacts, PatientPhotoIntegrationEvent photo, ActionEventTypes actionEventType, DeceaseRecordIntegration deceaseRecord, string tenantId, string hospitalId)
        {
            DemographicId = demographicId;
            GenderCode = genderCode;
            BirthDate = birthDate;
            NationalityCode = nationalityCode;
            MaritalStatusCode = maritalStatusCode;
            RaceCode = raceCode;
            EthnicityCode = ethnicityCode;
            ReligionCode = religionCode;
            EmploymentStatusCode = employmentStatusCode;
            OccupationCode = occupationCode;
            PreferenceLanguageCode = preferenceLanguageCode;
            Patient = patient;
            Identifier = identifier;
            PatientName = patientName;
            Contacts = contacts;
            Photo = photo;
            ActionEventType = actionEventType;
            DeceaseRecord = deceaseRecord;
            TenantId = tenantId;
            HospitalId = hospitalId;
        }
    }

    public class ActionEventTypes
    {
    }

    public class DeceaseRecordIntegration
    {
        #region Property
        /// <summary>
        /// DeceasedDatetime
        /// </summary>
        public DateTime? DeceasedDatetime { get; set; }
        /// <summary>
        /// RecordDatetime
        /// </summary>
        public DateTime? RecordDatetime { get; set; }
        /// <summary>
        /// WriterId
        /// </summary>
        public string WriterId { get; set; }
        #endregion

     

        public DeceaseRecordIntegration(DateTime? deceasedDatetime, DateTime? recordDatetime, string writerId)
        {
            DeceasedDatetime = deceasedDatetime;
            RecordDatetime = recordDatetime;
            WriterId = writerId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PatientIntegrationEvent
    {

        #region property
        public string PatientId { get; set; }
        public string TenantPatientId { get; set; }
        public string DisplayId { get; set; }
        public string MotherPatientId { get; set; }
        public string SequenceBuilderId { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public FileCloseIntegration FileClose { get; set; }
        public PatientServiceApplicationFormIntegration PatientServiceApplicationForm { get; set; }

        #endregion
       

        public PatientIntegrationEvent(string patientId, string tenantPatientId, string displayId, string motherPatientId, string sequenceBuilderId,
            string tenantId, string hospitalId, FileCloseIntegration fileClose, PatientServiceApplicationFormIntegration patientServiceApplicationForm)
        {
            PatientId = patientId;
            TenantPatientId = tenantPatientId;
            DisplayId = displayId;
            MotherPatientId = motherPatientId;
            SequenceBuilderId = sequenceBuilderId;
            TenantId = tenantId;
            HospitalId = hospitalId;
            FileClose = fileClose;
            PatientServiceApplicationForm = patientServiceApplicationForm;
        }

    }

    public class PatientServiceApplicationFormIntegration
    {
        #region property
        public bool IsPersonalInfo { get; set; }
        public DateTime? PersonalInfoDatetime { get; set; }
        public UtcPack PersonalInfoDatetimeUtc { get; set; }
        public bool IsProtectionHealthInfo { get; set; }
        public DateTime? ProtectionHealthInfoDatetime { get; set; }
        public UtcPack ProtectionHealthInfoDatetimeUtc { get; set; }
        public bool IsPrivacyProtection { get; set; }
        public DateTime? PrivacyProtectionDatetime { get; set; }
        public UtcPack PrivacyProtectionDatetimeUtc { get; set; } 
        #endregion

        public PatientServiceApplicationFormIntegration()
        {

        }

        public PatientServiceApplicationFormIntegration(bool isPersonalInfo, DateTime? personalInfoDatetime, UtcPack personalInfoDatetimeUtc,
            bool isProtectionHealthInfo, DateTime? protectionHealthInfoDatetime, UtcPack protectionHealthInfoDatetimeUtc, bool isPrivacyProtection,
            DateTime? privacyProtectionDatetime, UtcPack privacyProtectionDatetimeUtc)
        {
            IsPersonalInfo = isPersonalInfo;
            PersonalInfoDatetime = personalInfoDatetime;
            PersonalInfoDatetimeUtc = personalInfoDatetimeUtc;
            IsProtectionHealthInfo = isProtectionHealthInfo;
            ProtectionHealthInfoDatetime = protectionHealthInfoDatetime;
            ProtectionHealthInfoDatetimeUtc = protectionHealthInfoDatetimeUtc;
            IsPrivacyProtection = isPrivacyProtection;
            PrivacyProtectionDatetime = privacyProtectionDatetime;
            PrivacyProtectionDatetimeUtc = privacyProtectionDatetimeUtc;
        }
    }

    public class FileCloseIntegration
    {
        public bool IsClosed { get; private set; }
        public DateTime? CloseDate { get; private set; }
        //public BusinessTypeItemRdo PatientFileCloseReason { get; private set; }
        //public BusinessItemRdo PatientFileCloseReason { get; private set; }
        public DeathPatientIntegration DeathPatient { get; private set; }
        public BusinessItemIntegration PatientFileCloseReason { get; set; }

       
        public FileCloseIntegration(bool isClosed, DateTime? closeDate, BusinessItemIntegration patientFileCloseReason)
        {
            IsClosed = isClosed;
            CloseDate = closeDate;
            PatientFileCloseReason = patientFileCloseReason;
        }
    }

    public class DeathPatientIntegration
    {
        public string DeathSubType { get; set; }
        public string DeathSubTypeName { get; set; }
        public DateTime? DeathDatetime { get; set; }
        public DeathPatientIntegration()
        {

        }
    }

    /// <summary>
    /// 사망코드, 코드명
    /// 미사용
    /// </summary>
    public class BusinessItemIntegration
    {
        public string Id { get; private set; }
        public string Code { get; set; }
        public string Name { get; set; }


        public BusinessItemIntegration(string id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }
    }

    public class IdentifierIntegrationEvent
    {


        #region property
        public int DisplaySequence { get; set; }
        public string IdentifierCategoryId { get; set; }
        public string IdentifierSectionCode { get; set; }
        public string TypeCode { get; set; }
        public string DisplayNumber { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public UtcPack ExpiryDateUtc { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; } 
        #endregion
       

        public IdentifierIntegrationEvent(int displaySequence, string identifierCategoryId, string identifierSectionCode, string typeCode,
            string displayNumber, bool isPrimary, DateTime? expiryDate, UtcPack expiryDateUtc, string tenantId, string hospitalId)
        {
            DisplaySequence = displaySequence;
            IdentifierCategoryId = identifierCategoryId;
            IdentifierSectionCode = identifierSectionCode;
            TypeCode = typeCode;
            DisplayNumber = displayNumber;
            IsPrimary = isPrimary;
            ExpiryDate = expiryDate;
            ExpiryDateUtc = expiryDateUtc;
            TenantId = tenantId;
            HospitalId = hospitalId;
        }
    }

    public class PatientNameIntegrationEvent
    {
        public int DisplaySequence { get; private set; }
        public string NamingCategoryId { get; private set; }
        public string NamingSectionCode { get; private set; }
        public string FullName { get; private set; }
        public bool IsPrimary { get; private set; }

        public PatientNameIntegrationEvent(int displaySequence, string namingCategoryId, string namingSectionCode, string fullName, bool isPrimary)
        {
            DisplaySequence = displaySequence;
            NamingCategoryId = namingCategoryId;
            NamingSectionCode = namingSectionCode;
            FullName = fullName;
            IsPrimary = isPrimary;
        }
    }

    public class ContactIntegrationEvent
    {
        #region property
        public int DisplaySequence { get; private set; }
        public string RelationshipCode { get; private set; }
        public string Name { get; private set; }
        public string ClassificationCode { get; private set; }
        public string Email { get; private set; }
        public List<ContactTelephoneIntegrationEvent> Telephones { get; private set; }
        public List<ContactAddressIntegrationEvent> Addresses { get; private set; } 
        #endregion

        public ContactIntegrationEvent(int displaySequence, string relationshipCode, string name, string classificationCode, string email,
            List<ContactTelephoneIntegrationEvent> telephones, List<ContactAddressIntegrationEvent> addresses)
        {
            DisplaySequence = displaySequence;
            RelationshipCode = relationshipCode;
            Name = name;
            ClassificationCode = classificationCode;
            Email = email;
            Telephones = telephones;
            Addresses = addresses;
        }
    }

    public class ContactTelephoneIntegrationEvent
    {
        public int DisplaySequence { get; private set; }
        public string ClassificationCode { get; private set; }
        public string PhoneNumber { get; private set; }

        public ContactTelephoneIntegrationEvent(int displaySequence, string classificationCode, string phoneNumber)
        {
            DisplaySequence = displaySequence;
            ClassificationCode = classificationCode;
            PhoneNumber = phoneNumber;
        }
    }

    public class ContactAddressIntegrationEvent
    {



        #region property
        public int DisplaySequence { get; set; }
        public string ClassificationCode { get; set; }
        public string PostalCodeNumber { get; set; }
        public string BasicAddress { get; set; }
        public string DetailAddress { get; set; }
        public string EnglishBasicAddress { get; set; }
        public string LotNumberAddress { get; set; }
        public string AddressOptionTypeCode { get; set; }
        public DateTime? AddressApiCallDatetime { get; set; }
        public UtcPack AddressApiCallDatetimeUtc { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; } 
        #endregion

       

        public ContactAddressIntegrationEvent(int displaySequence, string classificationCode, string postalCodeNumber, string basicAddress,
            string detailAddress, string englishBasicAddress, string lotNumberAddress, string addressOptionTypeCode,
            DateTime? addressApiCallDatetime, UtcPack addressApiCallDatetimeUtc, string tenantId, string hospitalId)
        {
            DisplaySequence = displaySequence;
            ClassificationCode = classificationCode;
            PostalCodeNumber = postalCodeNumber;
            BasicAddress = basicAddress;
            DetailAddress = detailAddress;
            EnglishBasicAddress = englishBasicAddress;
            LotNumberAddress = lotNumberAddress;
            AddressOptionTypeCode = addressOptionTypeCode;
            AddressApiCallDatetime = addressApiCallDatetime;
            AddressApiCallDatetimeUtc = addressApiCallDatetimeUtc;
            TenantId = tenantId;
            HospitalId = hospitalId;
        }
    }

    public class PatientPhotoIntegrationEvent
    {
        public string ImageFilePath { get; private set; }

        public PatientPhotoIntegrationEvent()
        {
            //
        }

        public PatientPhotoIntegrationEvent(string imageFilePath)
        {
            ImageFilePath = imageFilePath;
        }
    }

}
