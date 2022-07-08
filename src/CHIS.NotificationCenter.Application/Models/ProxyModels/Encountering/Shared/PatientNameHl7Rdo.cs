using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class PatientNameHl7Rdo
    {
        public string PatientFullName { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string Prefix { get; set; }
        public string Degree { get; set; }
        public string NameTypeCode { get; set; }
        public string NameRepresentationCode { get; set; }

        public PatientNameHl7Rdo() { }
        public PatientNameHl7Rdo(string patientFullName, string familyName, string givenName, string middleName, string suffix, string prefix, string degree,
            string nameTypeCode, string nameRepresentationCode)
        {
            PatientFullName = patientFullName;
            FamilyName = familyName;
            GivenName = givenName;
            MiddleName = middleName;
            Suffix = suffix;
            Prefix = prefix;
            Degree = degree;
            NameTypeCode = nameTypeCode;
            NameRepresentationCode = nameRepresentationCode;
        }
    }
}
