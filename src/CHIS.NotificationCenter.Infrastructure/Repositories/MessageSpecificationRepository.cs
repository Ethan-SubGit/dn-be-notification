using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Linq;
using System.Threading.Tasks;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
namespace CHIS.NotificationCenter.Infrastructure.Repositories
{
    public class MessageSpecificationRepository : IMessageSpecificationRepository
    {
        private readonly NotificationCenterContext _context;
        public IUnitOfWork UnitOfWork => _context;
        private readonly ICallContext _callContext;
        public MessageSpecificationRepository(NotificationCenterContext context, ICallContext callContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
        }

        public MessageSpecification Create(MessageSpecification messageSpecification)
        {
            if (messageSpecification != null)
            {
                return _context.MessageSpecifications.Add(messageSpecification).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(messageSpecification));
            }
        }

        public MessageSpecification Update(MessageSpecification messageSpecification)
        {
            if (messageSpecification != null)
            {
                return _context.MessageSpecifications.Update(messageSpecification).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(messageSpecification));
            }
        }

        #region #### 수신정책 삭제
        public DepartmentPolicy DeleteDepartmentPolicy(DepartmentPolicy departmentPolicy)
        {
            if (departmentPolicy != null)
            {
                return _context.DepartmentPolicies.Remove(departmentPolicy).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(departmentPolicy));
            }
        }

        public EncounterPolicy DeleteEncounterPolicy(EncounterPolicy encounterPolicy)
        {
            if (encounterPolicy != null)
            {
                return _context.EncounterPolicies.Remove(encounterPolicy).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(encounterPolicy));
            }
        }

        public EmployeeRecipient DeleteEmployeeRecipient(EmployeeRecipient employeeRecipient)
        {
            if (employeeRecipient != null)
            {
                return _context.EmployeeRecipients.Remove(employeeRecipient).Entity;
            }
            else
            {
                throw new ArgumentNullException(nameof(employeeRecipient));
            }
        } 
        #endregion

        public async Task<MessageSpecification> Retrieve(string id)
        {
            MessageSpecification messageSpecification = _context.MessageSpecifications
                                                        .Where(c => c.Id == id
                                                            && c.TenantId == _callContext.TenantId
                                                            && c.HospitalId == _callContext.HospitalId
                                                        ).FirstOrDefault();

            #region # 불필요한 수신정책 가져오는 것을 방지(2019/12/18)
            /*
                if (messageSpecification != null)
                { 
                    await _context.Entry(messageSpecification).Collection(c => c.EmployeeRecipients).LoadAsync().ConfigureAwait(false);
                    await _context.Entry(messageSpecification).Collection(c => c.DepartmentPolicies).LoadAsync().ConfigureAwait(false);
                    await _context.Entry(messageSpecification).Collection(c => c.EncounterPolicies).LoadAsync().ConfigureAwait(false);
                }
                */
            #endregion

            //문자메시지 전송 경우에는 실행시키지 않음.
            if (messageSpecification != null && messageSpecification.ServiceType != Domain.Enum.NotificationServiceType.SMS)
            {
                await _context.Entry(messageSpecification).Collection(c => c.EmployeeRecipients).LoadAsync().ConfigureAwait(false);
                await _context.Entry(messageSpecification).Collection(c => c.DepartmentPolicies).LoadAsync().ConfigureAwait(false);

                //쪽지 전송의경우에는 수진정책없음
                if (messageSpecification.ServiceType != Domain.Enum.NotificationServiceType.CommunicationNote)
                {
                    await _context.Entry(messageSpecification).Collection(c => c.EncounterPolicies).LoadAsync().ConfigureAwait(false);
                }

            }

            return messageSpecification;
        }

        /// <summary>
        /// 수신정책을 제외한 MessageSpecification 리턴 
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        public MessageSpecification FindByServiceCode(string serviceCode)
        {
            
            MessageSpecification messageSpecification = _context.MessageSpecifications
                                                        .Where(p => p.ServiceCode == serviceCode
                                                            && p.TenantId == _callContext.TenantId
                                                            && p.HospitalId == _callContext.HospitalId
                                                        )
                                                        .FirstOrDefault();

            
            //문자메시지 전송 경우에는 실행시키지 않음.
            //if (messageSpecification != null || messageSpecification.ServiceType != Domain.Enum.NotificationServiceType.SMS)
            //{
            //    await _context.Entry(messageSpecification).Collection(c => c.EmployeeRecipients).LoadAsync().ConfigureAwait(false);
            //    await _context.Entry(messageSpecification).Collection(c => c.DepartmentPolicies).LoadAsync().ConfigureAwait(false);

            //    //쪽지 전송의경우에는 수진정책없음
            //    if (messageSpecification.ServiceType != Domain.Enum.NotificationServiceType.CommunicationNote)
            //    {
            //        await _context.Entry(messageSpecification).Collection(c => c.EncounterPolicies).LoadAsync().ConfigureAwait(false);
            //    }
                
            //}
            

            return messageSpecification;
        }

        public async Task<MessageSpecification> FindByServiceCodeWithPolicy(string serviceCode)
        {
            //MessageSpecification messageSpecification = _context.MessageSpecifications.Where(c => c.ServiceCode == serviceCode).FirstOrDefault();
            MessageSpecification messageSpecification = _context.MessageSpecifications
                                                        .Where(p => p.ServiceCode == serviceCode
                                                            && p.TenantId == _callContext.TenantId
                                                            && p.HospitalId == _callContext.HospitalId
                                                        )
                                                        .FirstOrDefault();

            //문자메시지 전송 경우에는 실행시키지 않음.
            if (messageSpecification != null && messageSpecification.ServiceType != Domain.Enum.NotificationServiceType.SMS)
            {
                await _context.Entry(messageSpecification).Collection(c => c.EmployeeRecipients).LoadAsync().ConfigureAwait(false);
                await _context.Entry(messageSpecification).Collection(c => c.DepartmentPolicies).LoadAsync().ConfigureAwait(false);

                //쪽지 전송의경우에는 수진정책없음
                if (messageSpecification.ServiceType != Domain.Enum.NotificationServiceType.CommunicationNote)
                {
                    await _context.Entry(messageSpecification).Collection(c => c.EncounterPolicies).LoadAsync().ConfigureAwait(false);
                }

            }

            return messageSpecification;
        }

        #region ##### callback No CUD
        public MessageCallbackNoConfig CreateCallbackNo(MessageCallbackNoConfig messageCallbackNo)
        {
            if (messageCallbackNo != null)
            {
                return _context.MessageCallbackNoConfigs.Add(messageCallbackNo).Entity;
            }
            else
            {
                throw new ArgumentException(nameof(messageCallbackNo));
            }
        }

        public MessageCallbackNoConfig UpdateCallbackNo(MessageCallbackNoConfig messageCallbackNo)
        {
            if (messageCallbackNo != null)
            {
                return _context.MessageCallbackNoConfigs.Update(messageCallbackNo).Entity;
            }
            else
            {
                throw new ArgumentException(nameof(messageCallbackNo));
            }
        }

        public MessageCallbackNoConfig DeleteCallbackNo(string id)
        {
            MessageCallbackNoConfig messageCallbackNo = _context.MessageCallbackNoConfigs.FirstOrDefault(p => p.Id == id);
            if (messageCallbackNo != null)
            {
                return _context.MessageCallbackNoConfigs.Remove(messageCallbackNo).Entity;
            }
            return messageCallbackNo;
        }

        public MessageCallbackNoConfig RetrieveCallbackNo(string id)
        {
            return _context.MessageCallbackNoConfigs
                    .FirstOrDefault(
                                p => p.Id == id
                                && p.TenantId == _callContext.TenantId
                                && p.HospitalId == _callContext.HospitalId);
        }
        #endregion

        #region ##### 템플릿 CUD
        public MessageTemplate CreateTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                return _context.MessageTemplates.Add(messageTemplate).Entity;
            }else
            {
                throw new ArgumentException(nameof(messageTemplate));
            }
        }

        public MessageTemplate UpdateTemplate(MessageTemplate messageTemplate)
        {
            if (messageTemplate != null)
            {
                return _context.MessageTemplates.Update(messageTemplate).Entity;
            }
            else
            {
                throw new ArgumentException(nameof(messageTemplate));
            }
        }

        public MessageTemplate DeleteTemplate(string id)
        {
            MessageTemplate messageTemplate = _context.MessageTemplates.FirstOrDefault(p => p.Id == id);
            if (messageTemplate != null)
            {
                return _context.MessageTemplates.Remove(messageTemplate).Entity;
            }
            return messageTemplate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MessageTemplate RetrieveTemplate(string id)
        {
            return _context.MessageTemplates
                    .FirstOrDefault(
                                p => p.Id == id 
                                && p.TenantId == _callContext.TenantId
                                && p.HospitalId == _callContext.HospitalId);
        }

      
        #endregion
    }
}
