using CHIS.Framework.Core;
using CHIS.Framework.Core.Localization;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Queries.ReadModels.CommunicationNote;
using CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
using CHIS.Share.MedicalAge;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Infrastructure.Queries
{

    public class CommunicationNoteQueries : DALBase, ICommunicationNoteQueries
    {
        private readonly ICallContext _callContext;
        private readonly ITimeManager _timeManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly NotificationCenterContext _notificationCenterContext;



        public CommunicationNoteQueries(ICallContext callContext
            , ITimeManager timeManager
            , ILocalizationManager localizationManager
            , NotificationCenterContext notificationCenterContext) : base(callContext)
        {
            this.DBCatalog = NotificationCenterContext.DOMAIN_NAME;
            _notificationCenterContext = notificationCenterContext;
            this._localizationManager = localizationManager;
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
        }


        #region #### 읽지않은 쪽지 갯수

        /// <summary>
        /// 쪽지 미확인 카운트 조회 (searchText가 empty이면 전체, empty가 아니면 search된 카운트)
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<int> SearchUnreadCount(string employeeId, string searchText)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;



            var query =

                        from employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                        join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                        join messageSpecification in _notificationCenterContext.MessageSpecifications
                            .Where(x => x.ServiceType == Domain.Enum.NotificationServiceType.CommunicationNote)
                        on messageDispatchItem.ServiceCode equals messageSpecification.ServiceCode

                        where employeeMessageInstance.TenantId == tenantId
                          && employeeMessageInstance.HospitalId == hospitalId
                          && employeeMessageInstance.EmployeeId == employeeId
                          && !employeeMessageInstance.IsReaded
                          && employeeMessageInstance.IsInbound
                          && (string.IsNullOrEmpty(searchText) || EF.Functions.Like(employeeMessageInstance.Content, "%" + searchText + "%"))
                          && !messageDispatchItem.IsCanceled

                          //new 0122
                          && messageSpecification.TenantId == tenantId
                          && messageSpecification.HospitalId == hospitalId
                        select new { employeeMessageInstance.Id };

            int unreadCount = await query.CountAsync().ConfigureAwait(false);

            return unreadCount;
        }
        #endregion

        #region #### 받은 쪽지목록 조회
        /// <summary>
        /// 받은 쪽지목록 조회
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="patientId"></param>
        /// <param name="searchText"></param>
        /// <param name="handleStatusFilter"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<CommunicationNotePackageView> SearchReceiveNote(
            string employeeId,
            string patientId,
            string searchText,
            int handleStatusFilter,
            int skip,
            int take)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            DateTime currentDateTime = _timeManager.GetNow();


            var query =
                        from employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                        join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                        join messageSpecification in _notificationCenterContext.MessageSpecifications
                        .Where(x => x.ServiceType == Domain.Enum.NotificationServiceType.CommunicationNote)
                        on messageDispatchItem.ServiceCode equals messageSpecification.ServiceCode
                       
                        join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        .Where(p => p.HospitalId == hospitalId && p.TenantId == tenantId)
                        on messageDispatchItem.SenderId equals employee.Id

                        join patient in _notificationCenterContext.Query<PatientReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on messageDispatchItem.PatientId equals patient.Id into patientProjection
                        from patientPj in patientProjection.DefaultIfEmpty()

                        join encounter in _notificationCenterContext.Query<EncounterReadModel>()
                            .Where(p => p.HospitalId == hospitalId
                                && p.TenantId == tenantId
                            )
                        on messageDispatchItem.EncounterId equals encounter.Id into encounterProjection
                        from encounterPj in encounterProjection.DefaultIfEmpty()

                        join departmentForEncounter in _notificationCenterContext.Query<DepartmentReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on encounterPj.AttendingDepartmentId equals departmentForEncounter.DisplayCode into departmentProjectionForEncounter
                        from departmentPjEncounter in departmentProjectionForEncounter.DefaultIfEmpty()

                        where employeeMessageInstance.TenantId == tenantId
                          && employeeMessageInstance.HospitalId == hospitalId
                          && employeeMessageInstance.IsInbound
                          && !messageDispatchItem.IsCanceled
                          && employeeMessageInstance.EmployeeId == employeeId

                          //new 0122
                          //&& employee.HospitalId == hospitalId
                          //&& employee.TenantId == tenantId
                          && messageSpecification.TenantId == tenantId
                          && messageSpecification.HospitalId == hospitalId

                        orderby employeeMessageInstance.SentTimeStamp descending
                        select new CommunicationNoteView()
                        {
                            EmployeeId = employeeMessageInstance.EmployeeId,
                            MessageInstanceId = employeeMessageInstance.Id,

                            IsInbound = employeeMessageInstance.IsInbound,
                            MessageDispatchItemId = messageDispatchItem.Id,
                            ServiceCode = messageSpecification.ServiceCode,
                            MessageCategory = messageSpecification.MessageCategory,
                            Classification = messageSpecification.Classification,
                            MessagePriority = messageDispatchItem.MessagePriority,
                            //IsRead = employeeMessageInstance.IsReaded ? true : false,
                            IsRead = employeeMessageInstance.IsReaded,
                            ReadTime = employeeMessageInstance.ReadTime,
                            Title = messageDispatchItem.Title,
                            Content = messageDispatchItem.Content,
                            SenderId = messageDispatchItem.SenderId,
                            SenderDisplayId = messageDispatchItem.SenderId == "System" ? "" : employee.DisplayId,
                            SenderName = messageDispatchItem.SenderId == "System" ? "System" : employee.FullName,
                            OccupationName = employee.OccupationName,
                            DepartmentName = string.IsNullOrEmpty(employee.DepartmentName) ? string.Empty : employee.DepartmentName,
                            SentTimeStamp = messageDispatchItem.SentTimeStamp,
                            IsCanceled = messageDispatchItem.IsCanceled,
                            PatientInformation = new PatientInformationView()
                            {
                                PatientId = messageDispatchItem.PatientId,
                                EncounterId = messageDispatchItem.EncounterId,
                                PatientDisplayId = patientPj.PatientDisplayId,
                                PatientName = patientPj.PatientFullName,
                                GenderType = string.Equals(patientPj.GenderCode, "01", StringComparison.Ordinal) ? "M" :
                                    string.Equals(patientPj.GenderCode, "02", StringComparison.Ordinal) ? "F" : "",
                                Age = patientPj.BirthDate.Value == null ? "" :
                                EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patientPj.BirthDate.Value, currentDateTime,
                                    new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일"))),
                                DepartmentId = departmentPjEncounter.Id,
                                DepartmentName = departmentPjEncounter.Name
                            },
                            AttachmentsCount = 0
                        };

            int unreadCount = await query.Where(x => x.IsRead.Equals(false)).CountAsync().ConfigureAwait(false);
            //int unreadCount = await query.AsNoTracking().CountAsync(p => !p.IsRead).ConfigureAwait(false);

            #region ## 조건절 분기
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(p => p.Content.Contains(searchText, StringComparison.Ordinal));
            }
            if (handleStatusFilter == 0)
            {
                //isReadFilter = false;
                query = query.Where(p => p.IsRead.Equals(false));
            }
            else if (handleStatusFilter == 1)
            {
                query = query.Where(p => p.IsRead.Equals(true));
            }

            if (!string.IsNullOrEmpty(patientId))
            {
                // query = query.Where(x => x.SenderId == emplyeeId);
                query = query.Where(x => x.PatientInformation.PatientId == patientId);
            } 
            #endregion

            int totalCount = await query.AsNoTracking().CountAsync().ConfigureAwait(false);

            var communicationNoteViews = await query.AsNoTracking().Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);

            CommunicationNotePackageView communicationNotePackageView = new CommunicationNotePackageView();
            communicationNotePackageView.CommunicationNotes = communicationNoteViews;
            communicationNotePackageView.UnreadCount = unreadCount;
            communicationNotePackageView.TotalCount = totalCount;
            return communicationNotePackageView;
             
        } 
        #endregion


        public async Task<CommunicationNoteView> FindReceiveNote(string messageInstanceId)
        {

            DateTime currentDateTime = _timeManager.GetNow();
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;



            var query =
                        from employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                            .Where(x => x.Id == messageInstanceId)
                        join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                        join messageSpecification in _notificationCenterContext.MessageSpecifications
                            .Where(x => x.ServiceType == Domain.Enum.NotificationServiceType.CommunicationNote)
                        on messageDispatchItem.ServiceCode equals messageSpecification.ServiceCode

                        join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        on messageDispatchItem.SenderId equals employee.Id

                        join patient in _notificationCenterContext.Query<PatientReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on messageDispatchItem.PatientId equals patient.Id into patientProjection
                        from patientPj in patientProjection.DefaultIfEmpty()


                        join encounter in _notificationCenterContext.Query<EncounterReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on messageDispatchItem.EncounterId equals encounter.Id into encounterProjection
                        from encounterPj in encounterProjection.DefaultIfEmpty()

                        join departmentForEncounter in _notificationCenterContext.Query<DepartmentReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on encounterPj.AttendingDepartmentId equals departmentForEncounter.Id into departmentProjectionForEncounter
                        from departmentPjEncounter in departmentProjectionForEncounter.DefaultIfEmpty()

                        where employeeMessageInstance.TenantId == tenantId
                          && employeeMessageInstance.HospitalId == hospitalId
                          && employeeMessageInstance.IsInbound

                          //new 0122
                          && employee.HospitalId == hospitalId
                          && employee.TenantId == tenantId
                          && messageSpecification.TenantId == tenantId
                          && messageSpecification.HospitalId == hospitalId

                        //orderby messageDispatchItem.SentTimeStamp descending
                        orderby employeeMessageInstance.SentTimeStamp descending
                        select new CommunicationNoteView()
                        {
                            EmployeeId = employeeMessageInstance.EmployeeId,
                            MessageInstanceId = employeeMessageInstance.Id,

                            IsInbound = employeeMessageInstance.IsInbound,
                            MessageDispatchItemId = messageDispatchItem.Id,
                            ServiceCode = messageSpecification.ServiceCode,
                            MessageCategory = messageSpecification.MessageCategory,
                            Classification = messageSpecification.Classification,
                            MessagePriority = messageDispatchItem.MessagePriority,
                            IsRead = employeeMessageInstance.IsReaded,
                            ReadTime = employeeMessageInstance.ReadTime,
                            Title = messageDispatchItem.Title,
                            Content = messageDispatchItem.Content,
                            SenderId = messageDispatchItem.SenderId,
                            SenderDisplayId = messageDispatchItem.SenderId == "System" ? "" : employee.DisplayId,
                            SenderName = messageDispatchItem.SenderId == "System" ? "System" : employee.FullName,
                            OccupationName = employee.OccupationName,
                            DepartmentName = string.IsNullOrEmpty(employee.DepartmentName) ? string.Empty : employee.DepartmentName,
                            SentTimeStamp = messageDispatchItem.SentTimeStamp,
                            IsCanceled = messageDispatchItem.IsCanceled,
                            PatientInformation = new PatientInformationView()
                            {
                                PatientId = messageDispatchItem.PatientId,
                                EncounterId = messageDispatchItem.EncounterId,
                                PatientDisplayId = patientPj.PatientDisplayId,
                                PatientName = patientPj.PatientFullName,
                                GenderType = string.Equals(patientPj.GenderCode, "01", StringComparison.Ordinal) ? "M" :
                                    string.Equals(patientPj.GenderCode, "02", StringComparison.Ordinal) ? "F" : "",
                                Age = patientPj.BirthDate.Value == null ? "" :
                                EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patientPj.BirthDate.Value, currentDateTime,
                                    new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일"))),
                                DepartmentId = departmentPjEncounter.Id,
                                DepartmentName = departmentPjEncounter.Name
                            },
                            AttachmentsCount = _notificationCenterContext.MessageAttachments
                            .Where(x => x.MessageDispatchItemId == employeeMessageInstance.MessageDispatchItemId
                                && x.HospitalId == hospitalId
                                && x.TenantId == tenantId
                            ).Count()
                            //MessageAttachments = SearchMessageAttachment(employeeMessageInstance.MessageDispatchItemId).Result
                        };

            var communicationNoteView = await query.FirstOrDefaultAsync().ConfigureAwait(false);

            if (communicationNoteView != null)
            {
                communicationNoteView.MessageAttachments = await SearchMessageAttachment(communicationNoteView.MessageDispatchItemId).ConfigureAwait(false);
                communicationNoteView.CommunicationNoteRecipientViews = await SearchNoteRecipient(communicationNoteView.MessageDispatchItemId).ConfigureAwait(false);
            }


            return communicationNoteView;

        }

        #region ##### 보낸 쪽지리스트 조회
        /// <summary>
        /// 보낸 쪽지리스트 조회
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="searchText"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<CommunicationNotePackageView> SearchSentNote(string employeeId, string patientId, string searchText, int skip, int take)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            DateTime currentDateTime = _timeManager.GetNow();
            
            var query =
                    from employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                    where employeeMessageInstance.EmployeeId == employeeId
                    && !employeeMessageInstance.IsInbound
                    join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                    join messageSpecification in _notificationCenterContext.MessageSpecifications
                        .Where(p => p.ServiceType == Domain.Enum.NotificationServiceType.CommunicationNote 
                            && p.HospitalId == hospitalId 
                            && p.TenantId == tenantId
                        )
                    on messageDispatchItem.ServiceCode equals messageSpecification.ServiceCode

                    join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        .Where(p => p.TenantId == tenantId
                            && p.HospitalId == hospitalId
                        )
                    on messageDispatchItem.SenderId equals employee.Id

                    join patient in _notificationCenterContext.Query<PatientReadModel>()
                        .Where(p => p.TenantId == tenantId
                            && p.HospitalId == hospitalId
                        )
                        on messageDispatchItem.PatientId equals patient.Id into patientProjection
                    from patientPj in patientProjection.DefaultIfEmpty()

                    join encounter in _notificationCenterContext.Query<EncounterReadModel>()
                        .Where(p => p.TenantId == tenantId
                            && p.HospitalId == hospitalId
                        )
                        on messageDispatchItem.EncounterId equals encounter.Id into encounterProjection
                    from encounterPj in encounterProjection.DefaultIfEmpty()

                    join departmentForEncounter in _notificationCenterContext.Query<DepartmentReadModel>()
                        .Where(p => p.TenantId == tenantId
                            && p.HospitalId == hospitalId
                        )
                        on encounterPj.AttendingDepartmentId equals departmentForEncounter.Id into departmentProjectionForEncounter
                    from departmentPjEncounter in departmentProjectionForEncounter.DefaultIfEmpty()

                    where messageDispatchItem.TenantId == tenantId
                        && messageDispatchItem.HospitalId == hospitalId
                        && employeeMessageInstance.EmployeeId == employeeId
                        //&& messageDispatchItem.EmployeeId == employeeId
                        //&& !employeeMessageInstance.IsInbound
                        //&& (string.IsNullOrEmpty(searchText) || EF.Functions.Like(messageDispatchItem.Content, "%" + searchText + "%"))
                        && !messageDispatchItem.IsCanceled

                        
                        //new 0122
                        //&& employee.HospitalId == hospitalId
                        //&& employee.TenantId == tenantId
                        //&& messageSpecification.TenantId == tenantId
                        //&& messageSpecification.HospitalId == hospitalId
                    //orderby messageDispatchItem.SentTimeStamp descending
                    orderby employeeMessageInstance.SentTimeStamp descending
                    select new CommunicationNoteView()
                    {
                        EmployeeId = messageDispatchItem.SenderId,
                        //MessageInstanceId = messageDispatchItem.Id,
                        MessageInstanceId = employeeMessageInstance.Id,

                        //IsInbound = employeeMessageInstance.IsInbound,
                        IsInbound = true,
                        MessageDispatchItemId = messageDispatchItem.Id,
                        ServiceCode = messageSpecification.ServiceCode,
                        MessageCategory = messageSpecification.MessageCategory,
                        Classification = messageSpecification.Classification,
                        MessagePriority = messageDispatchItem.MessagePriority,
                        //IsRead = employeeMessageInstance.IsReaded,
                        //ReadTime = employeeMessageInstance.ReadTime,
                        IsRead = false,
                        ReadTime = null,
                        Title = messageDispatchItem.Title,
                        Content = messageDispatchItem.Content,
                        SenderId = messageDispatchItem.SenderId,
                        SenderDisplayId = messageDispatchItem.SenderId == "System" ? "" : employee.DisplayId,
                        SenderName = messageDispatchItem.SenderId == "System" ? "System" : employee.FullName,
                        //OccupationName = occupation.Name,
                        OccupationName = employee.OccupationName,
                        //DepartmentName = string.IsNullOrEmpty(departmentPj.Name) ? string.Empty : departmentPj.Name,
                        DepartmentName = employee.DepartmentName,
                        SentTimeStamp = messageDispatchItem.SentTimeStamp,
                        IsCanceled = messageDispatchItem.IsCanceled,
                        PatientInformation = new PatientInformationView()
                        {
                            PatientId = messageDispatchItem.PatientId,
                            EncounterId = messageDispatchItem.EncounterId,
                            PatientDisplayId = patientPj.PatientDisplayId,
                            PatientName = patientPj.PatientFullName,
                            GenderType = string.Equals(patientPj.GenderCode, "01", StringComparison.Ordinal) ? "M" :
                                    string.Equals(patientPj.GenderCode, "02", StringComparison.Ordinal) ? "F" : "",
                            Age = patientPj.BirthDate.Value == null ? "" :
                            EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patientPj.BirthDate.Value, currentDateTime,
                                new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일"))),
                            DepartmentId = departmentPjEncounter.Id,
                            DepartmentName = departmentPjEncounter.Name
                        },
                        RecipientsReadCount = _notificationCenterContext.EmployeeMessageInstances
                                                .Where
                                                (x => x.IsInbound
                                                    && x.MessageDispatchItemId == messageDispatchItem.Id
                                                    && x.IsReaded
                                                    && x.TenantId == tenantId
                                                    && x.HospitalId == hospitalId
                                                ).Count(),
                        RecipientsCount = _notificationCenterContext.EmployeeMessageInstances
                                                .Where
                                                (x => x.IsInbound
                                                    && x.MessageDispatchItemId == messageDispatchItem.Id
                                                    && x.TenantId == tenantId
                                                    && x.HospitalId == hospitalId
                                                ).Count(),
                        AttachmentsCount = _notificationCenterContext.MessageAttachments
                                                .Where
                                                (x => x.MessageDispatchItemId == messageDispatchItem.Id
                                                    && x.TenantId == tenantId
                                                    && x.HospitalId == hospitalId
                                                )
                                                .Count(),
                        //RecipientName = "TMP"
                        //RecipientName = (from message in _notificationCenterContext.EmployeeMessageInstances
                        //                 join employee in _notificationCenterContext.Employees
                        //                 on message.EmployeeId equals employee.Id
                        //                 where message.MessageDispatchItemId == messageDispatchItem.Id
                        //                 && message.IsInbound
                        //                 select employee.FullName
                        //                 ).FirstOrDefault()
                        RecipientName = (from message in _notificationCenterContext.EmployeeMessageInstances
                                         join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                                            .Where(p => p.TenantId == tenantId && p.HospitalId == hospitalId)
                                        on message.EmployeeId equals employee.Id
                                        where message.MessageDispatchItemId == messageDispatchItem.Id
                                            && message.IsInbound
                                            && message.TenantId == tenantId
                                            && message.HospitalId == hospitalId
                                        select employee.FullName
                                         ).FirstOrDefault()
                    };

            //_notificationCenterContext.Query<EmployeeReadModel>()
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(p => p.Content.Contains(searchText, StringComparison.Ordinal));
            }
            
            if (!string.IsNullOrEmpty(patientId))
            {
                query = query.Where(x => x.PatientInformation.PatientId == patientId);
            }
            int totalCount = await query.CountAsync().ConfigureAwait(false);

            //var communicationNoteViews = await query.Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);
            var communicationNoteViews = query.Skip(skip).Take(take);

            

            var communicationNoteViewsList = await communicationNoteViews.ToListAsync().ConfigureAwait(false);



            CommunicationNotePackageView communicationNotePackageView = new CommunicationNotePackageView();
            communicationNotePackageView.CommunicationNotes = communicationNoteViewsList;
            communicationNotePackageView.TotalCount = totalCount;
            return communicationNotePackageView;
        }
        #endregion


        #region #### 보낸쪽지 조회
        /// <summary>
        /// 보낸 쪽지 조회
        /// </summary>
        /// <param name="messageInstanceId"></param>
        /// <returns></returns>
        public async Task<CommunicationNoteView> FindSentNote(string messageInstanceId)
        {

            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            DateTime currentDateTime = _timeManager.GetNow();

            var query =
                        from employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                        join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                        join messageSpecification in _notificationCenterContext.MessageSpecifications.Where(x => x.ServiceType == Domain.Enum.NotificationServiceType.CommunicationNote)
                        on messageDispatchItem.ServiceCode equals messageSpecification.ServiceCode

                        join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        on messageDispatchItem.SenderId equals employee.Id
                       
                        join patient in _notificationCenterContext.Query<PatientReadModel>()
                        on messageDispatchItem.PatientId equals patient.Id into patientProjection
                        from patientPj in patientProjection.DefaultIfEmpty()
                           
                        join encounter in _notificationCenterContext.Query<EncounterReadModel>()
                        on messageDispatchItem.EncounterId equals encounter.Id into encounterProjection
                        from encounterPj in encounterProjection.DefaultIfEmpty()

                        join departmentForEncounter in _notificationCenterContext.Query<DepartmentReadModel>()
                        on encounterPj.AttendingDepartmentId equals departmentForEncounter.Id into departmentProjectionForEncounter
                        from departmentPjEncounter in departmentProjectionForEncounter.DefaultIfEmpty()

                        where employeeMessageInstance.TenantId == tenantId
                          && employeeMessageInstance.HospitalId == hospitalId
                          && employeeMessageInstance.Id == messageInstanceId

                        //new 0122
                        && employee.HospitalId == hospitalId
                        && employee.TenantId == tenantId
                        && messageSpecification.TenantId == tenantId
                        && messageSpecification.HospitalId == hospitalId

                        orderby employeeMessageInstance.SentTimeStamp descending
                        select new CommunicationNoteView()
                        {
                            EmployeeId = employeeMessageInstance.EmployeeId,
                            MessageInstanceId = employeeMessageInstance.Id,

                            IsInbound = employeeMessageInstance.IsInbound,
                            MessageDispatchItemId = messageDispatchItem.Id,
                            ServiceCode = messageSpecification.ServiceCode,
                            MessageCategory = messageSpecification.MessageCategory,
                            Classification = messageSpecification.Classification,
                            MessagePriority = messageDispatchItem.MessagePriority,
                            IsRead = employeeMessageInstance.IsReaded,
                            ReadTime = employeeMessageInstance.ReadTime,
                            Title = messageDispatchItem.Title,
                            Content = messageDispatchItem.Content,
                            SenderId = messageDispatchItem.SenderId,
                            SenderDisplayId = messageDispatchItem.SenderId == "System" ? "" : employee.DisplayId,
                            SenderName = messageDispatchItem.SenderId == "System" ? "System" : employee.FullName,
                            OccupationName = employee.OccupationName,
                            DepartmentName = string.IsNullOrEmpty(employee.DepartmentName) ? string.Empty : employee.DepartmentName,
                            SentTimeStamp = messageDispatchItem.SentTimeStamp,
                            IsCanceled = messageDispatchItem.IsCanceled,
                            PatientInformation = new PatientInformationView()
                            {
                                PatientId = messageDispatchItem.PatientId,
                                EncounterId = messageDispatchItem.EncounterId,
                                //PatientDisplayId = patientPj.DisplayId,
                                PatientDisplayId = patientPj.PatientDisplayId,
                                PatientName = patientPj.PatientFullName,
                                GenderType = string.Equals(patientPj.GenderCode, "01", StringComparison.Ordinal) ? "M" :
                                    string.Equals(patientPj.GenderCode, "02", StringComparison.Ordinal) ? "F" : "",
                                Age = patientPj.BirthDate.Value == null ? "" :
                                EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patientPj.BirthDate.Value, currentDateTime,
                                    new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일"))),
                                DepartmentId = departmentPjEncounter.Id,
                                DepartmentName = departmentPjEncounter.Name
                            },
                            RecipientsReadCount = _notificationCenterContext.EmployeeMessageInstances
                                .Where(x => x.IsInbound
                                    && x.MessageDispatchItemId == employeeMessageInstance.MessageDispatchItemId
                                    && x.IsReaded
                                    && x.TenantId == tenantId
                                    && x.HospitalId == hospitalId
                                 ).Count(),
                            RecipientsCount = _notificationCenterContext.EmployeeMessageInstances
                                .Where(x => x.IsInbound
                                        && x.MessageDispatchItemId == employeeMessageInstance.MessageDispatchItemId
                                        && x.HospitalId == hospitalId
                                        && x.TenantId == tenantId
                                 ).Count(),
                            AttachmentsCount = _notificationCenterContext.MessageAttachments
                                  .Where(x => x.MessageDispatchItemId == employeeMessageInstance.MessageDispatchItemId
                                        && x.TenantId == tenantId && x.HospitalId == hospitalId
                                  ).Count()
                        };

            var communicationNoteView = await query.FirstOrDefaultAsync().ConfigureAwait(false);

            if (communicationNoteView != null)
            {
                communicationNoteView.MessageAttachments = await SearchMessageAttachment(communicationNoteView.MessageDispatchItemId).ConfigureAwait(false);
                communicationNoteView.CommunicationNoteRecipientViews = await SearchNoteRecipient(communicationNoteView.MessageDispatchItemId).ConfigureAwait(false);
            }


            return communicationNoteView;
        }
        #endregion

        #region #### 특정 메시지의 첨부파일 리스트 반환
        /// <summary>
        /// 특정 메시지의 첨부파일 리스트 반환
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<List<MessageAttachmentView>> SearchMessageAttachment(string messageDispatchItemId)
        {
            return await _notificationCenterContext.MessageAttachments
                .Where(i => i.MessageDispatchItemId == messageDispatchItemId
                    && i.TenantId == _callContext.TenantId
                    && i.HospitalId == _callContext.HospitalId
                )
                .Select(i => new MessageAttachmentView
                {
                    MessageAttachmentId = i.Id,
                    ContentType = i.ContentType,
                    Extension = i.Extension,
                    FileKey = i.FileKey,
                    OriginalFileName = i.OriginalFileName,
                    SavedFileName = i.SavedFileName,
                    SavedFilePath = i.SavedFilePath,
                    FileSize = i.FileSize,
                    Url = i.Url
                }).ToListAsync().ConfigureAwait(false);
        }
        #endregion

        #region #### 메시지 수신자/상태 리스트
        /// <summary>
        /// 메시지 수신자/상태 리스트 
        /// - 직원과 관련된 ( ..Occupations )이 없는 경우 수신대상자에서 나오지 않음.
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<IList<CommunicationNoteRecipientView>> SearchNoteRecipient(string messageDispatchItemId)
        {

            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            var query =
                        from employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                        join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                        join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        on employeeMessageInstance.EmployeeId equals employee.Id

                        where employeeMessageInstance.TenantId == tenantId
                          && employeeMessageInstance.HospitalId == hospitalId
                          && employeeMessageInstance.MessageDispatchItemId == messageDispatchItemId
                          && employeeMessageInstance.IsInbound

                        //new 0122
                        && employee.HospitalId == hospitalId
                        && employee.TenantId == tenantId


                        orderby employeeMessageInstance.SentTimeStamp descending

                        select new CommunicationNoteRecipientView()
                        {
                            EmployeeId = employeeMessageInstance.EmployeeId,
                            EmployeeDisplayId = employee.DisplayId,
                            EmployeeName = employee.FullName ?? "No Name",
                            OccupationName = employee.FullName ?? "",

                            MessageDispatchItemId = messageDispatchItem.Id,
                            DepartmentName = employee.DepartmentName ?? "",
                            IsRead = employeeMessageInstance.IsReaded,
                            ReadTime = employeeMessageInstance.ReadTime

                        };

            var communicationNoteRecipientViews = await query.ToListAsync().ConfigureAwait(false);

            return communicationNoteRecipientViews;
        }
        #endregion


        #region #### 사번으로 직원목록 조회
        /// <summary>
        /// 사번으로 직원목록 조회
        /// </summary>
        /// <param name="employeeIds"></param>
        /// <returns></returns>
        public async Task<IList<EmployeeRecipientDto>> SearchEmployees(List<string> employeeIds)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;


            var query = from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        join contactPoint in _notificationCenterContext.Query<ContactPointReadModel>()
                            .Where(p => p.TenantId == tenantId 
                                && p.SystemType == ContactPointSystem.Mobile
                            )
                        on employee.PersonId equals contactPoint.PersonId into contactPointProjection
                        from contactPointPj in contactPointProjection.DefaultIfEmpty()
                         where employee.TenantId == tenantId
                         //&& contactPointPj.TenantId == tenantId
                         //&& contactPointPj.SystemType == ContactPointSystem.Mobile
                         && employee.HospitalId == hospitalId
                         && employeeIds.Contains(employee.Id)
                         select new EmployeeRecipientDto()
                         {
                             EmployeeId = employee.Id,
                             EmployeeDisplayId = employee.DisplayId,
                             EmployeeName = employee.FullName,
                             OccupationId = employee.OccupationCode,
                             OccupationName = employee.OccupationName,
                             DepartmentId = employee.DepartmentId,
                             DepartmentName = employee.DepartmentName,
                             Mobile = contactPointPj.ContactValue ?? ""
                         };

            var recipients = await query.ToListAsync().ConfigureAwait(false);

            return recipients;
        } 
        #endregion

    }
}
