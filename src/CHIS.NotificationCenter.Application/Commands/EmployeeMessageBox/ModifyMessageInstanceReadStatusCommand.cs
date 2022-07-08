using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models;
using MediatR;
using CHIS.NotificationCenter.Application.Models.CommonModels;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{

    public class ModifyMessageInstanceReadStatusCommand : IRequest<bool>
    {
        public List<MessageInstanceHandleStatusDto> messageInstances { get; set; }
    }
}

