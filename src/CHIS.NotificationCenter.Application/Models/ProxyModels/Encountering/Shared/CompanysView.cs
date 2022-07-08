using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class CompanysView
    {
        public List<CompanyView> Companys { get; set; }

        public CompanysView()
        {

        }

        public CompanysView(List<CompanyView> companys)
        {
            Companys = companys;
        }
    }
}
