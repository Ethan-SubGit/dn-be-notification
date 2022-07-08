using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.Constants
{

    public static class ReadModelConstants 
    {
        /// <summary>
        /// ReadModel DB Prefix
        /// </summary>
        public static class Domains
        {
            public const string PatientDemographic = "patientdemographic";
            public const string Employee = "employee";
            public const string Encountering = "encountering";
            public const string DepartmentBuilder = "departmentbuilder";
            public const string HospitalBuilder = "hospitalbuilder";
            public const string PersonnelInfo = "personnelinfo";

            public const string MedicalRecordMerging = "medicalrecordmerging";
            public const string LocationBuilder = "locationbuilder";
        }

        public static class DomainTables
        {
            public const string Department = "Department";

            public const string Hospital = "Hospital";

            /// <summary>
            /// 직원연락처
            /// </summary>
            public const string ContactPoint = "ContactPoint";

            /// <summary>
            /// 진료과 참여자 정보
            /// </summary>
            public const string Participant = "Participant";
            public const string Contact = "Contact";
            public const string ContactTelephone = "ContactTelephone";

            public const string JobPosition = "jobposition";
            public const string JobPositionMapping = "jobpositionmapping";
            public const string Occupation = "occupation";

            public const string DomainResponse = "domainresponse";

            public const string Room = "room";
        }
    }

}