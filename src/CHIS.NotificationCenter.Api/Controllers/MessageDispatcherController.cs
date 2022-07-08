using CHIS.Framework.Core.BackgroundJob;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.MessageDispatcher;

//using CHIS.NotificationCenter.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("notificationcenter/v0/message-dispatcher")]
    public class MessageDispatcherController : BSLBase
    {

        private readonly ICallContext _context;
        private readonly IMediator _mediator;

        readonly IBackgroundJobCreator _backgroundJobCreator;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mediator"></param>
        /// <param name="backgroundJobCreator"></param>
        public MessageDispatcherController(
            ICallContext context
            , IMediator mediator
            , IBackgroundJobCreator backgroundJobCreator
            ) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
         
            _backgroundJobCreator = backgroundJobCreator ?? throw new ArgumentNullException(nameof(backgroundJobCreator));

        }

        /// <summary>
        /// Inbox Message 전송 서비스 API
        /// </summary>
        /// <param name="requestInboxMessageNotificationCommand"></param>
        /// <returns></returns>
        [Route("inbox")]
        [HttpPost]
        public async Task<IActionResult> RequestInboxMessageNotification([FromBody]RequestInboxMessageNotificationCommand requestInboxMessageNotificationCommand)
        {
            string commandResult = string.Empty;
            if (requestInboxMessageNotificationCommand != null)
            {
                commandResult = await _mediator.Send(requestInboxMessageNotificationCommand).ConfigureAwait(false);
            }

            return CreatedAtRoute(new { Result = commandResult }, commandResult);
        }

        /// <summary>
        /// 보낸 메시지 cancel 처리
        /// </summary>
        /// <param name="requestCancellationOfInboxMessageNotificationCommand"></param>
        /// <returns></returns>
        [Route("inbox/cancellation")]
        [HttpPut]
        public async Task<IActionResult> CancelInboxMessage([FromBody]RequestCancellationOfInboxMessageNotificationCommand requestCancellationOfInboxMessageNotificationCommand)
        {
            //bool commandResult = false;
            int commandResult = 0;

            if (requestCancellationOfInboxMessageNotificationCommand != null)
            {
                commandResult = await _mediator.Send(requestCancellationOfInboxMessageNotificationCommand).ConfigureAwait(false);
            }

            //return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
            return (IActionResult)Ok(commandResult);
        }

        /// <summary>
        /// sms전송요청
        /// </summary>
        /// <param name="requestSmsMessageNotificationCommand"></param>
        /// <returns></returns>
        [Route("sms/patient")]
        [HttpPost]
        public async Task<IActionResult> RequestPatientSmsMessageNotification([FromBody]RequestPatientSmsMessageNotificationCommand requestSmsMessageNotificationCommand)
        {
            bool commandResult = false;

            if (requestSmsMessageNotificationCommand != null)
            {
                commandResult = await _mediator.Send(requestSmsMessageNotificationCommand).ConfigureAwait(false);
            }

            return CreatedAtRoute(new { Result = commandResult }, commandResult);
            //return commandResult ? (IActionResult)Ok(new { result = commandResult }) : BadRequest();
        }
        
        [Route("sms/patient-mq-test")]
        [HttpPost]
        public async Task<IActionResult> RequestSmsMessageNotificationMqTest([FromBody]SmsMqTest requestSmsMessageNotificationCommand)
        {
            bool commandResult = false;

            if (requestSmsMessageNotificationCommand != null)
            {
                commandResult = await _mediator.Send(requestSmsMessageNotificationCommand).ConfigureAwait(false);
            }

            return CreatedAtRoute(new { Result = commandResult }, commandResult);
            //return commandResult ? (IActionResult)Ok(new { result = commandResult }) : BadRequest();
        }

        /// <summary>
        /// SMS 예약/일괄 발송위한 배치 스케줄러 진입 API 
        /// </summary>
        /// <returns></returns>
        [Route("sms-batch")] //sms-batch
        [HttpPost]
        public async Task<IActionResult> RequestProcessReservedSmsMessage()
        {
            this._backgroundJobCreator.WriteLog(_context.ContextId, StateType.Starting, "job start");

            try
            {
                string batchId = Guid.NewGuid().ToString();
                await _mediator.Send(new RequestSmsBatchExecutionCommand(batchId)).ConfigureAwait(false);
                this._backgroundJobCreator.WriteLog(_context.ContextId, StateType.Completed, "job start finished");

            }
            catch (Exception e)
            {
                this._backgroundJobCreator.WriteLog(_context.ContextId, StateType.ErrorAndStop, e.Message, e.ToString());
            }

            return Ok();
        }

        /// <summary>
        /// SMS 전송결과 업데이트 위한 배치 스케줄러 진입 API (Naver에 성공여부 조회)
        /// </summary>
        /// <returns></returns>
        [Route("sms-batch-result")] 
        [HttpPost]
        public async Task<IActionResult> RequestProcessSmsMessageResult()
        {

            this._backgroundJobCreator.WriteLog(_context.ContextId, StateType.Starting, "job start");

            try
            {
                await _mediator.Send(new RequestSmsResultBatchExecutionCommand(Guid.NewGuid().ToString())).ConfigureAwait(false);
                this._backgroundJobCreator.WriteLog(_context.ContextId, StateType.Completed, "job start finished");

            }
            catch (Exception e)
            {
                this._backgroundJobCreator.WriteLog(_context.ContextId, StateType.ErrorAndStop, e.Message, e.ToString());
            }

            return Ok();

        }
        /// <summary>
        /// SMS 전송결과 업데이트 위한 배치 스케줄러 진입 API (Naver에 성공여부 조회)
        /// </summary>
        /// <returns></returns>
        [Route("sms-batch-result-test-temporary")]
        [HttpPost]
        public async Task<IActionResult> RequestProcessSmsMessageResultTest()
        {

            await _mediator.Send(new RequestSmsResultBatchExecutionCommand(Guid.NewGuid().ToString())).ConfigureAwait(false);

            return Ok();

        }
        /// <summary>
        /// 직원간 쪽지 전송
        /// </summary>
        /// <param name="registerCommunicationNoteNotificationCommand"></param>
        /// <returns></returns>
        [Route("communication-note")]
        [HttpPost]
        public async Task<IActionResult> RegisterCommunicationNoteNotification([FromBody]RegisterCommunicationNoteNotificationCommand registerCommunicationNoteNotificationCommand)
        {
            string commandResult = string.Empty;
            if (registerCommunicationNoteNotificationCommand != null)
            {
                commandResult = await _mediator.Send(registerCommunicationNoteNotificationCommand).ConfigureAwait(false);
            }

            return CreatedAtRoute(new { Result = commandResult }, commandResult);
        }

    }
}
