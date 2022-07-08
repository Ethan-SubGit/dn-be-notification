using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    /// <summary>
    /// SMS 예약발송을 위한 배치 진입 Command
    /// </summary>
    public class RequestSmsBatchExecutionCommand : IRequest<bool>
    {
        public string BatchId { get; set; }

        public RequestSmsBatchExecutionCommand(string batchId)
        {
            BatchId = batchId;
        }
    }
}
