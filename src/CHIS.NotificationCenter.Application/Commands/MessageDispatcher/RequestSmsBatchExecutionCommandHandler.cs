using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    /// <summary>
    /// SMS 예약발송위한 배치 진입 Command Handler
    /// </summary>
    public class RequestSmsBatchExecutionCommandHandler : IRequestHandler<RequestSmsBatchExecutionCommand, bool>
    {
        //private readonly IBackgroundJobCreator          _backgroundJobCreator;
        
        //private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        //private readonly ITimeManager                   _timeManager;
        //private readonly ICallContext                   _callContext;
        private readonly ISmsService      _smsService;

        public RequestSmsBatchExecutionCommandHandler(
            //  IBackgroundJobCreator backgroundJobCreator
            //, IMessageDispatchItemRepository messageDispatchItemRepository
            ISmsMonitoringRepository smsMonitoringRepository
            //, ICallContext callContext
            //, ITimeManager timeManager
            , ISmsService smsService
            )
        {
            //_backgroundJobCreator           = backgroundJobCreator          ?? throw new ArgumentNullException(nameof(backgroundJobCreator          ));

            //_messageDispatchItemRepository  = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository ));
            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            //_timeManager                    = timeManager                   ?? throw new ArgumentNullException(nameof(timeManager                   ));
            //_callContext                    = callContext                   ?? throw new ArgumentNullException(nameof(callContext                   ));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
        }

        /// <summary>
        /// SMS 배치 실행 By Scheduler
        /// 조건에 부합되는 SMS 리스트 취합하여 상태를 Iprogress로 변경
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Handle(RequestSmsBatchExecutionCommand request, CancellationToken cancellationToken)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            // DESC : List up batch Jobs 
            //List<SmsSendLog> executionLogs = 
            //    _smsMonitoringRepository.PrepareSmsBatchExecution(request.BatchId).Result;
            List<SmsSendLog> executionLogs =
              await _smsMonitoringRepository.PrepareSmsBatchExecution(batchId: request.BatchId).ConfigureAwait(false);

            // DESC : Send SMS 
            foreach (var executionLog in executionLogs)
            {
                await _smsService.SendSmsMessage(messageDispatchItemId: executionLog.MessageDispatchItemId).ConfigureAwait(false);
            }
            return true;
        }

    }
}
