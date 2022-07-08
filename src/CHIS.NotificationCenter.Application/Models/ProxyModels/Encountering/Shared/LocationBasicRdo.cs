using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
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
}
