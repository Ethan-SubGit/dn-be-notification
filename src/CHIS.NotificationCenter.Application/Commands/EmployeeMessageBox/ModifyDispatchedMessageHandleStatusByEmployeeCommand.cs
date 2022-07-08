using System.Collections.Generic;
using System.Text;
using System;
using MediatR;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    public class ModifyDispatchedMessageHandleStatusByEmployeeCommand : IRequest<bool>
    {
        public string MessageDispatchItemId { get; set; }

        public string EmployeeId { get; set; }

    }
}
