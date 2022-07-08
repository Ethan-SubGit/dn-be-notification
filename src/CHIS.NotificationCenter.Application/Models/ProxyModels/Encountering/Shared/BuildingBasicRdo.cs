using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
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
