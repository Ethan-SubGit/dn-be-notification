using CHIS.Framework.Data.ORM;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.Framework.Core.Localization;
using CHIS.Framework.Core.Claims;
using CHIS.Framework.Core;
using System;
using CHIS.Share.MedicalAge;
using CHIS.NotificationCenter.Application.Models.QueryType;

namespace CHIS.NotificationCenter.Infrastructure.Queries
{
    public class EmployeeMessageBoxQueries : DALBase, IEmployeeMessageBoxQueries
    {
        private readonly ICallContext _callContext;
        private readonly ITimeManager _timeManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly NotificationCenterContext _notificationCenterContext;

        public EmployeeMessageBoxQueries(ICallContext callContext
            , ITimeManager timeManager
            , ILocalizationManager localizationManager
            , NotificationCenterContext notificationCenterContext) : base(callContext)
        {
            this.DBCatalog = NotificationCenterContext.DOMAIN_NAME;
            _notificationCenterContext = notificationCenterContext;
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            this._localizationManager = localizationManager;
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
        }


        #region ### message count조회
        /// <summary>
        /// 메시지 카운트 조회
        /// Discarded => Replaced to RetrieveMessageCountByEmployeeV2 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="searchText"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public async Task<IList<MessageCountView>> RetrieveMessageCountByEmployee(string employeeId, string searchText, string patientId)
        {
            IList<MessageCountView> messageCountView;
            string queryName = string.Empty;

            object parameter = new
            {
                TenantId = _callContext.TenantId
                ,
                HospitalId = _callContext.HospitalId
                ,
                EmployeeId = employeeId
                ,
                SearchText = searchText
                ,
                PatientId = patientId
            };
            queryName = "RetrieveMessageCount";

            using (var connection = db.CreateConnection())
            {
                messageCountView = (await connection.SqlTranslateQueryAsync<MessageCountView>(queryName
                                    , param: parameter).ConfigureAwait(false)).ToList();
            }

            return messageCountView;
        }
        /// <summary>
        /// 카운트 Grand TotalSummary 조회 (알림벨에서 사용)
        /// </summary>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        public async Task<IList<MessageCountView>> SearchGrandTotalMessageCount(string employeeId)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;


            IList<MessageCountView> messageCountView;
            string queryName = "SearchGrandTotalMessageCount";
            object parameter = new
            {
                TenantId = tenantId,
                HospitalId = hospitalId,
                EmployeeId = employeeId
            };
            using (var connection = db.CreateConnection())
            {
                messageCountView = (await connection.SqlTranslateQueryAsync<MessageCountView>(queryName
                                    , param: parameter).ConfigureAwait(false)).ToList();
            }

