using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.CommunicationNote;
using CHIS.NotificationCenter.Application.Queries;


using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("notificationcenter/v0/communication-note")]
    public class CommunicationNoteController : BSLBase
    {
        //private readonly ICallContext _context;
        private readonly IMediator _mediator;
        private readonly ICommunicationNoteQueries _communicationNoteQueries;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mediator"></param>
        /// <param name="communicationNoteQueries"></param>
        public CommunicationNoteController(ICallContext context, IMediator mediator, ICommunicationNoteQueries communicationNoteQueries) : base(context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _communicationNoteQueries = communicationNoteQueries ?? throw new ArgumentNullException(nameof(communicationNoteQueries));
        }

        /// <summary>
        /// 받은 쪽지 인스턴스 조회
        /// </summary>
        /// <remarks>
        /// messageInstanceId로 쪽지 Detail 조회
        /// </remarks>
        /// <param name="id">employeemessageinstance.id</param>
        /// <returns></returns>
        [Route("receive-notes/{id}")]
        [HttpGet]
        public async Task<IActionResult> FindReceiveNote(
            string id)
        {
            var result = await _communicationNoteQueries.FindReceiveNote(id).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 받은 쪽지 리스트 조회
        /// </summary>
        /// <remarks>
        /// {"employeeId":"GUID_10001","patientId":"","searchText":"","handleStatusFilter":2,"skip":0,"take":20}
        /// </remarks>
        /// <param name="searchNoteCommand">searchNoteCommand</param>
        /// <returns></returns>
        [Route("receive-notes/search")]
        [HttpPost]
        public async Task<IActionResult> SearchReceiveNotes([FromBody]SearchCommunicationNoteCommand searchNoteCommand)
        {
            if (searchNoteCommand == null)
            {
                return BadRequest();
            }


            string employeeId = searchNoteCommand.EmployeeId;
            string patientId = searchNoteCommand.PatientId;
            string searchText = searchNoteCommand.SearchText;
            int handleStatusFilter = searchNoteCommand.HandleStatusFilter;
            int skip = searchNoteCommand.Skip;
            int take = searchNoteCommand.Take;

            var result = await _communicationNoteQueries.SearchReceiveNote(
                employeeId: employeeId,
                patientId: patientId,
                searchText: searchText,
                handleStatusFilter: handleStatusFilter,
                skip: skip,
                take: take).ConfigureAwait(false);
  
            return Ok(result);
        }

        /// <summary>
        /// 받은쪽지 카운트 조회
        /// </summary>
        /// <param name="employeeId">employeeId</param>
        /// <param name="searchText">검색문자열</param>
        /// <returns></returns>
        [Route("receive-notes/count-unread")]
        [HttpGet]
        public async Task<IActionResult> SearchUnreadCount(string employeeId,string searchText)
        {
            var result = await _communicationNoteQueries.SearchUnreadCount(employeeId, searchText).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 쪽지 읽음상태 업데이트
        /// </summary>
        /// <param name="modifyCommunicationNoteReadStatusCommand"></param>
        /// <returns></returns>
        [Route("receive-notes/read-status")]
        [HttpPut]
        public async Task<IActionResult> ModifyReceiveNoteReadStatus([FromBody]ModifyCommunicationNoteReadStatusCommand modifyCommunicationNoteReadStatusCommand)
        {
            bool commandResult = false;

            if (modifyCommunicationNoteReadStatusCommand != null)
            {
                commandResult = await _mediator.Send(modifyCommunicationNoteReadStatusCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
        }

       /// <summary>
       /// 보낸쪽지 리스트 조회
       /// </summary>
       /// <param name="employeeId"></param>
       /// <param name="patientId"></param>
       /// <param name="searchText"></param>
       /// <param name="skip"></param>
       /// <param name="take"></param>
       /// <returns></returns>
        [Route("sent-notes/search")]
        [HttpGet]
        public async Task<IActionResult> SearchSentNotes(string employeeId, string patientId, string searchText, int skip, int take)
        {

            var result = await _communicationNoteQueries.SearchSentNote(employeeId, patientId, searchText, skip, take).ConfigureAwait(false);


            return Ok(result);
        }

        /// <summary>
        /// 보낸 쪽지 인스턴스 조회
        /// </summary>
        /// <param name="messageInstanceId"></param>
        /// <returns></returns>
        [Route("sent-notes/{messageInstanceId}")]
        [HttpGet]
        public async Task<IActionResult> FindSentNote(string messageInstanceId)
        {
            var result = await _communicationNoteQueries.FindSentNote(messageInstanceId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 보낸 쪽지 수신자 목록 조회
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        [Route("sent-notes/recipients")]
        [HttpGet]
        public async Task<IActionResult> SearchNoteRecipient(string messageDispatchItemId)
        {
            var result = await _communicationNoteQueries.SearchNoteRecipient(messageDispatchItemId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 쪽지 전송위한 employee information list 조회
        /// </summary>
        /// <param name="searchEmployeesCommand"></param>
        /// <returns></returns>
        [Route("search-employees")]
        [HttpPut]
        public async Task<IActionResult> ModifyReceiveNoteReadStatus([FromBody]SearchEmployeesCommand searchEmployeesCommand)
        {
            if (searchEmployeesCommand == null)
            {
                return BadRequest();
            }
            List<string> employeeIds = searchEmployeesCommand.EmployeeIds;
            var result = await _communicationNoteQueries.SearchEmployees(employeeIds).ConfigureAwait(false);

            return Ok(result);
        }
    }
}
