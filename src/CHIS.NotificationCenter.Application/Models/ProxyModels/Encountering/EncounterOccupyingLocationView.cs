using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering
{
    public class EncounterOccupyingLocationView
    {
        public LocationBasicView OccupyingLocation { get; set; }
        public string LocationStatusCode { get; set; }
        public DateTime? CheckinDatetime { get; set; }
        public DateTime? CheckoutDatetime { get; set; }
        public string EmergencyMedicalAreaCode { get; set; }

        public EncounterOccupyingLocationView() { }

        public EncounterOccupyingLocationView(LocationBasicView occupyingLocation, string locationStatusCode, DateTime? checkinDatetime,
            DateTime? checkoutDatetime, string emergencyMedicalAreaCode)
        {
            OccupyingLocation = occupyingLocation;
            LocationStatusCode = locationStatusCode;
            CheckinDatetime = checkinDatetime;
            CheckoutDatetime = checkoutDatetime;
            EmergencyMedicalAreaCode = emergencyMedicalAreaCode;
        }
    }
}
