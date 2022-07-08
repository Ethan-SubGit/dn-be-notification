using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.SmsMonitoring;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.Enum;

//using CHIS.NotificationCenter.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Api.Controllers
{
    [Produces("application/json")]
    [Route("notificationcenter/v0/sms-monitoring")]
    public class SmsMonitoringController : BSLBase
    {
        //private readonly ICallContext _context;
        private readonly IMediator _mediator;
        //private readonly IMessageSentLogRepository _repository;
        private readonly ISmsMonitoringQueries _smsMonitoringQueries;

        public SmsMonitoringController(

            IMediator mediator
            , ISmsMonitoringQueries smsMonitoringQueries
            //, IMessageSentLogRepository repository
            , ICallContext context

            ) : base(context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            //_repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _smsMonitoringQueries = smsMonitoringQueries ?? throw new ArgumentNullException(nameof(smsMonitoringQueries));

        }

        /// <summary>
        /// SMS 조회 - 전송결과 (SendLog)
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="searchText"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [Route("sms-result")] //sms-result
        [HttpGet]
        public async Task<IActionResult> RequestSmsResultNotification
            (DateTime? fromDate, DateTime? toDate, string searchText
            , int skip, int take, string employeeId = "")
        {
            var result = await _smsMonitoringQueries.SearchSendLog(fromDate: fromDate
                , toDate: toDate, skip: skip, take: take, searchText: searchText
                , emplyeeId: employeeId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// SMS 상세조회 - 통신사 수신결과 (ReceiveLog))
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        [Route("sms-result-detail")] //sms-result-detail
        [HttpGet]
        public async Task<IActionResult> RequestSmsResultDetailNotification(string messageDispatchItemId)
        {
            var result = await _smsMonitoringQueries.SearchReceiveLog(messageDispatchItemId: messageDispatchItemId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 환자 SMS 결과조회
        /// </summary>
        /// <param name="searchSmsResultPatientCommand"></param>
        /// <returns></returns>
        [Route("sms-result-patient")]
        [HttpPost]
        public async Task<IActionResult> SearchPatientSmsResult([FromBody]SearchSmsResultPatientCommand searchSmsResultPatientCommand)
        {
            if (searchSmsResultPatientCommand == null)
            {
                return BadRequest();
            }

            DateTime? fromDateTime = searchSmsResultPatientCommand.FromDateTime;
            DateTime? toDateTime = searchSmsResultPatientCommand.ToDateTime;
            int skip = searchSmsResultPatientCommand.Skip;
            int take = searchSmsResultPatientCommand.Take;
            string patientId = searchSmsResultPatientCommand.PatientId;
            string employeeId = searchSmsResultPatientCommand.EmployeeId;
            string searchText = searchSmsResultPatientCommand.SearchText;
            List<string> serviceCodeFilter = searchSmsResultPatientCommand.ServiceCodeFilter;
            SmsResultFilterType smsResultFilterType = searchSmsResultPatientCommand.SmsResultFilterType;
            string searchTelno = searchSmsResultPatientCommand.SearchTelno;

            var result = await _smsMonitoringQueries.SearchSmsResultPatient(
                fromDateTime: fromDateTime,
                toDateTime: toDateTime,
                skip: skip,
                take: take,
                patientId: patientId,
                employeeId: employeeId,
                searchText: searchText,
                serviceCodeFilter: serviceCodeFilter,
                smsResultFilterType: smsResultFilterType,
                searchTelno: searchTelno
                ).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 환자 SMS 재전송
        /// </summary>
        /// <param name="resendPatientSmsCommand"></param>
        /// <returns></returns>
        [Route("patient-sms-resend")]
        [HttpPost]
        public async Task<IActionResult> ResendPatientSms([FromBody]ResendPatientSmsCommand resendPatientSmsCommand)
        {
            bool commandResult = false;

            if (resendPatientSmsCommand != null)
            {
                commandResult = await _mediator.Send(resendPatientSmsCommand).ConfigureAwait(false);
            }

            return CreatedAtRoute(new { Result = commandResult }, commandResult);
        }
    }
}