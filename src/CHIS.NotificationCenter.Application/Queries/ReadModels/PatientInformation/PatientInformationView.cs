using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation
{

    public class PatientInformationView
    {
        public string PatientId         { get; set; }
        public string EncounterId       { get; set; }
        public string PatientDisplayId  { get; set; }

        public string PatientName       { get; set; }
        public string GenderType        { get; set; }

        public string    Age               { get; set; }

        public DateTime? BirthDay { get; set; }

        //[JsonProperty(NullValueHandling=NullValueHandling.Ignore)] //Json parsing 할때 속성 자체가 안나와서 못씀...
        public string DepartmentId      { get; set; } = "";
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DepartmentName    { get; set; } = "";

        /// <summary>
        /// 병동이름
        /// </summary>
        public string WardCodeName { get; set; } = "";
        /// <summary>
        /// 병실호수
        /// </summary>
        public string RoomCodeName { get; set; } = "";
    }
}

