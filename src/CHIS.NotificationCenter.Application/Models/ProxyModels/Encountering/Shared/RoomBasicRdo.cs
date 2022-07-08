using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
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
}
