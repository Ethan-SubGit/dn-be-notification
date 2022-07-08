using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Api.Controllers
{
    [Produces("application/json")]
    [Route("notificationcenter/v0/employee-message-boxes")]
    public class EmployeeMessageBoxController : BSLBase
    {
        //private readonly ICallContext _context;
        private readonly IMediator _mediator;
        private readonly IEmployeeMessageBoxQueries _employeeMessageBoxQueries;
        //private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mediator"></param>
        /// <param name="employeeMessageBoxQueries"></param>
        public EmployeeMessageBoxController(ICallContext context, IMediator mediator
            , IEmployeeMessageBoxQueries employeeMessageBoxQueries
            //, IEmployeeMessageBoxRepository employeeMessageBoxRepository
            ) : base(context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _employeeMessageBoxQueries = employeeMessageBoxQueries ?? throw new ArgumentNullException(nameof(employeeMessageBoxQueries));
            //_employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
        }



        /// <summary>
        /// 특정 메시지 확인 처리함 (By Message Instance Id)
        /// </summary>
        /// <param name="modifyMessageInstanceHandleStatusCommand"></param>
        /// <returns></returns>
        [Route("message-instances/message-handle-status")]
        [HttpPut]
        public async Task<IActionResult> ModifyMessageHandleStatus([FromBody]ModifyMessageInstanceHandleStatusCommand modifyMessageInstanceHandleStatusCommand)
        {
            bool commandResult = false;

            if (modifyMessageInstanceHandleStatusCommand != null)
            {
                commandResult = await _mediator.Send(modifyMessageInstanceHandleStatusCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult) Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// EmployeeId 로 특정 메시지 확인 처리. (by messageDispatchItemId and employeeId)
        /// Readback 처리 위함.(CVR등)
        /// </summary>
        /// <param name="modifyDispatchedMessageHandleStatusByEmployeeCommand"></param>
        /// <returns></returns>
        [Route("message-instances/message-handle-status-by-employee")]
        [HttpPut]
        public async Task<IActionResult> ModifyMessageHandleStatusByEmployeeId([FromBody]ModifyDispatchedMessageHandleStatusByEmployeeCommand modifyDispatchedMessageHandleStatusByEmployeeCommand)
        {
            bool commandResult = false;

            if (modifyDispatchedMessageHandleStatusByEmployeeCommand != null)
            {
                commandResult = await _mediator.Send(modifyDispatchedMessageHandleStatusByEmployeeCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// 모든 수신자의 메시지 Handle Status 변경 (By messageDispatchItemId) 
        /// 타팀연동 사용중임 (ex,Cosign확인시 전체 수신자 확인처리)
        /// </summary>
        /// <param name="modifyDispatchedMessageHandleStatusCommand"></param>
        /// <returns></returns>
        [Route("message-dispatch-items")]
        [HttpPut]
        public async Task<IActionResult> ModifyDispatchedMessageHandleStatus([FromBody]ModifyDispatchedMessageHandleStatusCommand modifyDispatchedMessageHandleStatusCommand)
        {
            bool commandResult = false;

            if (modifyDispatchedMessageHandleStatusCommand != null)
            {
                commandResult = await _mediator.Send(modifyDispatchedMessageHandleStatusCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
        }
        /// <summary>
        /// 메시지 읽음상태 수정
        /// </summary>
        /// <param name="modifyMessageReadStatusCommand"></param>
        /// <returns></returns>
        [Route("message-instances/message-read-status")]
        [HttpPut]
        public async Task<IActionResult> ModifyMessageReadStatus([FromBody]ModifyMessageInstanceReadStatusCommand modifyMessageReadStatusCommand)
        {
            bool commandResult = false;

            if (modifyMessageReadStatusCommand != null)
            {
                commandResult = await _mediator.Send(modifyMessageReadStatusCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// 특정 메시지 인스턴스 조회
        /// </summary>
        /// <remarks>
        /// messageInstanceId 값으로 메시지 상세 내용을 조회
        /// </remarks>
        /// <param name="id">messageInstanceId</param>
        /// <returns></returns>
        [Route("message-instances/{id}")]
        [HttpGet]
        public async Task<IActionResult> FindMessage(
            string id)
        {
            var result = await _employeeMessageBoxQueries.FindMessage(id).ConfigureAwait(false);

            return Ok(result);
        }



        /// <summary>
        /// 인박스 검색 API (페이징 처리됨)
        /// </summary>
        /// <remarks>
        /// remark inbox
        /// </remarks>
        /// <param name="searchMessageCommand"></param>
        /// <returns></returns>
        [Route("message-instances/search")]
        [HttpPost]
        public async Task<IActionResult> SearchMessages([FromBody]SearchMessageCommand searchMessageCommand)
        {

            if (searchMessageCommand == null)
            {
                return (IActionResult)BadRequest();
            }

            var result = await _employeeMessageBoxQueries.SearchMessages(
                searchMessageCommand.EmployeeId,
                searchMessageCommand.MessageCategory,
                searchMessageCommand.PatientId ,
                searchMessageCommand.SearchText ,
                searchMessageCommand.PeriodFilter ,
                searchMessageCommand.HandleStatusFilter,
                searchMessageCommand.ExclusionMessageInstanceId ,
                searchMessageCommand.FromDateTime ,
                searchMessageCommand.ToDateTime ,
                searchMessageCommand.FilterByServiceCodes ,
                searchMessageCommand.DepartmentId,
                searchMessageCommand.Skip,
                searchMessageCommand.Take
                ).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 환자별 보기 리스트 검색
        /// </summary>
        /// <param name="searchPatientMessageCommand"></param>
        /// <returns></returns>
        [Route("message-instances/search-patient")]
        [HttpPost]
        public async Task<IActionResult> SearchPatientMessages([FromBody]SearchPatientMessageCommand searchPatientMessageCommand)
        {

            if (searchPatientMessageCommand == null)
            {
                return (IActionResult)BadRequest();
            }

            var result = await _employeeMessageBoxQueries.SearchPatientMessages(
                searchPatientMessageCommand.EmployeeId,
                searchPatientMessageCommand.PatientId,
                searchPatientMessageCommand.SearchText,
                searchPatientMessageCommand.PeriodFilter,
                searchPatientMessageCommand.HandleStatusFilter,
                searchPatientMessageCommand.FromDateTime,
                searchPatientMessageCommand.ToDateTime,

                searchPatientMessageCommand.FilterByServiceCodes,
                searchPatientMessageCommand.DepartmentId,
                searchPatientMessageCommand.Skip,
                searchPatientMessageCommand.Take).ConfigureAwait(false);

            return Ok(result);
        }

       

        /// <summary>
        ///  메시지 카운트 Summary
        /// </summary>
        /// <param name="employeeId">Required</param>
        /// <param name="searchText">Optional</param>
        /// <param name="patientId">Optional</param>
        /// <returns></returns>
        [Route("message-instances/summary")]
        [HttpGet]
        public async Task<IActionResult> RetrieveInboxSummary(string employeeId, string searchText, string patientId)
        {
            var result = await _employeeMessageBoxQueries.RetrieveMessageCountByEmployee(employeeId, searchText, patientId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 읽음(확인)처리를 위한 messageDispatchItemId 조회
        /// </summary>
        /// <param name="referenceId"></param>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        [Route("search-message-dispatch-item-id/with-referenceId")]
        [HttpGet]
        public async Task<IActionResult> RetrieveMessageDispatchItemId(string referenceId, string serviceCode)
        {
            var result = await _employeeMessageBoxQueries
                                .RetrieveMessageDispatchItemIdWithReference(referenceId: referenceId, serviceCode: serviceCode)
                                .ConfigureAwait(false);
            return Ok(result);
        }
        

        /// <summary>
        /// Outbox에서 특정 메시지 조회
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        [Route("message-instances/outbox/{messageDispatchItemId}")]
        [HttpGet]
        public async Task<IActionResult> FindOutboxMessage(string messageDispatchItemId)
        {
            var result = await _employeeMessageBoxQueries.FindOutboxMessage(messageDispatchItemId).ConfigureAwait(false);

            return Ok(result);
        }
        /// <summary>
        /// Outbox 메시지 목록 조회 (페이징)
        /// </summary>
        /// <param name="searchMessageCommand"></param>
        /// <returns></returns>
        [Route("message-instances/outbox/search")]
        [HttpPost]
        public async Task<IActionResult> SearchOutboxMessages([FromBody]SearchMessageCommand searchMessageCommand)
        {
            if (searchMessageCommand == null)
            {
                return (IActionResult)BadRequest();
            }

            var result = await _employeeMessageBoxQueries.SearchOutboxMessages(
                searchMessageCommand.EmployeeId,
                searchMessageCommand.MessageCategory,
                searchMessageCommand.PatientId,
                searchMessageCommand.SearchText,
                searchMessageCommand.PeriodFilter,
                searchMessageCommand.ExclusionMessageInstanceId,
                searchMessageCommand.FromDateTime,
                searchMessageCommand.ToDateTime,
                searchMessageCommand.FilterByServiceCodes,
                searchMessageCommand.DepartmentId,
                searchMessageCommand.Skip,
                searchMessageCommand.Take
                ).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 메시지 수신자 조회 - message-instances/outbox/recipients-status 로 통합 검토
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        [Route("message-instances/outbox/recipients")]
        [HttpGet]
        public async Task<IActionResult> RetrieveMessagesRecipients(string messageDispatchItemId)
        {
            var result = await _employeeMessageBoxQueries.RetrieveMessagesRecipients(messageDispatchItemId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 보낸 메시지의 정보및 수신상태 서비스 API (Exam domain 의 전송메시지및 수신자의 수신상태 확인 서비스)
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        [Route("message-instances/outbox/recipients-status")]
        [HttpGet]
        public async Task<IActionResult> RetrieveMessagesRecipientsStatus(string messageDispatchItemId)
        {
            var result = await _employeeMessageBoxQueries.RetrieveMessageRecipientStatuses(messageDispatchItemId).ConfigureAwait(false);

            return Ok(result);
        }
    }
}
