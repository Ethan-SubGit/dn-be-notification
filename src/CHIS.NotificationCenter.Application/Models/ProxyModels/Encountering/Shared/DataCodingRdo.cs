using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class DataCodingRdo
    {
        public string Id { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }

        public DataCodingRdo() { }

        public DataCodingRdo(string id, string displayCode, string name)
        {
            Id = id;
            DisplayCode = displayCode;
            Name = name;
        }
    }
}
