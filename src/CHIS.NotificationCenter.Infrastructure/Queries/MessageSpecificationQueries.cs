using CHIS.Framework.Data.ORM;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.MessageSpecification;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CHIS.NotificationCenter.Domain.Enum;
using System;
using CHIS.NotificationCenter.Application.Models.QueryType;

namespace CHIS.NotificationCenter.Infrastructure.Queries
{
    public class MessageSpecificationQueries : DALBase, IMessageSpecificationQueries
    {
        private readonly ICallContext _callContext;
        private readonly NotificationCenterContext _notificationCenterContext;

        public MessageSpecificationQueries(ICallContext context, IMessageSpecificationRepository messageSpecificationRepository, NotificationCenterContext notificationCenterContext) : base(context)
        {
            this.DBCatalog = NotificationCenterContext.DOMAIN_NAME;
            _notificationCenterContext = notificationCenterContext;
            _callContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        // DESC : 쿼리파일에서 RetrieveMessageSpecifications 삭제 체크할것.
        public async Task<IList<MessageSpecificationView>> RetrieveMessageSpecifications(int serviceType)
        {
            IList<MessageSpecificationView> messageSpecificationView;

            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            using (var connection = db.CreateConnection())
            {
                messageSpecificationView = (await connection.SqlTranslateQueryAsync<MessageSpecificationView>("RetrieveMessageSpecifications",
                    new { ServiceType = serviceType, TenantId = tenantId, HospitalId = hospitalId }).ConfigureAwait(false)).ToList();
            }

            return messageSpecificationView;
        }

        /// <summary>
        /// inbox messageSpecification 조회
        /// </summary>
        /// <returns></returns>
        public async Task<IList<MessageSpecificationView>> SearchInboxMessageSpecifications()
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;



            var query =

                    from messageSpecification in _notificationCenterContext.MessageSpecifications
                        .Where(
                        x => x.ServiceType == Domain.Enum.NotificationServiceType.Inbox
                        && x.TenantId == tenantId
                        && x.HospitalId == hospitalId)
                    orderby messageSpecification.ServiceCode
                    select new MessageSpecificationView()
                    {
                        Id = messageSpecification.Id,
                        ServiceType = messageSpecification.ServiceType,
                        ServiceCode = messageSpecification.ServiceCode,
                        Classification = messageSpecification.Classification,
                        MessageCategory = messageSpecification.MessageCategory,
                        Description = messageSpecification.Description,
                        IsDeleted = messageSpecification.IsDeleted,
                        IsSystemProperty = messageSpecification.IsSystemProperty,
                        IsAddRecipient = messageSpecification.IsAddRecipient,
                        MessageCallbackNoConfigId = messageSpecification.MessageCallbackNoConfigId
                    };

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list;
        }

        /// <summary>
        /// sms, 카테고리 목록조회
        /// </summary>
        /// <param name="messageCategory"></param>
        /// <returns></returns>
        public async Task<IList<MessageSpecificationView>> SearchSmsMessageSpecifications(string messageCategory)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;



            var query =

                    from messageSpecification in _notificationCenterContext.MessageSpecifications
                        .Where(x => x.ServiceType == Domain.Enum.NotificationServiceType.SMS
                            && x.MessageCategory == messageCategory
                            && x.TenantId == tenantId
                            && x.HospitalId == hospitalId
                            )
                    orderby messageSpecification.ServiceCode
                    select new MessageSpecificationView()
                    {
                        Id = messageSpecification.Id,
                        ServiceType = messageSpecification.ServiceType,
                        ServiceCode = messageSpecification.ServiceCode,
                        Classification = messageSpecification.Classification,
                        MessageCategory = messageSpecification.MessageCategory,
                        Description = messageSpecification.Description,
                        IsDeleted = messageSpecification.IsDeleted,
                        IsSystemProperty = messageSpecification.IsSystemProperty,
                        IsAddRecipient = messageSpecification.IsAddRecipient,
                        MessageCallbackNoConfigId = messageSpecification.MessageCallbackNoConfigId
                    };

            var list = await query.ToListAsync().ConfigureAwait(false);
            return list;

        }

