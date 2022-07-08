using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.MessageSpecification
{
    public  class MessageTemplateViewByServiceType
    {
        public string Id { get; set; }
        public string ServiceCode { get; set; }
        public string Classification { get; set; }
        public string MessageCategory { get; set; }
        public string Description { get; set; }
        public NotificationServiceType ServiceType { get; set; }

        public string PredefinedContent { get; set; }
        public IList<MessageTemplateView> MessageTemplates { get; set; }

    }
}
