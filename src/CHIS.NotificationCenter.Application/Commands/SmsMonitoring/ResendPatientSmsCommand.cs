using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.SmsMonitoring
{
    public class ResendPatientSmsCommand : IRequest<bool>
    {
        public List<string> MessageDispatchItemIds { get; set; }
    }
}
