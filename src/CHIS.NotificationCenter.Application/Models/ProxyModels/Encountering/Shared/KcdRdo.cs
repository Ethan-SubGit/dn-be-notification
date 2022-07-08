using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class KcdRdo
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public KcdRdo() { }
        public KcdRdo(string id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }
    }
}
