using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Queries.ReadModels;

namespace CHIS.NotificationCenter.Infrastructure.Repositories
{
    public class SmsMonitoringRepository : ISmsMonitoringRepository
    {
        private readonly NotificationCenterContext _context;
        public IUnitOfWork UnitOfWork => _context;
        private readonly ITimeManager _timeManager;
        private readonly ICallContext _callContext;
        public SmsMonitoringRepository(NotificationCenterContext context, ICallContext callContext,ITimeManager timeManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
        }

        public SmsReceiveLog CreateSmsReceiveLog(SmsReceiveLog smsReceiveLog)
        {
            if (smsReceiveLog != null)
            {
                return _context.SmsReceiveLogs.Add(smsReceiveLog).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(smsReceiveLog));
            }
        }

        public SmsSendLog CreateSmsSendLog(SmsSendLog smsSendLog)
        {
            if (smsSendLog != null)
            {
                return _context.SmsSendLogs.Add(smsSendLog).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(smsSendLog));
            }
        }
        public async Task<SmsReceiveLog> FindSmsReceiveLog(string id)
        {
            return await _context.SmsReceiveLogs
                        .Where(i => i.Id == id
                                && i.TenantId == _callContext.TenantId
                                && i.HospitalId == _callContext.HospitalId)
                        .FirstOrDefaultAsync().ConfigureAwait(false);
        }



        public async Task<List<SmsReceiveLog>> FindSmsReceiveLogByMessageDispatchItemId(string messageDispatchItemId)
        {
                

            return await _context.SmsReceiveLogs
                        .Where(i => i.MessageDispatchItemId == messageDispatchItemId
                                && i.TenantId == _callContext.TenantId
                                && i.HospitalId == _callContext.HospitalId)
                        .ToListAsync().ConfigureAwait(false);
        }


        public async Task<SmsSendLog> FindSmsSendLogByMessageDispatchItemId(string messageDispatchItemId)
        {
            return await _context.SmsSendLogs
                        .Where(
                            i => i.MessageDispatchItemId == messageDispatchItemId
                            && i.TenantId == _callContext.TenantId
                            && i.HospitalId == _callContext.HospitalId)
                        .FirstOrDefaultAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// 배치 처리를 위한 발송항목 리스트
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public async Task<List<SmsSendLog>> PrepareSmsBatchExecution(string batchId)
        {
            // TO-DO : MessageDispatchItem 에서 IsReservedSms 가 true 인거 리스트 및 해당 리스트의 처리상태를 Inprogress로 변경함.  
            DateTime currentDateTimeLocal = _timeManager.GetNow();
            //DateTime CurrentNow = DateTime.Now;

            var item = await _context.SmsSendLogs
                            .Where(i =>  
                                (
                                    (i.IsReservedSms && i.ReservedTime.Value <= currentDateTimeLocal)
                                    || 
                                    (!i.IsReservedSms)
                                )
                                && i.SmsProgressStatus == Domain.Enum.SmsProgressStatus.BeforeProgress
                                && i.TenantId == _callContext.TenantId
                                && i.HospitalId  == _callContext.HospitalId
                                )
                            .ToListAsync().ConfigureAwait(false);

            item.ForEach(i => i.SmsProgressStatus = Domain.Enum.SmsProgressStatus.InProgress);

            //await _context.SaveChangesAsync().ConfigureAwait(false);

            return item;
        }

        /// <summary>
        /// 통신사 수신결과 업데이트 되지않은 리스트 추출함.
        /// </summary>
        /// <param name="messageIds"></param>
        /// <returns></returns>
        public async Task<List<SmsReceiveLog>> SearchUnprocessedReceiveLogs(int delayMinute)
        {
            DateTime currentLocalDateTime = _timeManager.GetNow().AddMinutes(delayMinute);

            var items = await _context.SmsReceiveLogs
                              .Where(p => !string.IsNullOrEmpty(p.SmsId)
                                    && string.IsNullOrEmpty(p.StatusName)
                                    && p.RequestTime <= currentLocalDateTime
                                    && p.TenantId == _callContext.TenantId
                                    && p.HospitalId  == _callContext.HospitalId
                               )
                               .ToListAsync().ConfigureAwait(false);


            return items;
        }


        public async Task<List<SmsReceiveLog>> FindPatientMergingTargets(string closingPatientId)
        {
            return await _context.SmsReceiveLogs
                        .Where(i => i.TenantId == _callContext.TenantId
                            && i.HospitalId == _callContext.HospitalId
                            && i.ActorId == closingPatientId
                            && (i.SmsRecipientType == Domain.Enum.SmsRecipientType.Patient || i.SmsRecipientType == Domain.Enum.SmsRecipientType.Guardian)
                            )
                        .ToListAsync().ConfigureAwait(false);

        }


    }


}
