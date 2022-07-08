using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class RemoveMessageTemplateCommand : IRequest<bool>
    {
        public string Id { get; set; }
        //public string MessateSpecificationId { get; set; }

        public RemoveMessageTemplateCommand()
        {
            //
        }
    }
}
