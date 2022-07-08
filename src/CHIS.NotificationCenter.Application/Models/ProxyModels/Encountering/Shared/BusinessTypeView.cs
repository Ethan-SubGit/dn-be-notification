using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class BusinessTypeView
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public BusinessTypeView() { }

        public BusinessTypeView(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
