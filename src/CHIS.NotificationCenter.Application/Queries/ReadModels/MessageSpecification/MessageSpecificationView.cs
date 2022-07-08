using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.Enum;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.MessageSpecification
{
    public class MessageSpecificationView
    {
        public string Id { get; set; }
        public string ServiceCode { get; set; }
        public string Classification { get; set; }
        public string MessageCategory { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSystemProperty { get; set; }
        public bool IsAddRecipient { get; set; }
        public string MessageCallbackNoConfigId { get; set; }
        public NotificationServiceType ServiceType{ get; set; }
    }

}