            return messageCountView;
        }
        /// <summary>
        /// 카운트 Summary 조회
        /// </summary>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        public async Task<IList<MessageCountView>> SearchMessageCount(
            string employeeId,
            //string messageCategory,
            string patientId,
            string searchText,
            string periodFilter,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            List<string> filterByServiceCodes,
            string departmentId
            )
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            string inClauseFilterByServiceCodes = BuildInValuesOfWhereClause(filterByServiceCodes);


            object parameterForCount = new
            {

                TenantId = CheckNullString(tenantId),
                HospitalId = CheckNullString(hospitalId),
                EmployeeId = CheckNullString(employeeId),
                PatientId = CheckNullString(patientId),
                SearchText = CheckNullString(searchText),
                FromDateTime = GetFromDateTime(fromDateTime, periodFilter),
                ToDateTime = GetToDateTime(toDateTime, periodFilter),
                InClauseFilterByServiceCodes = inClauseFilterByServiceCodes,//(담당환자만 필터링)
                DepartmentId = CheckNullString(departmentId)
            };

            IList<MessageCountView> messageCountView;
            string queryName = "SearchMessageCount";

            using (var connection = db.CreateConnection())
            {
                messageCountView = (await connection.SqlTranslateQueryAsync<MessageCountView>(queryName
                                    , param: parameterForCount).ConfigureAwait(false)).ToList();
            }

            return messageCountView;
        } 
        #endregion

        /// <summary>
        /// Inbox Message Instance List 조회 (검색조건 포함)
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="messageCategory"></param>
        /// <param name="patientId"></param>
        /// <param name="searchText"></param>
        /// <param name="handleOption"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="exclusionMessageInstanceId"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="serviceCodes"></param>
        /// <returns></returns>
        public async Task<EmployeeMessagePackageView> SearchMessages(
            string employeeId, 
            string messageCategory, 
            string patientId, 
            string searchText, 
            string periodFilter,
            int handleStatusFilter, 
            string exclusionMessageInstanceId, 
            DateTime? fromDateTime, 
            DateTime? toDateTime ,
            List<string> filterByServiceCodes,
            string departmentId,
            int skip,
            int take)
        {


        IList<EmployeeMessageView> employeeMessageView = null;
            string queryName = "SearchMessages";
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            Nullable<bool> IsHandledFilter = null;

            DateTime currentDateTimeLocal = _timeManager.GetNow();

            string inClauseFilterByServiceCodes = BuildInValuesOfWhereClause(filterByServiceCodes);

            if (handleStatusFilter == 0)
            {
                IsHandledFilter = false;
            }
            else if (handleStatusFilter == 1)
            {
                IsHandledFilter = true;
            }

            object parameter = new
            {
                TenantId = CheckNullString(tenantId) ,
                HospitalId = CheckNullString(hospitalId) ,
                EmployeeId = CheckNullString(employeeId) ,
                IsHandled = IsHandledFilter ,
                MessageCategory = CheckNullString(messageCategory) ,
                PatientId = CheckNullString(patientId) ,
                SearchText = CheckNullString(searchText) ,
                Skip = skip ,
                Take = take ,   
                ExclusionMessageInstanceId = CheckNullString(exclusionMessageInstanceId),
                FromDateTime = GetFromDateTime(fromDateTime, periodFilter),
                ToDateTime = GetToDateTime(toDateTime, periodFilter),
                InClauseFilterByServiceCodes = inClauseFilterByServiceCodes ,//(담당환자만 필터링)
                DepartmentId = CheckNullString(departmentId)
               
            };

            using (var connection = db.CreateConnection())
            {
                employeeMessageView = (await connection.SqlTranslateQueryAsync<EmployeeMessageView, PatientInformationView, EmployeeMessageView>(queryName
                    ,
                    (EmployeeMessageView, PatientInformationView) =>
                    {
                        EmployeeMessageView.PatientInformation = PatientInformationView;
                        return EmployeeMessageView;
                    }, param: parameter, splitOn: "PatientId").ConfigureAwait(false)).ToList();
            }

            // DESC : 여기서 Total Record Count 는 검색조건에 해당하는 Total Record Count 임. / 검색조건과 무관한 Total Record Count도 필요함.
            IList<MessageCountView> messageCountView = await SearchMessageCount(
                employeeId ,
                patientId ,
                searchText,
                periodFilter ,
                fromDateTime,
                toDateTime,
                filterByServiceCodes,
                departmentId).ConfigureAwait(false);

            Dictionary<string, List<MessageAttachmentView>> messageAttachmentGroup = 
                await RetrieveMessageAttachments(employeeMessageView
                    .Select(i => i.MessageDispatchItemId).ToList()).ConfigureAwait(false);

            foreach (var mainRow in employeeMessageView)
            {
                if (!messageAttachmentGroup.ContainsKey(mainRow.MessageDispatchItemId))
                {
                    continue;
                }

                mainRow.MessageAttachments = messageAttachmentGroup[mainRow.MessageDispatchItemId] ?? new List<MessageAttachmentView>();
            }

            // DESC : 나이 출력 오류로 나이계산 공통모듈 사용하여 조정함.
            foreach (EmployeeMessageView employeeMessage in employeeMessageView)
            {
                if (employeeMessage.PatientInformation == null || employeeMessage.PatientInformation.BirthDay == null)
                {
                    continue;
                }
                employeeMessage.PatientInformation.Age = 
                    EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, employeeMessage.PatientInformation.BirthDay.Value, currentDateTimeLocal
                    , new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일")));


            }

            EmployeeMessagePackageView employeeMessagePackageView = new EmployeeMessagePackageView();
            employeeMessagePackageView.EmployeeMessages = employeeMessageView;
            employeeMessagePackageView.MessageCounts = messageCountView;

            return employeeMessagePackageView;
        }


        public async Task<EmployeeMessageView> FindMessage(
            string messageInstanceId)
        {

            DateTime currentDateTimeLocal = _timeManager.GetNow();
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;


            var query = from employeeMessageBox in _notificationCenterContext.EmployeeMessageBoxes
                        join employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                        .Where(x => x.Id == messageInstanceId && x.HospitalId == hospitalId && x.TenantId == tenantId)
                        on employeeMessageBox.Id equals employeeMessageInstance.EmployeeMessageBoxId

                        join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                        join messageSpecification in _notificationCenterContext.MessageSpecifications
                        on messageDispatchItem.ServiceCode equals messageSpecification.ServiceCode

                        join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on messageDispatchItem.SenderId equals employee.Id into senderProjection
                        from senderPj in senderProjection.DefaultIfEmpty()

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
                        on encounterPj.AttendingDepartmentId equals departmentForEncounter.Id into departmentProjectionForEncounter
                        from departmentPjEncounter in departmentProjectionForEncounter.DefaultIfEmpty()

                        join departmentForEncounterWard in _notificationCenterContext.Query<DepartmentReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on encounterPj.OccupyingWardId equals departmentForEncounterWard.Id into departmentProjectionForEncounterWard
                        from departmentPjEncounterWard in departmentProjectionForEncounterWard.DefaultIfEmpty()

                        join locationRoom in _notificationCenterContext.Query<LocationRoomReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on encounterPj.OccupyingRoomId equals locationRoom.Id into locationRoomProjection
                        from locationRoomPj in locationRoomProjection.DefaultIfEmpty()

                        where messageDispatchItem.TenantId == tenantId
                          && messageDispatchItem.HospitalId == hospitalId
                          && messageSpecification.ServiceType == 0

                        //new 0122
                        //&& senderPj.HospitalId == hospitalId
                        //&& senderPj.TenantId == tenantId
                        && messageSpecification.TenantId == tenantId
                        && messageSpecification.HospitalId == hospitalId

                        orderby messageDispatchItem.SentTimeStamp descending
                        select new EmployeeMessageView()
                        {
                            EmployeeId = employeeMessageBox.EmployeeId,
                            MessageInstanceId = employeeMessageInstance.Id,
                            HandleTime = employeeMessageInstance.HandleTime,
                            IsHandled = employeeMessageInstance.IsHandled,
                            IsInbound = employeeMessageInstance.IsInbound,
                            MessageDispatchItemId = messageDispatchItem.Id,
                            ServiceCode = messageSpecification.ServiceCode,
                            MessageCategory = messageSpecification.MessageCategory,
                            Classification = messageSpecification.Classification,
                            MessagePriority = messageDispatchItem.MessagePriority,
                            IsReaded = employeeMessageInstance.IsReaded,
                            ReadTime = employeeMessageInstance.ReadTime,
                            Title = messageDispatchItem.Title,
                            Content = messageDispatchItem.Content,
                            SenderId = messageDispatchItem.SenderId,
                            SenderName = messageDispatchItem.SenderId == "System" ? "System" : senderPj.FullName,
                            IntegrationType = messageDispatchItem.IntegrationType,
                            IntegrationAddress = messageDispatchItem.IntegrationAddress,
                            IntegrationParameter = messageDispatchItem.IntegrationParameter,
                            SentTimeStamp = messageDispatchItem.SentTimeStamp,
                            IsSelectPatientByActiveEncounter = messageSpecification.IsSelectPatientByActiveEncounter,
                            PostActionType = messageSpecification.PostActionType,
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
                                EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patientPj.BirthDate.Value, currentDateTimeLocal,
                                    new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일"))),
                                DepartmentId = departmentPjEncounter.Id,
                                DepartmentName = departmentPjEncounter.Name,
                                WardCodeName = departmentPjEncounterWard.DisplayCode,
                                RoomCodeName = locationRoomPj.DisplayCode
                            }

                        };

            var employeeMessageView = await query.AsNoTracking().FirstOrDefaultAsync().ConfigureAwait(false);

           List<MessageAttachmentView> messageAttachmentGroup = await RetrieveMessageAttachment(employeeMessageView.MessageDispatchItemId).ConfigureAwait(false);
            employeeMessageView.MessageAttachments = messageAttachmentGroup;

            return employeeMessageView;

        }

        private string CheckNullString(string parameter)
        {
            string returnValue = string.Empty;
            if (string.IsNullOrEmpty(parameter))
            {
                returnValue = null;
            }
            else { returnValue = parameter; }

            return returnValue;
        }

        private string BuildInValuesOfWhereClause(List<string> parameters)
        {
            string inClauseConditionValues= string.Empty;

            if (parameters != null && parameters.Count > 0)
            {
                inClauseConditionValues = "'" + string.Join("','", parameters) + "'";
            }
            else
            {
                inClauseConditionValues = null;
            }
            return inClauseConditionValues;
        }

        #region Class Util

        public DateTime? GetFromDateTime(DateTime? parameter, string periodFilter)
        {

            //string timeZoneId = _callContext.TimeZoneId;
            string timeZoneId = _timeManager.GetTimeZone().TimeZoneId;
            DateTime currentDateTimeLocal = _timeManager.GetNow(timeZoneId);

            DateTime? returnDate = null;

            if (!string.IsNullOrEmpty(periodFilter))
            {
                //1주전,2주전,한달전,6개월전,1년전,기간검색 {1W,2W,1M,6M,1Y,RG(Range),NA(없음)}
                switch (periodFilter)
                {
                    case "1W":
                        returnDate = currentDateTimeLocal.AddDays(-7);

                        break;
                    case "2W":
                        returnDate = currentDateTimeLocal.AddDays(-14);

                        break;
                    case "1M":
                        returnDate = currentDateTimeLocal.AddMonths(-1);

                        break;
                    case "6M":
                        returnDate = currentDateTimeLocal.AddMonths(-6);

                        break;
                    case "1Y":
                        returnDate = currentDateTimeLocal.AddYears(-1);

                        break;
                    case "RG":
                        break;
                    default:
                        break;
                }

            }
            else
            {
                returnDate = null;

            }

            return returnDate;

        }
        public DateTime? GetToDateTime(DateTime? parameter, string periodFilter)
        {

            //string timeZoneId = _callContext.TimeZoneId;
            string timeZoneId = _timeManager.GetTimeZone().TimeZoneId;
            DateTime currentDateTimeLocal = _timeManager.GetNow(timeZoneId);

            DateTime? returnDate = null;

            if (!string.IsNullOrEmpty(periodFilter))
            {
                //1주전,2주전,한달전,6개월전,1년전,기간검색 {1W,2W,1M,6M,1Y,RG(Range),NA(없음)}
                switch (periodFilter)
                {
                    case "1W":
                    case "2W":
                    case "1M":
                    case "6M":
                    case "1Y":
                        returnDate = currentDateTimeLocal;

                        break;
                    case "RG":
                        break;
                    default:
                        break;
                }

            }
            else
            {
                returnDate = null;

            }

            return returnDate;

        }

        #endregion

        /// <summary>
        /// 인박스 환자별 보기 조회 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="messageCategory"></param>
        /// <param name="patientId"></param>
        /// <param name="searchText"></param>
        /// <param name="periodFilter"></param>
        /// <param name="handleStatusFilter"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="fromDateTime"></param>
        /// <param name="toDateTime"></param>
        /// <param name="filterByServiceCodes"></param>
        /// <param name="isFilterByPatientInCharge"></param>
        /// <returns></returns>
        public async Task<EmployeePatientMessagePackageView> SearchPatientMessages(
           //string employeeId, string searchText, int filterOption, int skip, int take)
           string employeeId,
            //string messageCategory,
            string patientId,
            string searchText,
            string periodFilter,
            int handleStatusFilter,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            List<string> filterByServiceCodes,
            string departmentId,
            int skip,
            int take)
        {
            EmployeePatientMessagePackageView PackageView = new EmployeePatientMessagePackageView();

            string queryName = "SearchPatientMessages";
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            //string timeZoneId = _callContext.TimeZoneId;
            DateTime currentDateTimeLocal = _timeManager.GetNow();
            string messagePriority = null;

            string inClauseFilterByServiceCodes = BuildInValuesOfWhereClause(filterByServiceCodes);



            Nullable<bool> IsHandledFilter = null;
            if (handleStatusFilter == 0)
            {
                IsHandledFilter = false;
            }
            else if (handleStatusFilter == 1)
            {
                IsHandledFilter = true;
            }

            object parameter = new
            {

                TenantId = CheckNullString(tenantId),
                HospitalId = CheckNullString(hospitalId),
                EmployeeId = CheckNullString(employeeId),
                IsHandled = IsHandledFilter,
                //MessageCategory = messageCategory,
                PatientId = CheckNullString(patientId),
                SearchText = CheckNullString(searchText),
                Skip = skip,
                Take = take,
                //ExclusionMessageInstanceId = exclusionMessageInstanceId,
                FromDateTime = GetFromDateTime(fromDateTime, periodFilter),
                ToDateTime = GetToDateTime(toDateTime , periodFilter),
                InClauseFilterByServiceCodes = inClauseFilterByServiceCodes ,//(ServiceCode 필터링)
                DepartmentId = CheckNullString(departmentId),
                MessagePriority = messagePriority
                //IsFilterByPatientInCharge = isFilterByPatientInCharge,
            };

            object parameterForCount = new
            {

                TenantId = CheckNullString(tenantId),
                HospitalId = CheckNullString(hospitalId),
                EmployeeId = CheckNullString(employeeId),
                IsHandled = IsHandledFilter,
                PatientId = CheckNullString(patientId),
                SearchText = CheckNullString(searchText),
                FromDateTime = GetFromDateTime(fromDateTime, periodFilter),
                ToDateTime = GetToDateTime(toDateTime, periodFilter),
                InClauseFilterByServiceCodes = inClauseFilterByServiceCodes,//(ServiceCode 필터링)
                DepartmentId = CheckNullString(departmentId),
                MessagePriority = messagePriority

            };
            using (var connection = db.CreateConnection())
            {
                PackageView.EmployeePatientMessages = (await connection.SqlTranslateQueryAsync<EmployeePatientMessageView, PatientInformationView, EmployeePatientMessageView>(queryName
                    ,
                    (EmployeeMessageView, PatientInformationView) =>
                    {
                        EmployeeMessageView.PatientInformation = PatientInformationView;
                        return EmployeeMessageView;
                    }, param: parameter, splitOn: "PatientId").ConfigureAwait(false)).ToList();

                // DESC : 여기서 Total Record Count 는 검색조건에 해당하는 Total Record Count 임. / 검색조건과 무관한 Total Record Count도 필요함.
                //위와 동일한 쿼리를 Skip과 Take만 제거하여 다시 돌려서 토탈 카운트 추출
                PackageView.TotalRecordCount = (await connection.SqlTranslateQueryAsync<EmployeePatientMessageView, PatientInformationView, EmployeePatientMessageView>(queryName
                    ,
                    (EmployeeMessageView, PatientInformationView) =>
                    {
                        EmployeeMessageView.PatientInformation = PatientInformationView;
                        return EmployeeMessageView;
                    }
                    , param: parameterForCount
                    , splitOn: "PatientId"
                    ).ConfigureAwait(false)).ToList().Count;
            }

            foreach (EmployeePatientMessageView employeeMessage in PackageView.EmployeePatientMessages)
            {
                employeeMessage.PatientInformation.Age = 
                    employeeMessage.PatientInformation.BirthDay  ==  null ? "" : 
                    EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, employeeMessage.PatientInformation.BirthDay.Value, currentDateTimeLocal
                    , new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일")));
            }
            PackageView.MessageCounts = await RetrieveMessageCountByEmployee(employeeId, searchText, null).ConfigureAwait(false);

            return PackageView;
        }

        #region ######### OUTBOX ###########
        /// <summary>
        /// 카운트 Summary 조회
        /// </summary>
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        public async Task<IList<MessageCountView>> SearchOutboxMessageCount(
            string employeeId,
            //string messageCategory,
            string patientId,
            string searchText,
            string periodFilter,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            List<string> filterByServiceCodes,
            string departmentId
            )
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            //string timeZoneId = _callContext.TimeZoneId;
            //DateTime currentDateTimeLocal = _timeManager.GetNow();


            string inClauseFilterByServiceCodes = BuildInValuesOfWhereClause(filterByServiceCodes);


            object parameterForCount = new
            {

                TenantId = CheckNullString(tenantId),
                HospitalId = CheckNullString(hospitalId),
                EmployeeId = CheckNullString(employeeId),
                PatientId = CheckNullString(patientId),
                SearchText = CheckNullString(searchText),
                FromDateTime = GetFromDateTime(fromDateTime, periodFilter),
                ToDateTime = GetToDateTime(toDateTime, periodFilter),
                InClauseFilterByServiceCodes = inClauseFilterByServiceCodes,//(담당환자만 필터링)
                DepartmentId = CheckNullString(departmentId)
            };

            IList<MessageCountView> messageCountView;
            string queryName = "SearchOutboxMessageCount";

            using (var connection = db.CreateConnection())
            {
                messageCountView = (await connection.SqlTranslateQueryAsync<MessageCountView>(queryName
                                    , param: parameterForCount).ConfigureAwait(false)).ToList();
            }

            return messageCountView;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageInstanceId"></param>
        /// <returns></returns>
        public async Task<EmployeeMessageView> FindOutboxMessage(string messageDispatchItemId)
        {

            //string TimeZoneId = _callContext.TimeZoneId;
            DateTime currentDateTimeLocal = _timeManager.GetNow();
            //DateTime currentDateTimeUTC = _timeManager.ConvertToUTC(TimeZoneId, currentDateTimeLocal);
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;



            var query = from employeeMessageBox in _notificationCenterContext.EmployeeMessageBoxes
                        join employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances.Where(x => x.MessageDispatchItemId == messageDispatchItemId && x.TenantId == tenantId
                            && x.HospitalId == hospitalId
                        )
                        on employeeMessageBox.Id equals employeeMessageInstance.EmployeeMessageBoxId

                        join messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        on employeeMessageInstance.MessageDispatchItemId equals messageDispatchItem.Id

                        join messageSpecification in _notificationCenterContext.MessageSpecifications
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
                            .Where(p => p.HospitalId == hospitalId
                                && p.TenantId == tenantId
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
                          && messageSpecification.ServiceType == 0

                        //new 0122
                        && employee.HospitalId == hospitalId
                        && employee.TenantId == tenantId
                        && messageSpecification.TenantId == tenantId
                        && messageSpecification.HospitalId == hospitalId

                        orderby messageDispatchItem.SentTimeStamp descending
                        select new EmployeeMessageView()
                        {
                            EmployeeId = employeeMessageBox.EmployeeId,
                            MessageInstanceId = employeeMessageInstance.Id,
                            HandleTime = employeeMessageInstance.HandleTime,
                            IsHandled = employeeMessageInstance.IsHandled,
                            IsInbound = employeeMessageInstance.IsInbound,
                            MessageDispatchItemId = messageDispatchItem.Id,
                            ServiceCode = messageSpecification.ServiceCode,
                            MessageCategory = messageSpecification.MessageCategory,
                            Classification = messageSpecification.Classification,
                            MessagePriority = messageDispatchItem.MessagePriority,
                            IsReaded = employeeMessageInstance.IsReaded,
                            ReadTime = employeeMessageInstance.ReadTime,
                            Title = messageDispatchItem.Title,
                            Content = messageDispatchItem.Content,
                            SenderId = messageDispatchItem.SenderId,
                            SenderName = messageDispatchItem.SenderId == "System" ? "System" : employee.FullName,
                            IntegrationType = messageDispatchItem.IntegrationType,
                            IntegrationAddress = messageDispatchItem.IntegrationAddress,
                            IntegrationParameter = messageDispatchItem.IntegrationParameter,
                            SentTimeStamp = messageDispatchItem.SentTimeStamp,

                            PostActionType = messageSpecification.PostActionType,
                            IsSelectPatientByActiveEncounter = messageSpecification.IsSelectPatientByActiveEncounter,
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
                                EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patientPj.BirthDate.Value, currentDateTimeLocal,
                                    new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일"))),
                                DepartmentId = departmentPjEncounter.Id,
                                DepartmentName = departmentPjEncounter.Name
                            }

                        };

            var employeeMessageView = await query.FirstOrDefaultAsync().ConfigureAwait(false);

            return employeeMessageView;

        }


        /// <summary>
        /// SentBox 메시지 조회
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="messageCategory"></param>
        /// <param name="searchText"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<EmployeeMessagePackageView> SearchOutboxMessages(
            string employeeId,
            string messageCategory,
            string patientId,
            string searchText,
            string periodFilter,
            string exclusionMessageInstanceId,
            DateTime? fromDateTime,
            DateTime? toDateTime,
            List<string> filterByServiceCodes,
            string departmentId,
            int skip,
            int take)
        {
            IList<EmployeeMessageView> employeeMessageView = null;
            string queryName = "SearchOutboxMessages";
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            //string timeZoneId = _callContext.TimeZoneId;
            //Nullable<bool> IsHandledFilter = null;

            DateTime currentDateTimeLocal = _timeManager.GetNow();

            string inClauseFilterByServiceCodes = BuildInValuesOfWhereClause(filterByServiceCodes);

            object parameter = new
            {
                TenantId = CheckNullString(tenantId),
                HospitalId = CheckNullString(hospitalId),
                EmployeeId = CheckNullString(employeeId),
                MessageCategory = CheckNullString(messageCategory),
                PatientId = CheckNullString(patientId),
                SearchText = CheckNullString(searchText),
                IsCanceled = false,
                Skip = skip,
                Take = take,
                ExclusionMessageInstanceId = CheckNullString(exclusionMessageInstanceId),
                FromDateTime = GetFromDateTime(fromDateTime, periodFilter),
                ToDateTime = GetToDateTime(toDateTime, periodFilter),
                InClauseFilterByServiceCodes = inClauseFilterByServiceCodes,//(담당환자만 필터링)
                DepartmentId = CheckNullString(departmentId)

            };


            using (var connection = db.CreateConnection())
            {
                employeeMessageView = (await connection.SqlTranslateQueryAsync<EmployeeMessageView, PatientInformationView, EmployeeMessageView>(queryName
                    ,
                    (EmployeeMessageView, PatientInformationView) =>
                    {
                        EmployeeMessageView.PatientInformation = PatientInformationView;
                        return EmployeeMessageView;
                    }, param: parameter, splitOn: "PatientId").ConfigureAwait(false)).ToList();
            }

            // DESC : 여기서 Total Record Count 는 검색조건에 해당하는 Total Record Count 임. / 검색조건과 무관한 Total Record Count도 필요함.
            IList<MessageCountView> messageCountView = await SearchOutboxMessageCount(
                employeeId,
                patientId,
                searchText,
                periodFilter,
                fromDateTime,
                toDateTime,
                filterByServiceCodes,
                departmentId).ConfigureAwait(false);

            Dictionary<string, List<MessageAttachmentView>> messageAttachmentGroup =
                await RetrieveMessageAttachments(employeeMessageView.Select(i => i.MessageDispatchItemId).ToList()).ConfigureAwait(false);

            foreach (var mainRow in employeeMessageView)
            {
                if (!messageAttachmentGroup.ContainsKey(mainRow.MessageDispatchItemId))
                {
                    continue;
                }

                mainRow.MessageAttachments = messageAttachmentGroup[mainRow.MessageDispatchItemId] ?? new List<MessageAttachmentView>();
            }

            // DESC : 나이 출력 오류로 나이계산 공통모듈 사용하여 조정함.
            foreach (EmployeeMessageView employeeMessage in employeeMessageView)
            {
                employeeMessage.PatientInformation.Age =
                    EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, employeeMessage.PatientInformation.BirthDay.Value, currentDateTimeLocal,
                        new UnitMark("", _localizationManager.GetFullName("9647", "개월"), _localizationManager.GetFullName("6134", "일")));
            }

            EmployeeMessagePackageView employeeMessagePackageView = new EmployeeMessagePackageView();
            employeeMessagePackageView.EmployeeMessages = employeeMessageView;
            employeeMessagePackageView.MessageCounts = messageCountView;

            return employeeMessagePackageView;

        }

        #endregion

        /// <summary>
        /// 첨부파일 목록 조회. GroupBy 부분에 의해 client evaluation 발생
        /// </summary>
        /// <param name="messageDispatchItemIdList"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, List<MessageAttachmentView>>> RetrieveMessageAttachments(List<string> messageDispatchItemIdList)
        {
            

            var attachments = await _notificationCenterContext.MessageAttachments
                .Where(i => messageDispatchItemIdList.Contains(i.MessageDispatchItemId)
                    && i.TenantId == _callContext.TenantId
                    && i.HospitalId == _callContext.HospitalId
                 )
                .ToListAsync()
                .ConfigureAwait(false);

            return attachments.GroupBy(i => i.MessageDispatchItemId).ToDictionary(g => g.Key, g => g.Select(i => new MessageAttachmentView
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
            }).ToList());

        }

        /// <summary>
        /// 특정 메시지의 첨부파일 리스트 반환
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<List<MessageAttachmentView>> RetrieveMessageAttachment(string messageDispatchItemId)
        {
            return await _notificationCenterContext.MessageAttachments
                .Where(i => i.MessageDispatchItemId == messageDispatchItemId
                    && i.TenantId == _callContext.TenantId
                    && i.HospitalId == _callContext.HospitalId)
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
        
        /// <summary>
        /// 메시지 수신자/상태 리스트 
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<IList<RecipientMessageView>> RetrieveMessagesRecipients(string messageDispatchItemId)
        {

            string queryName = "RetrieveMessagesRecipients";
            object parameter =
                new
                {
                    TenantId = _callContext.TenantId,
                    HospitalId = _callContext.HospitalId,
                    MessageDispatchItemId = messageDispatchItemId
                };

            using (var connection = db.CreateConnection())
            {
                IList<RecipientMessageView> employeeMessageView = (await connection.SqlTranslateQueryAsync<RecipientMessageView>(queryName, parameter).ConfigureAwait(false)).ToList();
                return employeeMessageView;
            }
        }

        /// <summary>
        /// Exam domain 의 전송메시지 수신상태 확인 서비스
        /// </summary>
        /// <param name="messageDispatchItemId"></param>
        /// <returns></returns>
        public async Task<MessageRecipientStatusView> RetrieveMessageRecipientStatuses(string messageDispatchItemId)
        {
            //input - serviceCode, inboxitemid
            //output - inboxitemid, encounterId, 전송내용, 발신자, 발신시간, 수신자, 수신시간, 확인시간
            var recipients = from messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                             join employeeMessageInstance in _notificationCenterContext.EmployeeMessageInstances
                                .Where(p => p.TenantId == _callContext.TenantId 
                                    && p.HospitalId == _callContext.HospitalId
                                    && p.IsInbound
                                 )
                             on messageDispatchItem.Id equals employeeMessageInstance.MessageDispatchItemId into employeeMessageInstanceProjection
                             from emip in employeeMessageInstanceProjection.DefaultIfEmpty()

                             join employeeMessageBox in _notificationCenterContext.EmployeeMessageBoxes
                             on emip.EmployeeMessageBoxId equals employeeMessageBox.Id

                             join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                                .Where(p => p.TenantId == _callContext.TenantId
                                    && p.HospitalId == _callContext.HospitalId
                                )
                             on employeeMessageBox.EmployeeId equals employee.Id

                             where messageDispatchItem.Id == messageDispatchItemId
                                //&& emip.IsInbound
                                //&& emip.TenantId == _callContext.TenantId
                                //&& emip.HospitalId == _callContext.HospitalId

                                //new 0122
                                && messageDispatchItem.HospitalId == _callContext.HospitalId
                                && messageDispatchItem.TenantId == _callContext.TenantId
                                && employeeMessageBox.TenantId == _callContext.TenantId
                                && employeeMessageBox.HospitalId == _callContext.HospitalId

                             select new RecipientMessageView()
                             {
                                 MessageDispatchItemId = emip.MessageDispatchItemId,
                                 MessageInstanceId = emip.Id,
                                 IsHandled = emip.IsHandled,
                                 HandleTime = emip.HandleTime,
                                 IsReaded = emip.IsReaded,
                                 ReadTime = emip.ReadTime,
                                 EmployeeId = employeeMessageBox.EmployeeId,
                                 EmployeeName = employee.FullName
                             };

            var recipientList = await recipients.ToListAsync().ConfigureAwait(false);

            var message = from messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                          join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                          on messageDispatchItem.SenderId equals employee.Id
                          where messageDispatchItem.Id == messageDispatchItemId
                                && messageDispatchItem.TenantId == _callContext.TenantId
                                && messageDispatchItem.HospitalId == _callContext.HospitalId
                                && employee.TenantId == _callContext.TenantId
                                && employee.HospitalId == _callContext.HospitalId
                          select new MessageRecipientStatusView()
                          {
                              MessageDispatchItemId = messageDispatchItem.Id,
                              PatientId = messageDispatchItem.PatientId,
                              EncounterId = messageDispatchItem.EncounterId,
                              Title = messageDispatchItem.Title,
                              Content = messageDispatchItem.Content,
                              SenderId = messageDispatchItem.SenderId,
                              SenderName = employee.FullName,
                              SentDateTime = messageDispatchItem.SentTimeStamp,
                              RecipientMessageViews = recipientList
                          };


            return await message.FirstOrDefaultAsync().ConfigureAwait(false);

        }

        public async Task<EmployeeMessageCategory> RetrieveMessageCategory(string employeeMessageInstanceId)
        {


            var message = from messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                          join messageInstance in _notificationCenterContext.EmployeeMessageInstances
                          on messageDispatchItem.Id  equals messageInstance.MessageDispatchItemId
                          join messageSpecification in _notificationCenterContext.MessageSpecifications
                          on messageDispatchItem.ServiceCode equals messageSpecification.ServiceCode
                          where  messageInstance.Id == employeeMessageInstanceId
                                && messageInstance.TenantId == _callContext.TenantId
                                && messageInstance.HospitalId == _callContext.HospitalId

                                //new 0122
                                && messageDispatchItem.HospitalId == _callContext.HospitalId
                                && messageDispatchItem.TenantId == _callContext.TenantId
                                && messageSpecification.TenantId == _callContext.TenantId
                                && messageSpecification.HospitalId == _callContext.HospitalId
                          select new EmployeeMessageCategory
                          {
                              ServiceCode = messageSpecification.ServiceCode ,
                              MessageCategory = messageSpecification.MessageCategory                              
                          };


            return await message.FirstOrDefaultAsync().ConfigureAwait(false);

        }

        public async Task<string> RetrieveMessageDispatchItemIdWithReference(string referenceId, string serviceCode)
        {
            var query = from messageDispatchItem in _notificationCenterContext.MessageDispatchItems
                        where messageDispatchItem.ReferenceId == referenceId
                            && messageDispatchItem.ServiceCode == serviceCode
                            && messageDispatchItem.HospitalId == _callContext.HospitalId
                            && messageDispatchItem.TenantId == _callContext.TenantId
                        select messageDispatchItem.Id ?? "";
            var returnString = "";
            try
            {
                
                returnString = (await query.FirstOrDefaultAsync().ConfigureAwait(false)).ToString();
            }
            catch (Exception)
            {
                returnString = "";
            }

            return returnString;
        }
    }
}
