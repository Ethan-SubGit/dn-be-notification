using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Domain.AggregateModels.NotificationCenterConfigurationAggregate;
//using CHIS.NotificationCenter.Application.Queries;


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
    [Route("notificationcenter/v0/notification-center-configurations")]
    public class NotificationCenterConfigurationController : BSLBase
    {
        //private readonly ICallContext _context;
        //private readonly IMediator _mediator;
        //private readonly INotificationCenterConfigurationRepository _repository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="mediator"></param>
        /// <param name="repository"></param>
        public NotificationCenterConfigurationController(ICallContext context, IMediator mediator, INotificationCenterConfigurationRepository repository) : base(context)
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            //_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            //_repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("onesignal-application-id")]
        [HttpGet]
        public async Task<IActionResult> FindOnesignalApplicationId()
        {
            string onesignalApplicationId = CHIS.Framework.Core.Configuration.ConfigurationManager.AppSettings.GetDictionary()["OneSignalAppId"];

            return Ok(onesignalApplicationId);
        }
       
    }
}
