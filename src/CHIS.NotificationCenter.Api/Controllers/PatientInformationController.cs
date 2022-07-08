using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.Commands.PatientInformation;
//using CHIS.NotificationCenter.Application.Models.EncounterAggregate;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.Enum;
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
    [Route("notificationcenter/v0/patient-information")]
    public class PatientInformationController : BSLBase
    {
        //private readonly ICallContext _context;
        //private readonly IMediator _mediator;
        private readonly IPatientInformationQueries _queries;
        //private readonly IEncounterRepository _repository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mediator"></param>
        /// <param name="queries"></param>
        public PatientInformationController(ICallContext context, IMediator mediator
            , IPatientInformationQueries queries
            //, IEncounterRepository repository
            ) : base(context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            //_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _queries = queries ?? throw new ArgumentNullException(nameof(queries));
            //_repository = repository ?? throw new ArgumentNullException(nameof(_repository));
        }

        /// <summary>
        /// 환자정보
        /// </summary>
        /// <param name="id"></param>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        [Route("patient-informations")]
        [HttpGet]
        public async Task<IActionResult> RetrievePatientInfomation(string id, string encounterId)
        {
            // DESC : SQL에서 ef로 변환함.
            var result = await _queries.RetrievePatientInfomationV2(id, encounterId).ConfigureAwait(false); 

            return Ok(result);
        }

        /// <summary>
        /// 환자정보 V2
        /// </summary>
        /// <param name="id"></param>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        [Route("patient-informationsV2")]
        [HttpGet]
        public async Task<IActionResult> RetrievePatientInfomationV2(string id, string encounterId)
        {
            var result = await _queries.RetrievePatientInfomationV2(id, encounterId).ConfigureAwait(false);

            return Ok(result);
        }

        #region ### 담당의료진 정보
        /// <summary>
        /// 담당의료진 리스트 출력 (by EncounterId , inbox Message 전송 => 담당의료진)
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        [Route("retrieve-patient-participants")]
        [HttpGet]
        public async Task<IActionResult> RetrievePatientParticipant(string encounterId)
        {
            //var result = await _repository.RetrievePatientParticipant(encounterId);
            var result = await _queries.RetrievePatientParticipant(encounterId).ConfigureAwait(false);

            return Ok(result);
        } 
        #endregion

        /// <summary>
        /// 환자 관련 전화번호 조회
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [Route("patient-contact/search")]
        [HttpGet]
        public async Task<IActionResult> RetrievePatientContact(string patientId)
        {
            var result = await _queries.FindPatientContact(patientId).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 환자연락처 조회
        /// </summary>
        /// <param name="searchPatientContactsCommnad"></param>
        /// <returns></returns>
        [Route("patient-contact/search-list")]
        [HttpPost]
        public async Task<IActionResult> SearchPatientContacts([FromBody]SearchPatientContactsCommnad searchPatientContactsCommnad)
        {
            if (searchPatientContactsCommnad == null)
            {
                return BadRequest();
            }
            
            List<string> patientIds = searchPatientContactsCommnad.PatientIds;
            var result = await _queries.SearchPatientContacts(patientIds).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 인박스 환자검색에서 사용(관련환자 목록)
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [Route("patient-list/inbox")]
        [HttpGet]
        public async Task<IActionResult> RetrievesPatientListForInbox(string employeeId)
        {
            var result = await _queries.FindPatientListForInbox(employeeId: employeeId).ConfigureAwait(false);

            return Ok(result);
        }
    }
}
