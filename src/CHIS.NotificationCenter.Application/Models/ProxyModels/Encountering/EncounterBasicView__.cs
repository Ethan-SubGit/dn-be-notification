//using CHIS.NotificationCenter.Application.Models.DepartmentAggregate;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels
{
    public class EncounterBasicView
    {
        public string Id { get; set; }
        public string PatientId { get; set; }
        public BusinessItemView Status { get; set; }
        public BusinessItemView Class { get; set; }
        public BusinessItemView Priority { get; set; }
        public BusinessItemView Type { get; set; }
        public BusinessItemView Through { get; set; }
        public BusinessItemView Purpose { get; set; }
        public BusinessItemView Treatment { get; set; }
        public BusinessItemView PatientType { get; set; }
        public Boolean IsTreatment { get; set; }
        public DateTime? VisitStartDateTime { get; set; }
        public DateTime? VisitEndDateTime { get; set; }
        public List<ParticipantBasicView> Participant { get; set; }
        public EncounterAdminLocationView AdminLocation { get; private set; }
        public EncounterOccupyingLocationView OccupyingLocation { get; private set; }
        public OutpatientEncounterDischargeView Discharge { get; set; }
        public string EncounterRemark { get; set; }
        public Boolean IsActivation { get; set; }
        public EncounterBasicView()
        {

        }

        public EncounterBasicView(string id, string patientId, BusinessItemView status, BusinessItemView @class, BusinessItemView priority, BusinessItemView type, BusinessItemView through,
            BusinessItemView purpose, BusinessItemView treatment, BusinessItemView patientType, bool isTreatment, DateTime? visitStartDateTime, DateTime? visitEndDateTime,
            List<ParticipantBasicView> participant, EncounterAdminLocationView adminLocation, EncounterOccupyingLocationView occupyingLocation, OutpatientEncounterDischargeView discharge,
            string encounterRemark, bool isActivation)
        {
            Id = id;
            PatientId = patientId;
            Status = status;
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
            Discharge = discharge;
            EncounterRemark = encounterRemark;
            IsActivation = isActivation;
        }
    }

    public class ParticipantBasicView
    {
        public string Id { get; set; }
        public int Sequence { get; set; }
        public BusinessItemView Type { get; set; }
        public BusinessItemView Position { get; set; }
        public DepartmentBasicView Department { get; set; }
        public EmployeeBasicView Actor { get; set; }
        public ParticipantActivationBasicView Activation { get; set; }
        public Boolean IsValidDataRow { get; set; }
        public ParticipantBasicView()
        {

        }

        public ParticipantBasicView(string id, int sequence, BusinessItemView type, BusinessItemView position, DepartmentBasicView department, EmployeeBasicView actor,
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

    public class EncounterAdminLocationView
    {
        public LocationBasicView AdminLocation { get; set; }
        public string PhysicalTypeCode { get; set; }
        public string LocationStatusCode { get; set; }
        public string ServiceRoleTypeCode { get; set; }
        public string LocationClassCode { get; set; }
        public string LocationTypeCode { get; set; }
        public string PatientServiceCode { get; set; }
        public DateTime? CheckinDatetime { get; set; }
        public DateTime? CheckoutDatetime { get; set; }
        public string AdminDualLocationId { get; set; }
        public EncounterAdminLocationView()
        {

        }

        public EncounterAdminLocationView(LocationBasicView adminLocation, string physicalTypeCode, string locationStatusCode, string serviceRoleTypeCode, string locationClassCode,
            string locationTypeCode, string patientServiceCode, DateTime? checkinDatetime, DateTime? checkoutDatetime, string adminDualLocationId)
        {
            AdminLocation = adminLocation;
            PhysicalTypeCode = physicalTypeCode;
            LocationStatusCode = locationStatusCode;
            ServiceRoleTypeCode = serviceRoleTypeCode;
            LocationClassCode = locationClassCode;
            LocationTypeCode = locationTypeCode;
            PatientServiceCode = patientServiceCode;
            CheckinDatetime = checkinDatetime;
            CheckoutDatetime = checkoutDatetime;
            AdminDualLocationId = adminDualLocationId;
        }
    }

    public class LocationBasicView
    {
        public string BedId { get; set; }
        public string FacilityBedId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public RoomBasicView Room { get; set; }
        public WardBasicView Ward { get; set; }
        public BuildingBasicView Building { get; set; }
        public LocationBasicView()
        {

        }

        public LocationBasicView(string bedId, string facilityBedId, string displayCode, string name, RoomBasicView room, WardBasicView ward, BuildingBasicView building)
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

    public class WardBasicView
    {
        public string WardId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public WardBasicView()
        {

        }

        public WardBasicView(string wardId, string displayCode, string name)
        {
            WardId = wardId;
            DisplayCode = displayCode;
            Name = name;
        }
    }

    public class RoomBasicView
    {
        public string RoomId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public RoomBasicView()
        {

        }

        public RoomBasicView(string roomId, string displayCode, string name)
        {
            RoomId = roomId;
            DisplayCode = displayCode;
            Name = name;
        }
    }

    public class BuildingBasicView
    {
        public string BuildingId { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public BuildingBasicView()
        {

        }

        public BuildingBasicView(string buildingId, string displayCode, string name)
        {
            BuildingId = buildingId;
            DisplayCode = displayCode;
            Name = name;
        }
    }

    public class EncounterOccupyingLocationView
    {
        public LocationBasicView OccupyingLocation { get; set; }
        public string LocationStatusCode { get; set; }
        public DateTime? CheckinDatetime { get; set; }
        public DateTime? CheckoutDatetime { get; set; }
        public EncounterOccupyingLocationView()
        {

        }

        public EncounterOccupyingLocationView(LocationBasicView occupyingLocation, string locationStatusCode, DateTime? checkinDatetime, DateTime? checkoutDatetime)
        {
            OccupyingLocation = occupyingLocation;
            LocationStatusCode = locationStatusCode;
            CheckinDatetime = checkinDatetime;
            CheckoutDatetime = checkoutDatetime;
        }
    }

    public class OutpatientEncounterDischargeView
    {
        public string DischargeOrdererId { get; set; }
        public DateTime? DischargeScheduledDatetime { get; set; }
        public DateTime? DischargeOrderDatetime { get; set; }
        public BusinessItemView DischargeStatus { get; set; }
        public BusinessItemView DischargeDisposition { get; set; }
        public OutpatientEncounterDischargeView()
        {

        }

        public OutpatientEncounterDischargeView(string dischargeOrdererId, DateTime? dischargeScheduledDatetime, DateTime? dischargeOrderDatetime,
            BusinessItemView dischargeStatus, BusinessItemView dischargeDisposition)
        {
            DischargeOrdererId = dischargeOrdererId;
            DischargeScheduledDatetime = dischargeScheduledDatetime;
            DischargeOrderDatetime = dischargeOrderDatetime;
            DischargeStatus = dischargeStatus;
            DischargeDisposition = dischargeDisposition;
        }
    }
    public class BusinessItemView
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public BusinessItemView() { }

        public BusinessItemView(string id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }
    }
}
