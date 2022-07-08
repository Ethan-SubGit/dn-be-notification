using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
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
}
