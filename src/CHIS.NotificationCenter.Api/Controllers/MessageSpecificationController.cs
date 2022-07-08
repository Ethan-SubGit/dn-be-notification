using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.MessageSpecification;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Api.Controllers
{
    [Produces("application/json")]
    [Route("notificationcenter/v0/message-specifications")]
    public class MessageSpecificationController : BSLBase
    {
        //private readonly ICallContext _context;
        private readonly IMediator _mediator;
        private readonly IMessageSpecificationQueries _messageSpecificationQueries;
        //private readonly IMessageSpecificationRepository _repository;

        public MessageSpecificationController(ICallContext context, IMediator mediator, IMessageSpecificationQueries messageSpecificationQueries) : base(context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _messageSpecificationQueries = messageSpecificationQueries ?? throw new ArgumentNullException(nameof(messageSpecificationQueries));
        }


        /// <summary>
        /// 메시지 규격등록
        /// </summary>
        /// <param name="registerMessageSpecificationCommand"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RegisterMessageSpecification([FromBody]RegisterMessageSpecificationCommand registerMessageSpecificationCommand)
        {
            string commandResult = null;

            if (registerMessageSpecificationCommand != null)
            {
              
                commandResult = await _mediator.Send(registerMessageSpecificationCommand).ConfigureAwait(false);
            }

            return commandResult != null ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// 메시지 규격 수정
        /// </summary>
        /// <param name="modifyMessageSpecificationCommand"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> ModifyMessageSpecification([FromBody]ModifyMessageSpecificationCommand modifyMessageSpecificationCommand)
        {
            bool commandResult = false;

            if (modifyMessageSpecificationCommand != null)
            {

                commandResult = await _mediator.Send(modifyMessageSpecificationCommand).ConfigureAwait(false);
            }

            return commandResult  ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// 메시지 규격 리스트 조회 (0:inbox, 1:sms, 2:쪽지)
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [Route("service-type/{serviceType}")]
        [HttpGet]
        public async Task<IActionResult> RetrieveMessageSpecifications(int serviceType)
        {
            var result = await _messageSpecificationQueries.RetrieveMessageSpecifications(serviceType).ConfigureAwait(false);

            return Ok(result);
        }



        /// <summary>
        /// 인박스 메시지 규격 조회
        /// </summary>
        /// <returns></returns>
        [Route("inbox/search")]
        [HttpGet]
        public async Task<IActionResult> SearchInboxMessageSpecifications()
        {
            var result = await _messageSpecificationQueries.SearchInboxMessageSpecifications().ConfigureAwait(false); 

            return Ok(result);
        }

       

        /// <summary>
        /// 메시지 규격 인스턴스 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> FindMessageSpecification(string id)
        {
            var result = await _messageSpecificationQueries.FindMessageSpecification(id).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        [Route("search/{serviceCode}")]
        [HttpGet]
        public async Task<IActionResult> FindMessageSpecificationByServiceCode(string serviceCode)
        {
            var result = await _messageSpecificationQueries.FindMessageSpecificationByServiceCode(serviceCode).ConfigureAwait(false);

            return Ok(result);
        }

        #region ###### 수신정책
        /// <summary>
        /// 수신설정 프로토콜 조회
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Route("recipient-policy-protocol")]
        [HttpGet]
        public async Task<IActionResult> RetrieveRecipientPolicyProtocol(int type)
        {
            var result = await _messageSpecificationQueries.RetrieveRecipientPolicyProtocol(type).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 직원 수신자 삭제
        /// </summary>
        /// <param name="removeEmployeeRecipientCommand"></param>
        /// <returns></returns>
        [Route("employee-recipients")]
        [HttpDelete]
        public async Task<IActionResult> RemoveEmployeeRecipient([FromBody]RemoveEmployeeRecipientCommand removeEmployeeRecipientCommand)
        {
            bool commandResult = false;

            if (removeEmployeeRecipientCommand != null)
            {

                commandResult = await _mediator.Send(removeEmployeeRecipientCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        

        /// <summary>
        /// 부서정책 삭제
        /// </summary>
        /// <param name="removeDepartmentPolicyCommand"></param>
        /// <returns></returns>
        [Route("department-policies")]
        [HttpDelete]
        public async Task<IActionResult> RemoveDepartmentPolicies([FromBody]RemoveDepartmentPolicyCommand removeDepartmentPolicyCommand)
        {
            bool commandResult = false;

            if (removeDepartmentPolicyCommand != null)
            {

                commandResult = await _mediator.Send(removeDepartmentPolicyCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// 수진(담당의료진) 정책 삭제
        /// </summary>
        /// <param name="removeEncounterPolicyCommand"></param>
        /// <returns></returns>
        [Route("encounter-policies")]
        [HttpDelete]
        public async Task<IActionResult> RemoveEncounterPolicies([FromBody]RemoveEncounterPolicyCommand removeEncounterPolicyCommand)
        {
            bool commandResult = false;

            if (removeEncounterPolicyCommand != null)
            {

                commandResult = await _mediator.Send(removeEncounterPolicyCommand).ConfigureAwait(false);
            }

            return commandResult ? (IActionResult)Ok(commandResult) : BadRequest();
        }
        #endregion


        #region ##### 메시지 콜백번호
        /// <summary>
        /// sms 회신번호 등록
        /// </summary>
        /// <param name="registerMessageCallbackNoCommand"></param>
        /// <returns></returns>
        [Route("register-callback-no")]
        [HttpPost]
        public async Task<IActionResult> RegisterMessageCallNo([FromBody]RegisterMessageCallbackNoCommand registerMessageCallbackNoCommand)
        {
            string commandResult = null;
            if (registerMessageCallbackNoCommand != null)
            {
                commandResult = await _mediator.Send(registerMessageCallbackNoCommand).ConfigureAwait(false);
            }
            return commandResult != null ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// sms 회신번호 수정
        /// </summary>
        /// <param name="modifyMessageCallbackNoCommand"></param>
        /// <returns></returns>
        [Route("modify-callback-no")]
        [HttpPut]
        public async Task<IActionResult> ModifyMessageCallbackNo([FromBody]ModifyMessageCallbackNoCommand modifyMessageCallbackNoCommand)
        {
            bool commandResult = false;
            if (modifyMessageCallbackNoCommand != null)
            {

                commandResult = await _mediator.Send(modifyMessageCallbackNoCommand).ConfigureAwait(false);
            }
            return (IActionResult)Ok(commandResult);
        }

       
        /// <summary>
        /// 회신번호 상세정보
        /// </summary>
        /// <param name="id">callbackNo id</param>
        /// <returns></returns>
        [Route("retrive-callback-no/{id}")]
        [HttpGet]
        public async Task<IActionResult> RetriveMessageCallbackNo(string id)
        {
            //var result = _messageSp
            //var result = await _messageSpecificationQueries.RetrievesMessageTemplateByServiceType(serviceType: serviceType).ConfigureAwait(false);
            //var result = _repository.RetrieveCallbackNo(id: id);
            var result = await _messageSpecificationQueries.RetrieveMessageCallbackNo(id:id).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// 유효한 병원 회신번호정보
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("retrives-callback-no")]
        [HttpGet]
        public async Task<IActionResult> RetrivesMessageCallbackNo()
        {

            var result = await _messageSpecificationQueries.RetrievesMessageCallbackNo().ConfigureAwait(false);
            
            return Ok(result);
        }

        /// <summary>
        /// 중복되는 발신번호가 있는지 체크
        /// </summary>
        /// <param name="callbackNo">[0-9, '-']만 허용</param>
        /// <returns></returns>
        [Route("duplicate-no-check/{callbackNo}")]
        [HttpGet]
        public async Task<IActionResult> DuplicateTelnoCheck(string callbackNo)
        {
            var result = await _messageSpecificationQueries.DuplicateCallbackNoCheck(callbackNo: callbackNo).ConfigureAwait(false);

            return Ok(result);
        }
        #endregion

        #region ##### 메시지 템플릿
        /// <summary>
        /// 메시지 템플릿 등록
        /// </summary>
        /// <param name="registerMessageTemplateCommand"></param>
        /// <returns></returns>
        [Route("register-message-template")]
        [HttpPost]
        public async Task<IActionResult> RegisterMessageTemplate([FromBody]RegisterMessageTemplateCommand registerMessageTemplateCommand)
        {
            string commandResult = null;

            if (registerMessageTemplateCommand != null)
            {
                commandResult = await _mediator.Send(registerMessageTemplateCommand).ConfigureAwait(false);
            }

            return commandResult != null ? (IActionResult)Ok(commandResult) : BadRequest();
        }

        /// <summary>
        /// message 템플릿 수정
        /// </summary>
        /// <param name="modifyMessageTemplateCommand"></param>
        /// <returns></returns>
        [Route("modify-message-template")]
        [HttpPut]
        public async Task<IActionResult> ModifyMessageTemplate([FromBody]ModifyMessageTemplateCommand modifyMessageTemplateCommand)
        {
            bool commandResult = false;
            if (modifyMessageTemplateCommand != null)
            {

                commandResult = await _mediator.Send(modifyMessageTemplateCommand).ConfigureAwait(false);
            }
            return (IActionResult)Ok(commandResult);
        }

        /// <summary>
        /// 메시지 템플릿 삭제
        /// </summary>
        /// <param name="removeMessageTemplateCommand"></param>
        /// <returns></returns>
        [Route("remove-message-template")]
        [HttpDelete]
        public async Task<IActionResult> RemoveMessageTemplate([FromBody]RemoveMessageTemplateCommand removeMessageTemplateCommand)
        {
            bool commandResult = false;
            if (removeMessageTemplateCommand != null)
            {

                commandResult = await _mediator.Send(removeMessageTemplateCommand).ConfigureAwait(false);
            }
            //return commandResult != null ? (IActionResult)Ok(commandResult) : BadRequest();
            return (IActionResult)Ok(commandResult);


        }

        /// <summary>
        /// 서비스코드별 템플릿 리스트
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        [Route("retrieves-message-template/{serviceCode}")]
        [HttpGet]
        public async Task<IActionResult> RetrieveMessageTemplates(string serviceCode)
        { 
            var result = await _messageSpecificationQueries.RetrieveMessageTemplatesByServiceCode(serviceCode: serviceCode).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 템플릿 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("retrive-message-template/{id}")]
        [HttpGet]
        public async Task<IActionResult> RetriveMessageTemplate(string id)
        {
            var result = await _messageSpecificationQueries.retrieveMessageTemplateById(id: id).ConfigureAwait(false);

            return Ok(result);
        }

        /// <summary>
        /// 서비스타입별 메시지 템플릿 조회
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        [Route("retrives-message-template")]
        [HttpGet]
        public async Task<IActionResult> RetrivesMessageTemplateByServiceType(NotificationServiceType serviceType = NotificationServiceType.SMS)
        {
            var result = await _messageSpecificationQueries.RetrievesMessageTemplateByServiceType(serviceType: serviceType).ConfigureAwait(false);
            return Ok(result);
        }
        #endregion


    }
}
