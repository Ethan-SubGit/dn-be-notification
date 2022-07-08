using CHIS.NotificationCenter.Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    public class SmsMqTest : IRequest<bool>
    {
        public string test { get; set; }
        public string PatientId { get; set; }
    }
}
