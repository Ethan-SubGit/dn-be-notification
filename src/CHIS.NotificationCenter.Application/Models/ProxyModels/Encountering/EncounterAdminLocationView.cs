using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering
{
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

        public EncounterAdminLocationView() { }

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
}
