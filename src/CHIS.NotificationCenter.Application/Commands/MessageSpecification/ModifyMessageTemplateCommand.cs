using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class ModifyMessageTemplateCommand : IRequest<bool>
    {
        #region property
        public string Id { get; set; }
        public string MessageSpecificationId { get; set; }
        public ContentTemplateScopeType ContentTemplateScope { get; set; }
        public string TemplateTitle { get; set; }
        public string ContentTemplate { get; set; }
        //public string EmployeeId { get; set; }

        //public DateTime DataLastModifiedDateTimeUtc { get; set; }
        //public Trace Trace { get; set; }
        #endregion

        public ModifyMessageTemplateCommand()
        {
            //
        }
    }
}
