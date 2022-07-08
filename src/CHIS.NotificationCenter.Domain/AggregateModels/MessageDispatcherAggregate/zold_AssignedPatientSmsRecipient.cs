//using System;
//using System.Collections.Generic;
//using System.Text;
//using CHIS.NotificationCenter.Domain.SeedWork;
//using CHIS.NotificationCenter.Domain.Enum;
////using CHIS.Share.NotificationCenter.Enum;

//namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate
//{

//    public class AssignedPatientSmsRecipient : EntityBase
//    {
//        /// <summary>
//        ///   SmsPatientResolveType
//        //    Patient = 1
//        //,   Guardian = 2
//        //,   PatientAndGuardian = 3
//        /// </summary>
//        public SmsRecipientType SmsRecipientType{ get; set; }

//        public string PatientId { get; set; }
//        public string Mobile { get; set; }
//        /// <summary>
//        /// ClassificationCode
//        /// 01 : Noraml
//        /// 02 : Emergency
//        /// </summary>

//        public string ContactRelationShipCode { get; set; }
//        public string ContactClassificationCode { get; set; }

//        public string MessageDispatchItemId { get; set; }
        
//        public string TenantId { get; set; }
//        public string HospitalId { get; set; }
//    }
//}