        /// <summary>
        /// 메시지 스펙 인스턴스 조회
        /// </summary>
        /// <param name="messageSpecificationId"></param>
        /// <returns></returns>
        public async Task<dynamic> FindMessageSpecification(string messageSpecificationId)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            var MessageSpecification = await _notificationCenterContext.MessageSpecifications
                                .Where(i => i.TenantId == tenantId
                                   && i.HospitalId == hospitalId
                                   && i.Id == messageSpecificationId)
                                .FirstOrDefaultAsync().ConfigureAwait(false);

            #region #### 수신추가정보조회
            var DepartmentPolicy = _notificationCenterContext.DepartmentPolicies
                                                .Where(i => i.MessageSpecificationId == messageSpecificationId
                                                && i.TenantId == tenantId
                                                && i.HospitalId == hospitalId);

            var EncounterPolicy = _notificationCenterContext.EncounterPolicies
                                    .Where(i => i.MessageSpecificationId == messageSpecificationId
                                        && i.TenantId == tenantId
                                        && i.HospitalId == hospitalId);

            var EmployeeRecipient = _notificationCenterContext.EmployeeRecipients
                                    .Where(i => i.MessageSpecificationId == messageSpecificationId
                                        && i.TenantId == tenantId
                                        && i.HospitalId == hospitalId);

            #region ## 추가부서정보
            var DepartmentPolicies = from departmentPolicy in DepartmentPolicy
                                     join recipientPolicyProtocol in _notificationCenterContext.RecipientPolicyProtocols
                                     on departmentPolicy.ProtocolCode equals recipientPolicyProtocol.PolicyCode

                                     join department in _notificationCenterContext.Query<DepartmentReadModel>()
                                        .Where(p => p.TenantId == tenantId
                                            && p.HospitalId == hospitalId
                                        )
                                     //on departmentPolicy.DepartmentId equals department.Id into result1
                                     //from department in result1.DefaultIfEmpty()
                                     on departmentPolicy.DepartmentId equals department.Id


                                     join occupation in _notificationCenterContext.Query<OccupationReadModel>()
                                        .Where(p => p.TenantId == tenantId
                                            && p.HospitalId == hospitalId
                                        )
                                     on departmentPolicy.OccupationId equals occupation.Id into result2
                                     from occupation in result2.DefaultIfEmpty()

                                     join jobPosition in _notificationCenterContext.Query<JobPositionReadModel>()
                                        .Where(p => p.TenantId == tenantId
                                            && p.HospitalId == hospitalId
                                        )
                                     on departmentPolicy.JobPositionId equals jobPosition.Id into result3
                                     from jobPosition in result3.DefaultIfEmpty()
                                     where recipientPolicyProtocol.TenantId == tenantId

                                     select new
                                     {
                                         id = departmentPolicy.Id,
                                         ProtocolCode = departmentPolicy.ProtocolCode,
                                         ProtocolName = recipientPolicyProtocol.Name,
                                         DepartmentId = departmentPolicy.DepartmentId,
                                         DepartmentName = department.Name ?? "",
                                         OccupationId = departmentPolicy.OccupationId,
                                         //OccupationName = occupation.Name ?? "",
                                         OccupationName = occupation.Name,
                                         JobPositionId = departmentPolicy.JobPositionId,
                                         //JobPositonName = jobPosition.Name ?? "",
                                         JobPositonName = jobPosition.Name,
                                         MessageSpecificationId = departmentPolicy.MessageSpecificationId
                                     };
            #endregion

            #region ## 추가encounter정보
            var EncounterPolicies = from EP in EncounterPolicy
                                    join RPP in _notificationCenterContext.RecipientPolicyProtocols
                                    on EP.ProtocolCode equals RPP.PolicyCode
                                    where EP.TenantId == tenantId
                                        && EP.HospitalId == hospitalId
                                        && RPP.TenantId == tenantId

                                    select new
                                    {
                                        id = EP.Id
                                        ,
                                        ProtocolCode = EP.ProtocolCode
                                        ,
                                        ProtocolName = RPP.Name
                                        ,
                                        MessageSpecificationId = EP.MessageSpecificationId
                                    };
            #endregion

            #region ## 추가직원정보
            var EmployeeRecipientList = from ER in EmployeeRecipient
                                        join EM in _notificationCenterContext.Query<EmployeeReadModel>()
                                            .Where(p => p.TenantId == tenantId
                                                && p.HospitalId == hospitalId
                                            )
                                        on ER.EmployeeId equals EM.Id into result1

