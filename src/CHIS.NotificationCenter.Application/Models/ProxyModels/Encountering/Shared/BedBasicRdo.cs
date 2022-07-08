using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
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
}
