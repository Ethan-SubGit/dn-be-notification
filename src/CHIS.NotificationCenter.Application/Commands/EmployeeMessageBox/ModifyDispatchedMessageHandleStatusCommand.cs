using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    /// <summary>
    /// MessageDispatchItemId로 inbound message의 handle status를 일괄 변경함
    /// Cosign등 Medical Record에서 사용
    /// </summary>
    public class ModifyDispatchedMessageHandleStatusCommand : IRequest<bool>
    {
        public string MessageDispatchItemId { get; set; }
        public bool IsHandled { get; set; }

    }
}