                                        from EM in result1.DefaultIfEmpty()
                                        join DEPT in _notificationCenterContext.Query<DepartmentReadModel>()
                                            .Where(p => p.TenantId == tenantId
                                                && p.HospitalId == hospitalId
                                            )
                                        on EM.DepartmentId equals DEPT.Id into result2
                                        from DEPT in result2.DefaultIfEmpty()
                                        select new
                                        {
                                            id = ER.Id,
                                            EmployeeId = ER.EmployeeId,
                                            displayId = EM.DisplayId ?? "",
                                            MessageSpecificationId = ER.MessageSpecificationId,
                                            EmployeeName = EM.FullName ?? "",
                                            DepartmentName = DEPT.Name ?? ""
                                        };
            #endregion 
            #endregion

            var rtnStructure = new
            {
                MessageSpecification.Id,
                MessageSpecification.ServiceType,
                MessageSpecification.ServiceCode,
                MessageSpecification.MessageCategory,
                MessageSpecification.Classification,
                MessageSpecification.Description,
                MessageSpecification.PredefinedContent,
                MessageSpecification.PostActionType,
                MessageSpecification.IsForceToSendInboxSmsMessage,
                MessageSpecification.IsSelectPatientByActiveEncounter,
                MessageSpecification.IsDeleted,
                MessageSpecification.IsSystemProperty,
                MessageSpecification.IsAddRecipient,
                MessageSpecification.MessageCallbackNoConfigId,
                DepartmentPolicies = await DepartmentPolicies.ToListAsync().ConfigureAwait(false),
                EncounterPolicies = await EncounterPolicies.ToListAsync().ConfigureAwait(false),
                EmployeeRecipients = await EmployeeRecipientList.ToListAsync().ConfigureAwait(false)
            };

