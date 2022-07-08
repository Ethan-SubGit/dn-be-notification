using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.MessageSpecification
{
    public class MessageTemplateView
    {
        public string Id { get; set; }
        public string MessageSpecificationId { get; set; }
        public ContentTemplateScopeType ContentTemplateScope { get; set; }
        public string TemplateTitle { get; set; }
        public string ContentTemplate { get; set; }
        public bool IsDeleted { get; set; }

    }
}
