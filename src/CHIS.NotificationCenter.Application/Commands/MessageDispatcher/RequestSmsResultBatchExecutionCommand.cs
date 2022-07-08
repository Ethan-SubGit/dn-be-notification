using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    public class RequestSmsResultBatchExecutionCommand : IRequest<bool>
    {
        public string BatchId { get; set; }

        public RequestSmsResultBatchExecutionCommand(string batchId)
        {
            BatchId = batchId;
        }
    }
}
