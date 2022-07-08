using CHIS.Framework.Core.BackgroundJob;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    public class RequestSmsResultBatchExecutionCommandHandler : IRequestHandler<RequestSmsResultBatchExecutionCommand, bool>
    {
        //private readonly IBackgroundJobCreator _backgroundJobCreator;

        //private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        //private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        ////private readonly ITimeManager                   _timeManager;
        //private readonly ISmsMonitoringQueries _smsLogViewerQueries;
        //private readonly ICallContext _callContext;
        private readonly ISmsService _smsService;


        public RequestSmsResultBatchExecutionCommandHandler(
              IBackgroundJobCreator backgroundJobCreator
            , IMessageDispatchItemRepository messageDispatchItemRepository
            , ISmsMonitoringRepository smsMonitoringRepository
            , ICallContext callContext
            , ISmsService smsService
            , ISmsMonitoringQueries smsLogViewerQueries
            )
        {
            //_backgroundJobCreator = backgroundJobCreator ?? throw new ArgumentNullException(nameof(backgroundJobCreator));

            //_messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            //_smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            ////_timeManager                    = timeManager                   ?? throw new ArgumentNullException(nameof(timeManager                   ));
            //_callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            
            //_smsLogViewerQueries = smsLogViewerQueries ?? throw new ArgumentNullException(nameof(smsLogViewerQueries));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
        }


        public async Task<bool> Handle(RequestSmsResultBatchExecutionCommand request, CancellationToken cancellationToken)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            await _smsService.BatchUpdateSmsReceiveLog(-5).ConfigureAwait(false);

            return true;

        }

      
    }
}
