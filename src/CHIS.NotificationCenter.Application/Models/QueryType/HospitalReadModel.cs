
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class HospitalReadModel
    {
        public string Id { get; set; }
        public string RegionId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string AddressContent { get; set; }

      
    }

    public class AddressContentDto
    {
        public string ZipCode { get; set; }
        public string BasicAddress { get; set; }
        public string DetailAddress { get; set; }
        public string EmailAddress { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public string AddressDate { get; set; }
        public DateTime? AddressDateUtc { get; set; }
    }

}
