using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class EncounterType
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public EncounterType()
        {

        }

        public EncounterType(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
