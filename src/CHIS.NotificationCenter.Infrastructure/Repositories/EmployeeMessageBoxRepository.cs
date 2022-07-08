using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHIS.Framework.Middleware;
using CHIS.Framework.Core;

namespace CHIS.NotificationCenter.Infrastructure.Repositories
{
    public class EmployeeMessageBoxRepository : IEmployeeMessageBoxRepository
    {
        private readonly NotificationCenterContext _context;
        private readonly ICallContext _callContext;
        public IUnitOfWork UnitOfWork => _context;

        public EmployeeMessageBoxRepository(NotificationCenterContext context
             , ICallContext callContext, ITimeManager timeManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
        }

        public EmployeeMessageBox Create(EmployeeMessageBox employeeMessageBox)
        {
            if (employeeMessageBox != null)
            {
                return _context.EmployeeMessageBoxes.Add(employeeMessageBox).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(employeeMessageBox));
            }
        }

        public EmployeeMessageBox Update(EmployeeMessageBox employeeMessageBox)
        {
            if (employeeMessageBox != null)
            {
                return _context.EmployeeMessageBoxes.Update(employeeMessageBox).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(employeeMessageBox));
            }
        }

        public async Task<List<EmployeeMessageBox>> Retrieve(List<string> employeeList)
        {
            var employeeMessageBoxes = await _context.EmployeeMessageBoxes.Where(c =>
                c.TenantId == _callContext.TenantId
                && c.HospitalId == _callContext.HospitalId
                && employeeList.Contains(c.EmployeeId)).ToListAsync().ConfigureAwait(false);

            //if (employeeMessageBoxes != null)
            //{
            //    foreach (EmployeeMessageBox employeeMessageBox in employeeMessageBoxes)
            //    {
            //        await _context.Entry(employeeMessageBox).Collection(c => c.MessageExclusionFilters).LoadAsync().ConfigureAwait(false);
            //    }
            //}

            return employeeMessageBoxes;
        }
        public async Task<EmployeeMessageBox> RetrieveByEmployeeId(string employeeId)
        {
            EmployeeMessageBox employeeMessageBox = await _context.EmployeeMessageBoxes.Where(c =>
                c.TenantId == _callContext.TenantId
               && c.HospitalId == _callContext.HospitalId
               && c.EmployeeId == employeeId).FirstOrDefaultAsync().ConfigureAwait(false);

            return employeeMessageBox;
        }
        public async Task<EmployeeMessageBox> Retrieve(string employeeMessageBoxId)
        {
            EmployeeMessageBox employeeMessageBox =  await _context.EmployeeMessageBoxes.Where(c =>
                 c.TenantId == _callContext.TenantId
                && c.HospitalId == _callContext.HospitalId
                && c.Id  == employeeMessageBoxId).FirstOrDefaultAsync().ConfigureAwait(false); 

            //if (employeeMessageBox != null)
            //{
            //    await _context.Entry(employeeMessageBox).Collection(c => c.MessageExclusionFilters).LoadAsync().ConfigureAwait(false);
            //}

            return employeeMessageBox;
        }

        public async Task<List<EmployeeMessageBox>> RetrieveEmployeeMessageBoxList(string messageDispatchItemId)
        {

            var rtnValue = from emi in _context.EmployeeMessageInstances
                           join emb in _context.EmployeeMessageBoxes
                            on emi.EmployeeMessageBoxId equals emb.Id
                           where emi.MessageDispatchItemId == messageDispatchItemId
                                && emi.IsInbound
                                && emi.TenantId == _callContext.TenantId
                                && emi.HospitalId == _callContext.HospitalId
                                && emb.TenantId == _callContext.TenantId
                                && emb.HospitalId == _callContext.HospitalId
                            select emb;

          

            return await rtnValue.ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<EmployeeMessageInstance>> RetrieveUnhandledEmployeeMessageInstancesByMessageDispatchItemId(string messageDispatchItemId)
        {
            var employeeMessageInstances = await _context.EmployeeMessageInstances
                                        .Where(c => 
                                            c.MessageDispatchItemId == messageDispatchItemId
                                            && !c.IsHandled 
                                            && c.IsInbound
                                            && c.TenantId == _callContext.TenantId 
                                            && c.HospitalId == _callContext.HospitalId
                                        ).ToAsyncEnumerable().ToList().ConfigureAwait(false);

            return employeeMessageInstances;
        }

        /// <summary>
        /// 읽은 메시지 카운트
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public int GetUnhandledEmployeeMessageInstanceCountByMessageDispatchItemId(string messageDispatchItemId)
        {
            var returnCount = 0;
            returnCount = _context.EmployeeMessageInstances
                            .Where(p => p.MessageDispatchItemId == messageDispatchItemId
                                && p.IsHandled && p.IsInbound
                                && p.TenantId == _callContext.TenantId
                                && p.HospitalId == _callContext.HospitalId
                            ).Count();

            return returnCount;
        }

        public async Task<EmployeeMessageInstance> RetrieveEmployeeMessageInstance(string id)
        {
            var employeeMessageInstance = await _context.EmployeeMessageInstances
                                        .Where(c => c.Id == id
                                            && !c.IsHandled 
                                            && c.IsInbound
                                            && c.TenantId == _callContext.TenantId
                                            && c.HospitalId == _callContext.HospitalId
                                        ).ToAsyncEnumerable().FirstOrDefault().ConfigureAwait(false);  

            return employeeMessageInstance;
        }

        public async Task<EmployeeMessageInstance> RetrieveEmployeeMessageInstance(string employeeMessageBoxId, string messageDispatchItemId , bool isHandled , bool isInbound)
        {
            var employeeMessageInstance = await _context.EmployeeMessageInstances
                                        .Where(c => c.EmployeeMessageBoxId  == employeeMessageBoxId
                                            && c.IsHandled  == isHandled  
                                            && c.IsInbound == isInbound 
                                            && c.MessageDispatchItemId == messageDispatchItemId
                                            && c.TenantId == _callContext.TenantId
                                            && c.HospitalId == _callContext.HospitalId
                                        ).ToAsyncEnumerable()
                                        .FirstOrDefault().ConfigureAwait(false);

            return employeeMessageInstance;
        }

        public EmployeeMessageInstance UpdateEmployeeMessageInstance(EmployeeMessageInstance employeeMessageInstance)
        {
            if (employeeMessageInstance != null)
            {
                return _context.EmployeeMessageInstances.Update(employeeMessageInstance).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(employeeMessageInstance));
            }
        }

    }
}
