using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering
{
    public class EncounterBasicView
    {
        public string Id { get; set; }
        public PatientRdo Patient { get; set; }
        public BusinessItemRdo Status { get; set; }
        public EncounterPatientStatusRdo PatientStatus { get; set; }
        public BusinessItemRdo Class { get; set; }
        public BusinessItemRdo Priority { get; set; }
        public BusinessItemRdo Type { get; set; }
        public BusinessItemRdo Through { get; set; }
        public BusinessItemRdo Purpose { get; set; }
        public BusinessItemRdo Treatment { get; set; }
        public BusinessItemRdo PatientType { get; set; }
        public Boolean IsTreatment { get; set; }
        public DateTime? VisitStartDateTime { get; set; }
        public DateTime? VisitEndDateTime { get; set; }
        public List<ParticipantBasicView> Participant { get; set; }
        public EncounterAdminLocationView AdminLocation { get; set; }
        public EncounterOccupyingLocationView OccupyingLocation { get; set; }
        public OutpatientEncounterDischargeOrderView DischargeOrder { get; set; }
        public string EncounterRemark { get; set; }
        public Boolean IsActivation { get; set; }

        public EncounterBasicView() { }
        public EncounterBasicView(string id, PatientRdo patient, BusinessItemRdo status, EncounterPatientStatusRdo patientStatus, BusinessItemRdo @class,
            BusinessItemRdo priority, BusinessItemRdo type, BusinessItemRdo through, BusinessItemRdo purpose, BusinessItemRdo treatment, BusinessItemRdo patientType,
            bool isTreatment, DateTime? visitStartDateTime, DateTime? visitEndDateTime, List<ParticipantBasicView> participant, EncounterAdminLocationView adminLocation,
            EncounterOccupyingLocationView occupyingLocation, OutpatientEncounterDischargeOrderView dischargeOrder, string encounterRemark, bool isActivation)
        {
            Id = id;
            Patient = patient;
            Status = status;
            PatientStatus = patientStatus;
            Class = @class;
            Priority = priority;
            Type = type;
            Through = through;
            Purpose = purpose;
            Treatment = treatment;
            PatientType = patientType;
            IsTreatment = isTreatment;
            VisitStartDateTime = visitStartDateTime;
            VisitEndDateTime = visitEndDateTime;
            Participant = participant;
            AdminLocation = adminLocation;
            OccupyingLocation = occupyingLocation;
            DischargeOrder = dischargeOrder;
            EncounterRemark = encounterRemark;
            IsActivation = isActivation;
        }
    }

    public class ParticipantBasicView
    {
        public string Id { get; set; }
        public int Sequence { get; set; }
        public BusinessItemRdo Type { get; set; }
        public BusinessItemRdo Position { get; set; }
        public DepartmentBasicView Department { get; set; }
        public EmployeeBasicView Actor { get; set; }
        public ParticipantActivationBasicView Activation { get; set; }
        public Boolean IsValidDataRow { get; set; }

        public ParticipantBasicView()
        {

        }

        public ParticipantBasicView(string id, int sequence, BusinessItemRdo type, BusinessItemRdo position, DepartmentBasicView department, EmployeeBasicView actor,
            ParticipantActivationBasicView activation, bool isValidDataRow)
        {
            Id = id;
            Sequence = sequence;
            Type = type;
            Position = position;
            Department = department;
            Actor = actor;
            Activation = activation;
            IsValidDataRow = isValidDataRow;
        }
    }

    public class EmployeeBasicView
    {
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public string FullName { get; set; }
        public DepartmentBasicView Department { get; set; }

        public EmployeeBasicView()
        {

        }

        public EmployeeBasicView(string id, string displayId, string fullName, DepartmentBasicView department)
        {
            Id = id;
            DisplayId = displayId;
            FullName = fullName;
            Department = department;
        }
    }

    public class ParticipantActivationBasicView
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? WorkDateTime { get; set; }
        public EmployeeBasicView WorkActor { get; set; }

        public ParticipantActivationBasicView()
        {

        }

        public ParticipantActivationBasicView(DateTime? startDateTime, DateTime? endDateTime, DateTime? workDateTime, EmployeeBasicView workActor)
        {
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            WorkDateTime = workDateTime;
            WorkActor = workActor;
        }
    }

    public class DepartmentBasicView
    {
        public string Id { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public DepartmentType DepartmentType { get; set; }
        public Boolean IsVirtual { get; set; }
        public Boolean IsAppointment { get; set; }

        public DepartmentBasicView()
        {

        }

        public DepartmentBasicView(string id, string displayCode, string name, string abbreviation, DepartmentType departmentType, bool isVirtual, bool isAppointment)
        {
            Id = id;
            DisplayCode = displayCode;
            Name = name;
            Abbreviation = abbreviation;
            DepartmentType = departmentType;
            IsVirtual = isVirtual;
            IsAppointment = isAppointment;
        }
    }

    public class LocationBasicView
    {
        public string BedId { get; set; }
        public string FacilityBedId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public RoomBasicRdo Room { get; set; }
        public WardBasicRdo Ward { get; set; }
        public BuildingBasicRdo Building { get; set; }

        public LocationBasicView()
        {

        }

        public LocationBasicView(string bedId, string facilityBedId, string displayCode, string name, RoomBasicRdo room, WardBasicRdo ward, BuildingBasicRdo building)
        {
            BedId = bedId;
            FacilityBedId = facilityBedId;
            DisplayCode = displayCode;
            Name = name;
            Room = room;
            Ward = ward;
            Building = building;
        }
    }

    public class OutpatientEncounterDischargeOrderView
    {
        public string DischargeOrdererId { get; set; }
        public DateTime? DischargeScheduledDatetime { get; set; }
        public DateTime? DischargeOrderDatetime { get; set; }

        public OutpatientEncounterDischargeOrderView()
        {

        }

        public OutpatientEncounterDischargeOrderView(string dischargeOrdererId, DateTime? dischargeScheduledDatetime, DateTime? dischargeOrderDatetime)
        {
            DischargeOrdererId = dischargeOrdererId;
            DischargeScheduledDatetime = dischargeScheduledDatetime;
            DischargeOrderDatetime = dischargeOrderDatetime;
        }
    }
}
