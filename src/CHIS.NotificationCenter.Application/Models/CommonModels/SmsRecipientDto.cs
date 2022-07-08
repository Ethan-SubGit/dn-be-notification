using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.Enum;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class SmsRecipientDto
    {
        public SmsRecipientType SmsRecipientType { get; set; }

        public string ActorId { get; set; }
        public string Name { get; set; }

        public string Mobile { get; set; }

        public string PatientContactRelationShipCode { get; set; }
        public string PatientContactClassificationCode { get; set; }
    }
}
