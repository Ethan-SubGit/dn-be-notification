using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models;
using MediatR;
using CHIS.NotificationCenter.Application.Models.CommonModels;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    /// <summary>
    /// Inbox 화면에서 확인 처리할경우 호출함.
    /// IsProcessHandleStatusToAllRecipients 파라미터 옵션에 따라 본인확인 또는 전체수신자 확인으로 처리함.
    /// </summary>
    public class ModifyMessageInstanceHandleStatusCommand : IRequest<bool>
    {
        public List<MessageInstanceHandleStatusDto> messageInstances { get; set; }
    }
}

