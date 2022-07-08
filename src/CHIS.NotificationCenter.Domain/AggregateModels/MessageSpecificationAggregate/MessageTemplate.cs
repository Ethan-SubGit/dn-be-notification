using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate
{
    public class MessageTemplate : EntityBase
    {
        #region property

        public string MessageSpecificationId { get; set; }
        public ContentTemplateScopeType ContentTemplateScope { get; set; }
        public string TemplateTitle { get; set; }
        public string ContentTemplate { get; set; }
        public bool IsDeleted { get; set; }
        public string EmployeeId { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public DateTime? DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime? DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion

        public MessageTemplate()
        {
            //
        }

        public MessageTemplate(string messageSpecificationId, ContentTemplateScopeType contentTemplateScope
            , string templateTitle, string contentTemplate, string employeeId, string tenantId
            , string hospitalId, DateTime dataFirstRegisteredDateTimeUtc, DateTime dataLastModifiedDateTimeUtc
            , Trace trace, bool isDeleted)
        {
            MessageSpecificationId = messageSpecificationId;
            ContentTemplateScope = contentTemplateScope;
            TemplateTitle = templateTitle;
            ContentTemplate = contentTemplate;
            EmployeeId = employeeId;
            TenantId = tenantId;
            HospitalId = hospitalId;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
            Trace = trace;
            IsDeleted = isDeleted;
        }
    }
}
