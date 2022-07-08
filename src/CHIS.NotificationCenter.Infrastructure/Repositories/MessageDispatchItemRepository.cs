using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using Microsoft.EntityFrameworkCore;

namespace CHIS.NotificationCenter.Infrastructure.Repositories
{
    public class MessageDispatchItemRepository : IMessageDispatchItemRepository
    {
        public IUnitOfWork UnitOfWork => _dbContext;

        private readonly NotificationCenterContext _dbContext;
        //private readonly ITimeManager               _timeManager;
        private readonly ICallContext _callContext;

        public MessageDispatchItemRepository(
              NotificationCenterContext dbContext
            , ICallContext callContext
            //, ITimeManager timeManager
            )
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            //_timeManager = timeManager  ?? throw new ArgumentNullException(nameof(timeManager));
        }

        public MessageDispatchItem Create(MessageDispatchItem messageDispatchItem)
        {
            if (messageDispatchItem != null)
            {
                return _dbContext.MessageDispatchItems.Add(messageDispatchItem).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(messageDispatchItem));
            }

        }

        public async Task<MessageDispatchItem> Retrieve(string messageDispatchItemId)
        {
            MessageDispatchItem messageDispatchItem = await RetrieveWithNoPolicy(messageDispatchItemId).ConfigureAwait(false);

            if (messageDispatchItem != null)
            {
                await _dbContext.Entry(messageDispatchItem).Collection(c => c.AssignedEmployeeRecipients).LoadAsync().ConfigureAwait(false);
                await _dbContext.Entry(messageDispatchItem).Collection(c => c.AssignedDepartmentPolicies).LoadAsync().ConfigureAwait(false);
                await _dbContext.Entry(messageDispatchItem).Collection(c => c.AssignedEncounterPolicies).LoadAsync().ConfigureAwait(false);
                await _dbContext.Entry(messageDispatchItem).Collection(c => c.ContentParameters).LoadAsync().ConfigureAwait(false);
            }

            return messageDispatchItem;
        }

        /// <summary>
        /// 수신정책없는 MessageDispatchItems
        /// </summary>
        /// <param name="messageDispatchItemId">messageDispatchItemId</param>
        /// <returns></returns>
        public async Task<MessageDispatchItem> RetrieveWithNoPolicy(string messageDispatchItemId)
        {
            return await _dbContext.MessageDispatchItems
                        .Where(c =>
                            c.TenantId == _callContext.TenantId
                            && c.HospitalId == _callContext.HospitalId
                            && c.Id == messageDispatchItemId
                        ).FirstOrDefaultAsync().ConfigureAwait(false);
        }
      

        //public async Task<List<MessageDispatchItem>> PrepareUnProcessedReservedSmsMessageItem(string timeZoneId)
        //{
        //    // TO-DO : MessageDispatchItem 에서 IsReservedSms 가 true 인거 리스트 및 해당 리스트의 처리상태를 Inprogress로 변경함.  
        //    DateTime CurrentNow = _timeManager.GetNow(timeZoneId);
        //    //DateTime CurrentNow = DateTime.Now;

        //    var item = await _context.MessageDispatchItems.Where(i => i.ServiceType == Domain.Enum.NotificationServiceType.SMS
        //                                                           && i.IsReservedSms == true
        //                                                           && i.ReservedSmsDateTime.Value <= CurrentNow
        //                                                           && i.SmsSendStatus == Domain.Enum.SmsSendStatusType.BeforeProgress)
        //                                                  .ToListAsync().ConfigureAwait(false);

        //    item.ForEach(i => i.SmsSendStatus = Domain.Enum.SmsSendStatusType.InProgress);

        //    await _context.SaveChangesAsync();

        //    return item;
        //}

        public async Task<MessageDispatchItem> ModifyMessageDispatchItem(MessageDispatchItem item)
        {
            return await Utils.RepositorySupportFunction.CreateOrModifyEntity(_dbContext, item).ConfigureAwait(false);
        }

        /// <summary>
        /// 환자 합본대상 messageDispatchItem 반환
        /// </summary>
        /// <param name="closingPatientId"></param>
        /// <returns></returns>
        public async Task<List<MessageDispatchItem>> FindPatientMergingTargets(string closingPatientId)
        {
            return await _dbContext.MessageDispatchItems
                .Where(i => i.PatientId == closingPatientId
                        && i.TenantId == _callContext.TenantId
                        && i.HospitalId == _callContext.HospitalId)
                .ToListAsync().ConfigureAwait(false);

        }

       
    }


}