            return rtnStructure;
        }



        public async Task<dynamic> FindMessageSpecificationByServiceCode(string serviceCode)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            //var rtnStructure = null;

            //var MessageSpecification = await _notificationCenterContext.MessageSpecifications.Where(i =>
            //   i.TenantId == tenantId
            //   && i.HospitalId == hospitalId
            //   && i.ServiceCode == serviceCode).FirstOrDefaultAsync().ConfigureAwait(false);

            var query = from msgSpec in _notificationCenterContext.MessageSpecifications
                        .Where(i => i.TenantId == tenantId
                            && i.HospitalId == hospitalId
                            && i.ServiceCode == serviceCode
                            )
                        join callback in _notificationCenterContext.MessageCallbackNoConfigs
                        .Where(i => i.TenantId == tenantId
                            && i.HospitalId == hospitalId
                            && !i.IsDeleted
                            )
                        on msgSpec.MessageCallbackNoConfigId equals callback.Id
                        //where msgSpec.HospitalId == hospitalId
                        select new
                        {
                            msgSpec.Id,
                            msgSpec.ServiceType,
                            msgSpec.ServiceCode,
                            msgSpec.MessageCategory,
                            msgSpec.Classification,
                            msgSpec.Description,
                            msgSpec.PredefinedContent,
                            msgSpec.PostActionType,
                            msgSpec.IsForceToSendInboxSmsMessage,
                            msgSpec.IsSelectPatientByActiveEncounter,
                            msgSpec.IsDeleted,
                            msgSpec.IsSystemProperty,
                            msgSpec.IsAddRecipient,
                            msgSpec.MessageCallbackNoConfigId,
                            callback.CallbackNo
                        };
            var MessageSpecification = await query.FirstOrDefaultAsync().ConfigureAwait(false);
            //row2.FirstOrDefaultAsync().ConfigureAwait(false);
            if (MessageSpecification == null)
            {
                return null;
            }

            var rtnStructure = new
            {
                MessageSpecification.Id,
                MessageSpecification.ServiceType,
                MessageSpecification.ServiceCode,
                MessageSpecification.MessageCategory,
                MessageSpecification.Classification,
                MessageSpecification.Description,
                MessageSpecification.PredefinedContent,
                MessageSpecification.PostActionType,
                MessageSpecification.IsForceToSendInboxSmsMessage,
                MessageSpecification.IsSelectPatientByActiveEncounter,
                MessageSpecification.IsDeleted,
                MessageSpecification.IsSystemProperty,
                MessageSpecification.IsAddRecipient,
                MessageSpecification.MessageCallbackNoConfigId,
                MessageSpecification.CallbackNo
                //DepartmentPolicies = await DepartmentPolicies.ToListAsync().ConfigureAwait(false),
                //EncounterPolicies = await EncounterPolicies.ToListAsync().ConfigureAwait(false),
                //EmployeeRecipients = await EmployeeRecipientList.ToListAsync().ConfigureAwait(false)
            };

            return rtnStructure;
        }

        /// <summary>
        /// TenantId, HospitalId 추후 적용 검토
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IList<dynamic>> RetrieveRecipientPolicyProtocol(int type)
        {

            IList<dynamic> list = null;
            string tenantId = _callContext.TenantId;
            //string hospitalId = _callContext.HospitalId;

            using (var connection = db.CreateConnection())
            {
                list = (await connection.SqlTranslateQueryAsync<dynamic>("RetrieveRecipientPolicyProtocol"
                    , param: new { Type = type, TenantId = tenantId }).ConfigureAwait(false)).ToList();
            }

            return list;
        }


        #region ###### 메시지 템플릿

        public async Task<IList<MessageTemplateView>> RetrieveMessageTemplatesByServiceCode(string serviceCode)
        {
            //IList<MessageTemplate> list = null;
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;


            var query = from template in _notificationCenterContext.MessageTemplates
                        join specification in _notificationCenterContext.MessageSpecifications
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                                && p.ServiceCode == serviceCode
                                && !p.IsDeleted
                            )
                        //on template.MessageSpecificationId equals specification.Id 
                        on new { specId = template.MessageSpecificationId, tenantId = template.TenantId, hospitalId = template.HospitalId } equals new { specId = specification.Id, tenantId = specification.TenantId, hospitalId = specification.HospitalId }
                        into specificationProjection
                        from specificationPj in specificationProjection.DefaultIfEmpty()
                        where specificationPj.ServiceCode == serviceCode
                        //specificationPj.ServiceCode.Contains(serviceCode, StringComparison.Ordinal)
                        && template.TenantId == tenantId
                        && template.HospitalId == hospitalId
                        && !template.IsDeleted
                        select new MessageTemplateView()
                        {
                            Id = template.Id,
                            MessageSpecificationId = template.MessageSpecificationId,
                            ContentTemplateScope = template.ContentTemplateScope,
                            IsDeleted = template.IsDeleted,
                            TemplateTitle = template.TemplateTitle,
                            ContentTemplate = template.ContentTemplate
                        };


            return await query.AsNoTracking().ToListAsync().ConfigureAwait(false);

        }

        public async Task<MessageTemplateView> retrieveMessageTemplateById(string id)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            var query = from template in _notificationCenterContext.MessageTemplates
                        where template.Id == id
                        && template.HospitalId == hospitalId
                        && template.TenantId == tenantId
                        select new MessageTemplateView()
                        {
                            Id = template.Id,
                            MessageSpecificationId = template.MessageSpecificationId,
                            ContentTemplateScope = template.ContentTemplateScope,
                            IsDeleted = template.IsDeleted,
                            TemplateTitle = template.TemplateTitle,
                            ContentTemplate = template.ContentTemplate
                        };


            return await query.AsNoTracking().FirstOrDefaultAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 서비스타입별 모든 메시지 템플릿 리턴
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public async Task<IList<MessageTemplateViewByServiceType>> RetrievesMessageTemplateByServiceType(NotificationServiceType serviceType)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            var query = _notificationCenterContext.MessageSpecifications
                        .Where(p => p.ServiceType == serviceType
                            && p.TenantId == tenantId
                            && p.HospitalId == hospitalId
                            && !p.IsDeleted
                        )
                        .Select(spec => new MessageTemplateViewByServiceType()
                        {
                            Classification = spec.Classification,
                            PredefinedContent = spec.PredefinedContent,
                            Description = spec.Description,
                            Id = spec.Id,
                            MessageCategory = spec.MessageCategory,
                            ServiceCode = spec.ServiceCode,
                            ServiceType = spec.ServiceType,
                            MessageTemplates =
                                _notificationCenterContext.MessageTemplates
                                .Where(
                                    //p => p.MessageSpecificationId.Equals(spec.Id, StringComparison.Ordinal)
                                    p => p.MessageSpecificationId == spec.Id
                                    && p.HospitalId == hospitalId
                                    && p.TenantId == tenantId
                                )
                                .Select(n => new MessageTemplateView
                                {
                                    ContentTemplate = n.ContentTemplate,
                                    ContentTemplateScope = n.ContentTemplateScope,
                                    Id = n.Id,
                                    IsDeleted = n.IsDeleted,
                                    MessageSpecificationId = n.MessageSpecificationId,
                                    TemplateTitle = n.TemplateTitle
                                }).ToList()
                            //MessageTemplates = null
                        });

            return await query.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }
        #endregion

        #region ##### 메시지 회신번호
        /// <summary>
        /// 상세 콜백번호 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MessageCallbackNoConfig> RetrieveMessageCallbackNo(string id)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            var query = await _notificationCenterContext.MessageCallbackNoConfigs
                        .Where(p => p.HospitalId == hospitalId
                                && p.TenantId == tenantId
                                && p.IsDeleted == false
                                && p.Id == id
                            )
                        .FirstOrDefaultAsync().ConfigureAwait(false);

            return query;

        }


        /// <summary>
        /// 병원 회신번호목록
        /// </summary>
        /// <returns></returns>
        public async Task<IList<MessageCallbackNoConfig>> RetrievesMessageCallbackNo()
        {
            //order by IsMaster desc, isSystemProperty desc, DataFirstRegisteredDateTimeUtc asc
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            var query = _notificationCenterContext.MessageCallbackNoConfigs
                        .OrderByDescending(p => p.IsMaster).ThenByDescending(p => p.IsSystemProperty)
                        .ThenBy(p => p.DataFirstRegisteredDateTimeUtc)
                        .Where(p => p.HospitalId == hospitalId
                                && p.TenantId == tenantId
                                && p.IsDeleted == false
                            );



            return await query.AsNoTracking().ToListAsync().ConfigureAwait(false);

        }


        /// <summary>
        /// 중복되는 발신번호가 있는지 체크
        /// </summary>
        /// <param name="callbackNo"></param>
        /// <returns></returns>
        public async Task<bool> DuplicateCallbackNoCheck(string callbackNo)
        {
            string tenantId = _callContext.TenantId;
            string hospital = _callContext.HospitalId;
            string convertCallbackNo = callbackNo.Replace("-", "", StringComparison.OrdinalIgnoreCase).Replace(" ", "", StringComparison.OrdinalIgnoreCase);
            var query = await _notificationCenterContext.MessageCallbackNoConfigs
                        .CountAsync(p => p.HospitalId == hospital
                                && p.TenantId == tenantId
                                && p.CallbackNo == convertCallbackNo
                        ).ConfigureAwait(false);
            return query > 0;
        }

        /// <summary>
        /// 서비스코드로 병원 발신번호를 조회
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        public async Task<string> GetCallbackNoByServiceCode(string serviceCode)
        {
            string tenantId = _callContext.TenantId;
            string hospital = _callContext.HospitalId;

            var basicQuery = from msgSpec in _notificationCenterContext.MessageSpecifications
                            .Where(p => p.TenantId == tenantId && p.HospitalId == hospital)
                             join msgConfig in _notificationCenterContext.MessageCallbackNoConfigs
                                 .Where(
                                     p => p.TenantId == tenantId && p.HospitalId == hospital
                                     && !p.IsDeleted
                                 )
                             on msgSpec.MessageCallbackNoConfigId equals msgConfig.Id
                             where msgSpec.ServiceCode == serviceCode
                             select new
                             {
                                 callbackNo = msgConfig.CallbackNo,
                                 orderNo = 1
                             };

            var basicResult = await basicQuery.Select(p => p.callbackNo).Take(1).FirstAsync().ConfigureAwait(false);
            //매칭되는결과가 없다면
            if (basicResult == null)
            {
                var defaultCallbackNoQuery = from msgConfig in _notificationCenterContext.MessageCallbackNoConfigs
                                             where msgConfig.TenantId == tenantId
                                                && msgConfig.HospitalId == hospital
                                                && msgConfig.IsMaster == true
                                                && msgConfig.IsDeleted == false
                                             select new
                                             {
                                                 callbackNo = msgConfig.CallbackNo,
                                                 orderNo = 2
                                             };
                basicResult = await defaultCallbackNoQuery.Select(p => p.callbackNo).Take(1).FirstAsync().ConfigureAwait(false);
            }


            return basicResult.Replace(" ", "").Replace("-", "");

        }
        #endregion
    }
}
