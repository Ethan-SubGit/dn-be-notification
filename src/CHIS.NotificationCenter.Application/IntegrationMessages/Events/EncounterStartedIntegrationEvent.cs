using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;



namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    /// <summary>
    /// read model 로 변경하면서 MQ 수신하지 않음.
    /// 2020.1
    /// </summary>
    public class EncounterStartedIntegrationEvent : IntegrationEvent
    {
        public string EncounterId { get; set; }
        public BusinessItemRdo Status { get; set; }
        public EncounterPatientStatusIntegrationEvent PatientStatus { get; set; }
        public BusinessItemRdo Class { get; set; }
        public BusinessItemRdo Priority { get; set; }
        public string PatientId { get; set; }
        public BusinessItemRdo EncounterType { get; set; }
        public BusinessItemRdo Through { get; set; }
        public BusinessItemRdo Purpose { get; set; }
        public BusinessItemRdo TreatmentType { get; set; }
        public BusinessItemRdo PatientType { get; set; }
        public bool IsTreatment { get; set; }
        public DateTime? OrderDatetime { get; set; }
        //public UtcPack OrderDatetimeUtc { get; set; }
        public DateTime? VisitStartDateTime { get; set; }
        //public UtcPack VisitStartDateTimeUtc { get; set; }
        public DateTime? VisitEndDateTime { get; set; }
        //public UtcPack VisitEndDateTimeUtc { get; set; }
        public BusinessItemRdo ArrivalPathway { get; set; }
        public string CreationNumber { get; set; }
        public List<CheckinIntegrationEvent> Checkins { get; set; }
        public List<ReasonIntegrationEvent> Reasons { get; set; }
        public List<ParticipantIntegrationEvent> Participants { get; set; }
        public List<OncallIntegrationEvent> Oncalls { get; set; }
        public List<AppointmentProfileIntegrationEvent> Appointments { get; set; }
        public List<ClassHistoryIntegrationEvent> ClassHistories { get; set; }
        public List<StatusHistoryIntegrationEvent> StatusHistories { get; set; }
        public EncounterHospitalizationIntegrationEvent Hospitalization { get; set; }
        public EncounterAdminLocationIntegrationEvent AdminLocation { get; set; }
        public EncounterOccupyingLocationIntegrationEvent OccupyLocation { get; set; }
        public EncounterCurrentLocationIntegrationEvent CurrentLocation { get; set; }
        public string EncounterRemark { get; set; }
        public bool IsActivation { get; set; }
        public DateTime? ActivationDatetime { get; set; }
        //public UtcPack ActivationDatetimeUtc { get; set; }
        public DateTime? ScheduleDatetime { get; set; }
        //public UtcPack ScheduleDatetimeUtc { get; set; }
        public bool IsValidDataRow { get; set; }
        public EncounterEmergencyRoomIntegrationEvent EmergencyRoom { get; set; }
        public EncounterDischargeOrderIntegrationEvent DischargeOrder { get; set; }
        public List<EncounterSubstitutionIntegrationEvent> Substitutions { get; set; }
        public EncounterHospitalizationSubstitutionIntegrationEvent HospitalizationSubstitution { get; set; }
        public List<EncounterRecordNoteIntegrationEvent> RecordNotes { get; set; }
        public EncounterMainParticipantPhysicianIntegrationEvent MainParticipantPhysician { get; set; }

        public EncounterDrgStatusIntegrationEvent DrgStatus { get; set; }

        public EncounterStartedIntegrationEvent(string encounterId, BusinessItemRdo status, EncounterPatientStatusIntegrationEvent patientStatus, BusinessItemRdo @class,
            BusinessItemRdo priority, string patientId, BusinessItemRdo encounterType, BusinessItemRdo through, BusinessItemRdo purpose, BusinessItemRdo treatmentType,
            BusinessItemRdo patientType, bool isTreatment, DateTime? orderDatetime, DateTime? visitStartDateTime,
            DateTime? visitEndDateTime, BusinessItemRdo arrivalPathway, string creationNumber, List<ReasonIntegrationEvent> reasons,
            List<CheckinIntegrationEvent> checkins, List<ParticipantIntegrationEvent> participants, List<OncallIntegrationEvent> oncalls,
            List<AppointmentProfileIntegrationEvent> appointments, List<ClassHistoryIntegrationEvent> classHistories, List<StatusHistoryIntegrationEvent> statusHistories,
            EncounterHospitalizationIntegrationEvent hospitalization, EncounterAdminLocationIntegrationEvent adminLocation,
            EncounterOccupyingLocationIntegrationEvent occupyLocation, EncounterCurrentLocationIntegrationEvent currentLocation, string encounterRemark, bool isActivation,
            DateTime? activationDatetime, DateTime? scheduleDatetime, bool isValidDataRow,
            EncounterDischargeOrderIntegrationEvent dischargeOrder, EncounterEmergencyRoomIntegrationEvent emergencyRoom,
            List<EncounterSubstitutionIntegrationEvent> substitutions, EncounterHospitalizationSubstitutionIntegrationEvent hospitalizationSubstitution,
            List<EncounterRecordNoteIntegrationEvent> recordNotes, EncounterMainParticipantPhysicianIntegrationEvent mainParticipantPhysician)
        {
            EncounterId = encounterId;
            Status = status;
            PatientStatus = patientStatus;
            Class = @class;
            Priority = priority;
            PatientId = patientId;
            EncounterType = encounterType;
            Through = through;
            Purpose = purpose;
            TreatmentType = treatmentType;
            PatientType = patientType;
            IsTreatment = isTreatment;
            OrderDatetime = orderDatetime;
            VisitStartDateTime = visitStartDateTime;
            VisitEndDateTime = visitEndDateTime;
            ArrivalPathway = arrivalPathway;
            CreationNumber = creationNumber;
            Reasons = reasons;
            Checkins = checkins;
            Participants = participants;
            Oncalls = oncalls;
            Appointments = appointments;
            ClassHistories = classHistories;
            StatusHistories = statusHistories;
            Hospitalization = hospitalization;
            AdminLocation = adminLocation;
            OccupyLocation = occupyLocation;
            CurrentLocation = currentLocation;
            EncounterRemark = encounterRemark;
            IsActivation = isActivation;
            ActivationDatetime = activationDatetime;
            ScheduleDatetime = scheduleDatetime;
            IsValidDataRow = isValidDataRow;
            EmergencyRoom = emergencyRoom;
            DischargeOrder = dischargeOrder;
            Substitutions = substitutions;
            HospitalizationSubstitution = hospitalizationSubstitution;
            RecordNotes = recordNotes;
            MainParticipantPhysician = mainParticipantPhysician;
        }
    }

    public class EncounterDrgStatusIntegrationEvent
    {
        public bool IsDrgOfPatieht { get; set; }
        public string DrgNumber { get; set; }
        public DateTime? DrgRegistrationDateTime { get; set; }
        public UtcPack DrgRegistrationDateTimeUtc { get; set; }
        public bool IsDrgAdmissionOrder { get; set; }
        public bool IsDrgDiagnosis { get; set; }
        public bool IsDrgSugery { get; set; }
        public bool IsDrgTarget { get; set; }

        public EncounterDrgStatusIntegrationEvent()
        {
        }

        public EncounterDrgStatusIntegrationEvent(bool isPatient, string number, DateTime regDatetime, UtcPack dtUtc, bool isAdmissionOrder, bool isDiagnosis, bool isSugery, bool isTarget)
        {
            IsDrgOfPatieht = isPatient;
            DrgNumber = number;
            DrgRegistrationDateTime = regDatetime;
            DrgRegistrationDateTimeUtc = dtUtc;
            IsDrgAdmissionOrder = isAdmissionOrder;
            IsDrgDiagnosis = isDiagnosis;
            IsDrgSugery = isSugery;
            IsDrgTarget = isTarget;
        }
    }

    public class ReasonIntegrationEvent
    {
        public string ReasonCode { get; set; }

        public ReasonIntegrationEvent() { }
        public ReasonIntegrationEvent(string reasonCode)
        {
            ReasonCode = reasonCode;
        }
    }

    public class CheckinIntegrationEvent
    {
        public string Id { get; set; }
        public int DisplaySequence { get; set; }
        public BusinessItemRdo Status { get; set; }
        public CheckinActivationIntegrationEvent Activation { get; set; }
        public bool IsValidDataRow { get; set; }
        public bool IsLastDataRow { get; set; }

        public CheckinIntegrationEvent() { }
        public CheckinIntegrationEvent(string id, int displaySequence, BusinessItemRdo status, CheckinActivationIntegrationEvent activation, bool isValidDataRow, bool isLastDataRow)
        {
            Id = id;
            DisplaySequence = displaySequence;
            Status = status;
            Activation = activation;
            IsValidDataRow = isValidDataRow;
            IsLastDataRow = isLastDataRow;
        }
    }

    public class CheckinActivationIntegrationEvent
    {
        public DateTime? ActivationDateTime { get; set; }
        public DateTime? WorkDateTime { get; set; }
        public string WorkActorId { get; set; }

        public CheckinActivationIntegrationEvent() { }
        public CheckinActivationIntegrationEvent(DateTime? activationDateTime,
            DateTime? workDateTime, string workActorId)
        {
            ActivationDateTime = activationDateTime;
            WorkDateTime = workDateTime;
            WorkActorId = workActorId;
        }
    }

    public class ParticipantIntegrationEvent
    {
        public string Id { get; set; }
        public int DisplaySequence { get; set; }
        public BusinessItemRdo ParticipantType { get; set; }
        public BusinessItemRdo ParticipantPosition { get; set; }
        public string DepartmentId { get; set; }
        public string ActorId { get; set; }
        public ParticipantActivationIntegrationEvent Activation { get; set; }
        public bool IsValidDataRow { get; set; }

        public ParticipantIntegrationEvent() { }
        public ParticipantIntegrationEvent(string id, int displaySequence, BusinessItemRdo participantType, BusinessItemRdo participantPosition, string departmentId,
            string actorId, ParticipantActivationIntegrationEvent activation, bool isValidDataRow)
        {
            Id = id;
            DisplaySequence = displaySequence;
            ParticipantType = participantType;
            ParticipantPosition = participantPosition;
            DepartmentId = departmentId;
            ActorId = actorId;
            Activation = activation;
            IsValidDataRow = isValidDataRow;
        }
    }

    public class ParticipantActivationIntegrationEvent
    {
        public DateTime? AssessmentDateTime { get; set; }
        public DateTime? WorkDateTime { get; set; }
        public string WorkActorId { get; set; }

        public ParticipantActivationIntegrationEvent() { }
        public ParticipantActivationIntegrationEvent(DateTime? assessmentDateTime,
            DateTime? workDateTime, string workActorId)
        {
            AssessmentDateTime = assessmentDateTime;
            WorkDateTime = workDateTime;
            WorkActorId = workActorId;
        }
    }

    public class OncallIntegrationEvent
    {
        public string Id { get; set; }
        public int DisplaySequence { get; set; }
        public BusinessItemRdo OncallType { get; set; }
        public string DepartmentId { get; set; }
        public string ActorId { get; set; }
        public bool IsValidDataRow { get; set; }
        public OncallActivationIntegrationEvent Activation { get; set; }

        public OncallIntegrationEvent() { }
        public OncallIntegrationEvent(string id, int displaySequence, BusinessItemRdo oncallType, string departmentId, string actorId, bool isValidDataRow,
            OncallActivationIntegrationEvent activation)
        {
            Id = id;
            DisplaySequence = displaySequence;
            OncallType = oncallType;
            DepartmentId = departmentId;
            ActorId = actorId;
            IsValidDataRow = isValidDataRow;
            Activation = activation;
        }
    }

    public class OncallActivationIntegrationEvent
    {
        public DateTime? OncallDateTime { get; set; }
        public DateTime? AssessmentDateTime { get; set; }
        public DateTime? WorkDateTime { get; set; }
        public string WorkActorId { get; set; }

        public OncallActivationIntegrationEvent() { }
        public OncallActivationIntegrationEvent(DateTime? oncallDateTime, DateTime? assessmentDateTime,
            DateTime? workDateTime, string workActorId)
        {
            OncallDateTime = oncallDateTime;
            AssessmentDateTime = assessmentDateTime;
            WorkDateTime = workDateTime;
            WorkActorId = workActorId;
        }
    }

    public class AppointmentProfileIntegrationEvent
    {
        public string Id { get; set; }
        public string AppointmentId { get; set; }
        public bool IsValidDataRow { get; set; }

        public AppointmentProfileIntegrationEvent() { }
        public AppointmentProfileIntegrationEvent(string id, string appointmentId, bool isValidDataRow)
        {
            Id = id;
            AppointmentId = appointmentId;
            IsValidDataRow = isValidDataRow;
        }
    }

    public class ClassHistoryIntegrationEvent
    {
        public string Id { get; set; }
        public string ClassCode { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public ClassHistoryIntegrationEvent() { }
        public ClassHistoryIntegrationEvent(string id, string classCode, DateTime? startDateTime,
            DateTime? endDateTime)
        {
            Id = id;
            ClassCode = classCode;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }
    }

    public class StatusHistoryIntegrationEvent
    {
        public string Id { get; set; }
        public string StatusCode { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public StatusHistoryIntegrationEvent() { }
        public StatusHistoryIntegrationEvent(string id, string statusCode, DateTime? startDateTime
             , DateTime? endDateTime)
        {
            Id = id;
            StatusCode = statusCode;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }
    }

    public class EncounterHospitalizationIntegrationEvent
    {
        public string PreAdmissionId { get; set; }
        public BusinessItemRdo AdmissionSource { get; set; }
        public BusinessItemRdo ReAdmissionReason { get; set; }
        public BusinessItemRdo DietPreference { get; set; }
        public BusinessItemRdo SpecialCourtesy { get; set; }
        public BusinessItemRdo SpecialArrangement { get; set; }
        public DateTime? WardArrivalDatetime { get; set; }
        public DateTime? CheckinAvaliableDatetime { get; set; }
        public DateTime? EmergencyRoomArrivalDatetime { get; set; }
        public bool IsFast { get; set; }
        public BusinessItemRdo MealDivision { get; set; }
        public bool IsDrg { get; set; }
        public string AdmissionDiseaseId { get; set; }
        public string AdmissionDIseaseComments { get; set; }
        public string PhysicianComments { get; set; }
        public int Elos { get; set; }
        public BusinessItemRdo AdmissionType { get; set; }
        public string AdmissionPlanId { get; set; }

        public EncounterHospitalizationIntegrationEvent() { }

        public EncounterHospitalizationIntegrationEvent(string preAdmissionId, BusinessItemRdo admissionSource, BusinessItemRdo reAdmissionReason,
            BusinessItemRdo dietPreference, BusinessItemRdo specialCourtesy, BusinessItemRdo specialArrangement, DateTime? wardArrivalDatetime,
            DateTime? checkinAvaliableDatetime, DateTime? emergencyRoomArrivalDatetime,
            bool isFast, BusinessItemRdo mealDivision, bool isDrg, string admissionDiseaseId, string admissionDIseaseComments,
            string physicianComments, int elos, BusinessItemRdo admissionType, string admissionPlanId)
        {
            PreAdmissionId = preAdmissionId;
            AdmissionSource = admissionSource;
            ReAdmissionReason = reAdmissionReason;
            DietPreference = dietPreference;
            SpecialCourtesy = specialCourtesy;
            SpecialArrangement = specialArrangement;
            WardArrivalDatetime = wardArrivalDatetime;
            CheckinAvaliableDatetime = checkinAvaliableDatetime;
            EmergencyRoomArrivalDatetime = emergencyRoomArrivalDatetime;
            IsFast = isFast;
            MealDivision = mealDivision;
            IsDrg = isDrg;
            AdmissionDiseaseId = admissionDiseaseId;
            AdmissionDIseaseComments = admissionDIseaseComments;
            PhysicianComments = physicianComments;
            Elos = elos;
            AdmissionType = admissionType;
            AdmissionPlanId = admissionPlanId;
        }
    }

    public class EncounterAdminLocationIntegrationEvent
    {
        public LocationBasicRdo AdminLocation { get; set; }
        public BusinessItemRdo PhysicalType { get; set; }
        public BusinessItemRdo LocationStatus { get; set; }
        public BusinessItemRdo ServiceRoleType { get; set; }
        public BusinessItemRdo LocationClass { get; set; }
        public BusinessItemRdo LocationType { get; set; }
        public BusinessItemRdo PatientService { get; set; }
        public DateTime? CheckinDatetime { get; set; }
        public DateTime? CheckoutDatetime { get; set; }
        public LocationBasicRdo AdminDualLocation { get; set; }
        public BusinessItemRdo EmergencyMedicalArea { get; set; }

        public EncounterAdminLocationIntegrationEvent() { }
        public EncounterAdminLocationIntegrationEvent(LocationBasicRdo adminLocation, BusinessItemRdo physicalType, BusinessItemRdo locationStatus,
            BusinessItemRdo serviceRoleType, BusinessItemRdo locationClass, BusinessItemRdo locationType, BusinessItemRdo patientService, DateTime? checkinDatetime,
            DateTime? checkoutDatetime, LocationBasicRdo adminDualLocation, BusinessItemRdo emergencyMedicalArea)
        {
            AdminLocation = adminLocation;
            PhysicalType = physicalType;
            LocationStatus = locationStatus;
            ServiceRoleType = serviceRoleType;
            LocationClass = locationClass;
            LocationType = locationType;
            PatientService = patientService;
            CheckinDatetime = checkinDatetime;
            CheckoutDatetime = checkoutDatetime;
            AdminDualLocation = adminDualLocation;
            EmergencyMedicalArea = emergencyMedicalArea;
        }
    }

    public class EncounterOccupyingLocationIntegrationEvent
    {
        public LocationBasicRdo OccupyLocation { get; set; }
        public BusinessItemRdo LocationStatus { get; set; }
        public DateTime? CheckinDatetime { get; set; }
        public DateTime? CheckoutDatetime { get; set; }
        public BusinessItemRdo EmergencyMedicalArea { get; set; }

        public EncounterOccupyingLocationIntegrationEvent() { }
        public EncounterOccupyingLocationIntegrationEvent(LocationBasicRdo occupyLocation, BusinessItemRdo locationStatus, DateTime? checkinDatetime,
               DateTime? checkoutDatetime, BusinessItemRdo emergencyMedicalArea)
        {
            OccupyLocation = occupyLocation;
            LocationStatus = locationStatus;
            CheckinDatetime = checkinDatetime;
            CheckoutDatetime = checkoutDatetime;
            EmergencyMedicalArea = emergencyMedicalArea;
        }
    }

    public class EncounterCurrentLocationIntegrationEvent
    {
        public LocationBasicRdo CurrentLocation { get; set; }
        public BusinessItemRdo LocationStatus { get; set; }
        public DateTime? CheckinDatetime { get; set; }
        public DateTime? CheckoutDatetime { get; set; }

        public EncounterCurrentLocationIntegrationEvent() { }
        public EncounterCurrentLocationIntegrationEvent(LocationBasicRdo currentLocation, BusinessItemRdo locationStatus, DateTime? checkinDatetime,
               DateTime? checkoutDatetime)
        {
            CurrentLocation = currentLocation;
            LocationStatus = locationStatus;
            CheckinDatetime = checkinDatetime;
            CheckoutDatetime = checkoutDatetime;
        }
    }

    public class EncounterDischargeOrderIntegrationEvent
    {
        public string DischargeOrdererId { get; set; }
        public DateTime? DischargeScheduledDatetime { get; set; }
        public DateTime? DischargeOrderDatetime { get; set; }

        public EncounterDischargeOrderIntegrationEvent() { }
        public EncounterDischargeOrderIntegrationEvent(string dischargeOrdererId,
            DateTime? dischargeScheduledDatetime, DateTime? dischargeOrderDatetime)
        {
            DischargeOrdererId = dischargeOrdererId;
            DischargeScheduledDatetime = dischargeScheduledDatetime;
            DischargeOrderDatetime = dischargeOrderDatetime;
        }
    }

    public class EncounterEmergencyRoomIntegrationEvent
    {
        public BusinessItemRdo PatientClassification { get; set; }
        public bool IsCalculationClosing { get; set; }

        public EncounterEmergencyRoomIntegrationEvent() { }
        public EncounterEmergencyRoomIntegrationEvent(BusinessItemRdo patientClassification, Boolean isCalculationClosing)
        {
            PatientClassification = patientClassification;
            IsCalculationClosing = isCalculationClosing;
        }
    }

    public class EncounterSubstitutionIntegrationEvent
    {
        public string SubstitutionEncounterId { get; set; }
        public string SubstitutionEncounterClassCode { get; set; }

        public EncounterSubstitutionIntegrationEvent() { }
        public EncounterSubstitutionIntegrationEvent(string substitutionEncounterId, string substitutionEncounterClassCode)
        {
            SubstitutionEncounterId = substitutionEncounterId;
            SubstitutionEncounterClassCode = substitutionEncounterClassCode;
        }
    }

    public class EncounterHospitalizationSubstitutionIntegrationEvent
    {
        public string AdmissionEncounterId { get; set; }
        public DateTime? SubstitutionWorkDatetime { get; set; }
        public string SubstitutionWorkId { get; set; }

        public EncounterHospitalizationSubstitutionIntegrationEvent() { }
        public EncounterHospitalizationSubstitutionIntegrationEvent(string admissionEncounterId,
            DateTime? substitutionWorkDatetime, string substitutionWorkId)
        {
            AdmissionEncounterId = admissionEncounterId;
            SubstitutionWorkDatetime = substitutionWorkDatetime;
            SubstitutionWorkId = substitutionWorkId;
        }
    }

    public class EncounterRecordNoteIntegrationEvent
    {
        public string RecordNoteId { get; set; }

        public EncounterRecordNoteIntegrationEvent() { }
        public EncounterRecordNoteIntegrationEvent(string recordNoteId)
        {
            RecordNoteId = recordNoteId;
        }
    }

    public class EncounterPatientStatusIntegrationEvent
    {
        public bool IsInHospital { get; set; }
        public bool IsDischargeOrder { get; set; }
        public bool IsDischargeNurseReview { get; set; }
        public bool IsMedicalRecordReview { get; set; }
        public string ReviewStatusCode { get; set; }
        public bool IsDischargeBillPrint { get; set; }
        public bool IsDischarge { get; set; }
        public string DischargeProgressStatusCode { get; set; }

        public EncounterPatientStatusIntegrationEvent() { }

        public EncounterPatientStatusIntegrationEvent(bool isInHospital, bool isDischargeOrder, bool isDischargeNurseReview, bool isMedicalRecordReview,
            string reviewStatusCode, bool isDischargeBillPrint, bool isDischarge, string dischargeProgressStatusCode)
        {
            IsInHospital = isInHospital;
            IsDischargeOrder = isDischargeOrder;
            IsDischargeNurseReview = isDischargeNurseReview;
            IsMedicalRecordReview = isMedicalRecordReview;
            ReviewStatusCode = reviewStatusCode;
            IsDischargeBillPrint = isDischargeBillPrint;
            IsDischarge = isDischarge;
            DischargeProgressStatusCode = dischargeProgressStatusCode;
        }
    }

    public class EncounterMainParticipantPhysicianIntegrationEvent
    {
        public string AdmittingPhysicianId { get; set; }
        public string AttendingDepartmentId { get; set; }
        public string AttendingPhysicianId { get; set; }
        public string PrimaryCarePhysicianId { get; set; }

        public EncounterMainParticipantPhysicianIntegrationEvent() { }

        public EncounterMainParticipantPhysicianIntegrationEvent(string admittingPhysicianId, string attendingDepartmentId,
            string attendingPhysicianId, string primaryCarePhysicianId)
        {
            AdmittingPhysicianId = admittingPhysicianId;
            AttendingDepartmentId = attendingDepartmentId;
            AttendingPhysicianId = attendingPhysicianId;
            PrimaryCarePhysicianId = primaryCarePhysicianId;
        }
    }

    public class BusinessItemRdo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public BusinessItemRdo() { }

        public BusinessItemRdo(string id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }

        public BusinessItemRdo(string code)
        {
            Code = code;
        }
    }

    public class LocationBasicRdo
    {
        public BedBasicRdo Bed { get; set; }
        public RoomBasicRdo Room { get; set; }
        public WardBasicRdo Ward { get; set; }
        public BuildingBasicRdo Building { get; set; }

        public LocationBasicRdo() { }
        public LocationBasicRdo(BedBasicRdo bed, RoomBasicRdo room, WardBasicRdo ward, BuildingBasicRdo building)
        {
            Bed = bed;
            Room = room;
            Ward = ward;
            Building = building;
        }
    }

    public class BedBasicRdo
    {
        public string BedId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public string FacilityBedId { get; set; }

        public BedBasicRdo() { }
        public BedBasicRdo(string bedId, string displayCode, string name, string facilityBedId)
        {
            BedId = bedId;
            DisplayCode = displayCode;
            Name = name;
            FacilityBedId = facilityBedId;
        }
    }

    public class RoomBasicRdo
    {
        public string RoomId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }

        public RoomBasicRdo() { }
        public RoomBasicRdo(string roomId, string displayCode, string name)
        {
            RoomId = roomId;
            DisplayCode = displayCode;
            Name = name;
        }
    }

    public class WardBasicRdo
    {
        public string WardId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }

        public WardBasicRdo() { }
        public WardBasicRdo(string wardId, string displayCode, string name)
        {
            WardId = wardId;
            DisplayCode = displayCode;
            Name = name;
        }
    }

    public class BuildingBasicRdo
    {
        public string BuildingId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }

        public BuildingBasicRdo() { }
        public BuildingBasicRdo(string buildingId, string displayCode, string name)
        {
            BuildingId = buildingId;
            DisplayCode = displayCode;
            Name = name;
        }
    }
}

