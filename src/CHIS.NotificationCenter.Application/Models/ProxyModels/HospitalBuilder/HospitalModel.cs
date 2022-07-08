using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.HospitalBuilder
{
    public class HospitalModel
    {
        public string Id { get; set; }
        public string regionId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public AddressContent addressContent { get; set; }

    }
    public class AddressContent
    {
        public string zipCode { get; set; }
        public string basicAddress { get; set; }
        public string detailAddress { get; set; }
        public string emailAddress { get; set; }
        public string businessPhoneNumber { get; set; }
        public string fullAddress { get; set; }
    }
}
