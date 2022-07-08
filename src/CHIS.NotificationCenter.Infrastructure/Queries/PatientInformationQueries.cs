using CHIS.Framework.Core;
using CHIS.Framework.Data.ORM;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.Share.MedicalAge;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Infrastructure.Queries
{
    public class PatientInformationQueries : DALBase, IPatientInformationQueries
    {
        private readonly ICallContext _callContext;
        private readonly ITimeManager _timeManager;
        private readonly IEncounterParticipantQueries _encounterPaticipantQueries;
        private readonly IMedicalRecordMergingQueries _medicalRecordMergingQueries;
        private readonly NotificationCenterContext _notificationCenterContext;
        

        public PatientInformationQueries(ICallContext callContext
            , ITimeManager timeManager
            , IEncounterParticipantQueries encounterParticipantQueries
            , IMedicalRecordMergingQueries medicalRecordMergingQueries
            , NotificationCenterContext notificationCenterContext) : base(callContext)
        {
            this.DBCatalog = NotificationCenterContext.DOMAIN_NAME;
            _notificationCenterContext = notificationCenterContext;
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _encounterPaticipantQueries = encounterParticipantQueries ?? throw new ArgumentNullException(nameof(encounterParticipantQueries));
            _medicalRecordMergingQueries = medicalRecordMergingQueries ?? throw new ArgumentNullException(nameof(medicalRecordMergingQueries));


        }

        /// <summary>
        /// RetrievePatientInfomationV2 로 업데이트 함.  [사용안하는지 확인하고 삭제할것.]
        /// Discarded.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filterOption">0:unhandled, 1:handled,2:all</param>
        /// <returns></returns>
        public async Task<PatientInformationView> RetrievePatientInfomation(string patientId, string encounterId)
        {
            PatientInformationView view;
            string queryName = "RetrievePatientInfomation";
            object parameter = new { PatientId = patientId, EncounterId = encounterId, TenantId = _callContext.TenantId, HospitalId = _callContext.HospitalId };

            using (var connection = db.CreateConnection())
            {
                view = (await connection.SqlTranslateQueryAsync<PatientInformationView>(queryName, param: parameter).ConfigureAwait(false)).FirstOrDefault();
            }

            return view;
        }

        /// <summary>
        /// 환자정보 조회 method.
        /// 환자의 진료과는 encounter의 attendingDepartmentId로 변경. (기존 : Participant의 "01", "02", "06" department.)
        /// </summary>
        /// <param name="patientId"></param>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public async Task<PatientInformationView> RetrievePatientInfomationV2(string patientId, string encounterId)
        {
            DateTime currentDateTimeLocal = _timeManager.GetNow();
            //DateTime currentDateTimeUTC = _timeManager.GetUTCNow();
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            // var participantTypeCodeCondition = new string[] { "01", "02", "06" };

            var query =
                        from patient in _notificationCenterContext.Query<PatientReadModel>()
                            .Where(x => x.Id == patientId
                                && x.TenantId == tenantId
                                && x.HospitalId == hospitalId
                            )
                        join encounter in _notificationCenterContext.Query<EncounterReadModel>()
                            .Where(X => X.Id == encounterId && X.TenantId == tenantId 
                                && X.HospitalId == hospitalId)
                        on patient.Id equals encounter.PatientId into encounterProjection
                        from encounterPj in encounterProjection.DefaultIfEmpty()

                        join department in _notificationCenterContext.Query<DepartmentReadModel>()
                            .Where(p => p.TenantId == tenantId
                                && p.HospitalId == hospitalId
                            )
                        on encounterPj.AttendingDepartmentId equals department.Id into departmentProjection
                        from departmentPj in departmentProjection.DefaultIfEmpty()

                        where patient.TenantId == tenantId
                        && patient.HospitalId == hospitalId
                        //&& (participantTypeCodeCondition.Contains(participantPj.TypeCode) || string.IsNullOrEmpty(participantPj.Id))
                        && (encounterPj.IsValidDataRow || string.IsNullOrEmpty(encounterPj.Id))
                        //&& ( participantPj.IsValidDataRow == true || string.IsNullOrEmpty(participantPj.Id))
                        select new PatientInformationView()
                        {
                            PatientId = patient.Id,
                            EncounterId = encounterPj.Id,
                            PatientDisplayId = patient.PatientDisplayId,
                            PatientName = patient.PatientFullName,
                            BirthDay = patient.BirthDate,
                            GenderType = string.Equals(patient.GenderCode, "01", StringComparison.Ordinal) ? "M" :
                                    string.Equals(patient.GenderCode, "02", StringComparison.Ordinal) ? "F" : "",
                            Age = patient.BirthDate.Value == null ? "" :
                                EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patient.BirthDate.Value, currentDateTimeLocal, new UnitMark("세", "개월", "일")),
                            DepartmentId = departmentPj.Id,
                            DepartmentName = departmentPj.Name

                        };


            var patientInformationView = await query.FirstOrDefaultAsync().ConfigureAwait(false);


            return patientInformationView;
        }


        /// <summary>
        /// 환자/보호자 전화번호 찾기 - sms 전송시 최적 전화번호 찾아서 설정, 백엔드 api
        /// Contact : relationshipCode : 01 본인, 그외 가족, classificationCode : 01 = 1순위
        /// ContactTelephone : classificationCode : 01 >  휴대폰
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public async Task<SmsRecipientDto> FindPatientContact(string patientId, SmsRecipientType smsRecipientType)
        {
            var contactList =
                 from patient in _notificationCenterContext.Query<PatientReadModel>()
                 join contact in _notificationCenterContext.Query<ContactReadModel>()
                    .Where(p => p.HospitalId == _callContext.HospitalId
                            && p.TenantId == _callContext.TenantId
                    )
                 on patient.DemographicId equals contact.Demographicid

                 join contactTelephone in _notificationCenterContext.Query<ContactTelephoneReadModel>()
                    .Where(p => p.ClassificationCode == "01")
                 on contact.Id equals contactTelephone.ContactId into contactTelephonesProjection
                 from contactTelephonePj in contactTelephonesProjection.DefaultIfEmpty()

                 where patient.Id == patientId
                    && patient.TenantId == _callContext.TenantId
                    && patient.HospitalId == _callContext.HospitalId
                    //0122
                    //&& contact.HospitalId == _callContext.HospitalId
                    //&& contact.TenantId == _callContext.TenantId

                    //&& contactTelephonePj.ClassificationCode == "01" // 휴대폰 필터 조건
                    && !string.IsNullOrEmpty(contactTelephonePj.PhoneNumber)
                    && !string.IsNullOrEmpty(contact.RelationshipCode)
                    && !string.IsNullOrEmpty(contact.ClassificationCode)
                 orderby contact.ClassificationCode
                 select new
                 {
                     patient.Id,
                     contact.DisplaySequence,
                     contact.RelationshipCode,
                     contact.ClassificationCode,
                     contact.Name,
                     contactTelephonePj.PhoneNumber,
                 };

            // DESC : 전화번호가 찾아지지 않을경우 대비하여 디폴트 설정을 위한 Dummy Result. 
            //        기본 환자정보만 있고 전화번호 없음.
            var patientQuery =
                                from patient in _notificationCenterContext.Query<PatientReadModel>()
                                where patient.Id == patientId
                                select new SmsRecipientDto()
                                {
                                    ActorId = patient.Id,
                                    Name = patient.PatientFullName,
                                    PatientContactRelationShipCode = "01",
                                    PatientContactClassificationCode = "01"
                                };

            // DESC : 환자 본인 또는 보호자 연락처 검색.
            //var ConatctPhones = await contactList.ToListAsync().ConfigureAwait(false);
            var ConatctPhones = contactList.ToList();
            //SmsRecipientDto smsRecipientContactBase = await patientQuery.FirstOrDefaultAsync().ConfigureAwait(false);
            SmsRecipientDto smsRecipientContactBase = patientQuery.FirstOrDefault();

            //환자 기본정보가 없으면 null리턴
            if (smsRecipientContactBase == null)
            {
                return null;
            }

            if (smsRecipientType == SmsRecipientType.PatientDirectMobile && smsRecipientContactBase != null)
            {
                smsRecipientContactBase.SmsRecipientType = SmsRecipientType.PatientDirectMobile;
                return smsRecipientContactBase;
            }

            SmsRecipientDto smsRecipientContact = null;

            switch (smsRecipientType)
            {
                case SmsRecipientType.Patient: // 환자 본인

                    //smsRecipientContact = ConatctPhones.Where(i => i.RelationshipCode == "01").OrderBy(i => new { i.ClassificationCode, i.DisplaySequence })
                    smsRecipientContact = ConatctPhones.Where(i => i.RelationshipCode == "01").OrderBy(i => i.ClassificationCode).ThenBy(i => i.DisplaySequence)
                                                    .Select(i => new SmsRecipientDto
                                                    {
                                                        SmsRecipientType = Domain.Enum.SmsRecipientType.Patient,
                                                        Mobile = i.PhoneNumber,
                                                        Name = i.Name,
                                                        ActorId = i.Id,
                                                        PatientContactRelationShipCode = i.RelationshipCode,
                                                        PatientContactClassificationCode = i.ClassificationCode
                                                    }).FirstOrDefault();

                    #region 본인 정보가 없는경우 차상위 보호자 연락처 추가
                    /*
                    if (smsRecipientContact == null)
                    {
                        // DESC 환자 본인 전화번호가 없을경우 차상위 보호자 연락처 가져옴
                        smsRecipientContact = ConatctPhones.Where(i => i.RelationshipCode != "01").OrderBy(i => i.DisplaySequence)
                                               .Select(i => new SmsRecipientDto
                                               {
                                                   SmsRecipientType = Domain.Enum.SmsRecipientType.Patient,
                                                   Mobile = i.PhoneNumber,
                                                   Name = i.Name,
                                                   //ActorId = i.PatientId,
                                                   ActorId = i.Id,
                                                   PatientContactRelationShipCode = i.RelationshipCode,
                                                   PatientContactClassificationCode = i.ClassificationCode
                                               }).FirstOrDefault();
                    } 
                    */
                    #endregion

                    if (smsRecipientContact == null)
                    {

                        smsRecipientContactBase.SmsRecipientType = Domain.Enum.SmsRecipientType.Patient;
                        smsRecipientContactBase.PatientContactRelationShipCode = "01";
                        smsRecipientContact = smsRecipientContactBase;
                    }

                    break;
                case SmsRecipientType.Guardian: // 보호자
                    smsRecipientContact = ConatctPhones.Where(i => i.RelationshipCode != "01").OrderBy(i => i.DisplaySequence)
                                                .Select(i => new SmsRecipientDto
                                                {
                                                    SmsRecipientType = Domain.Enum.SmsRecipientType.Patient,
                                                    Mobile = i.PhoneNumber,
                                                    Name = i.Name,
                                                    ActorId = i.Id,
                                                    PatientContactRelationShipCode = i.RelationshipCode,
                                                    PatientContactClassificationCode = i.ClassificationCode
                                                }).FirstOrDefault();

                    if (smsRecipientContact == null)
                    {
                        smsRecipientContactBase.SmsRecipientType = Domain.Enum.SmsRecipientType.Guardian;
                        smsRecipientContact = smsRecipientContactBase;
                    }
                    break;
                default:
                    break;

            }
            return smsRecipientContact;

        }

        /// <summary>
        /// 환자 관련 전화번호 조회 / 환자 sms 전송 ui에서 사용
        /// </summary>
        /// <param name="patientId">환자아이디</param>
        /// <returns></returns>
        public async Task<List<SmsRecipientDto>> FindPatientContact(string patientId)
        {
            var contractList = from patient in _notificationCenterContext.Query<PatientReadModel>()
                               join patientContact in _notificationCenterContext.Query<ContactReadModel>()
                                .Where(p => p.HospitalId == _callContext.HospitalId
                                        && p.TenantId == _callContext.TenantId
                                )

                               on patient.DemographicId equals patientContact.Demographicid  into contactProjection
                               from contactPj in contactProjection.DefaultIfEmpty()

                               join patientContactTelephone in _notificationCenterContext.Query<ContactTelephoneReadModel>()
                                .Where(p => p.ClassificationCode == "01")
                               on contactPj.Id equals patientContactTelephone.ContactId into contactTelephoneProjection
                               from contactTelephonePj in contactTelephoneProjection.DefaultIfEmpty()
                               where patient.TenantId == _callContext.TenantId
                                    && patient.HospitalId == _callContext.HospitalId
                                    //&& contactTelephonePj.ClassificationCode == "01"
                                    && !string.IsNullOrEmpty(contactTelephonePj.PhoneNumber)
                                    && string.Equals(patient.Id, patientId, StringComparison.Ordinal)
                               select new
                               {
                                   Id = patient.Id,
                                   RelationshipCode = contactPj.RelationshipCode,
                                   ClassificationCode = contactTelephonePj.ClassificationCode,
                                   Name = contactPj.Name,
                                   PhoneNumber = contactTelephonePj.PhoneNumber,
                                   DisplaySequence = contactTelephonePj.DisplaySequence,
                                   DisplayOrder = contactPj.ClassificationCode
                               };

            var ConatctPhones = await contractList.ToListAsync().ConfigureAwait(false);

            List<SmsRecipientDto> smsRecipientContact = null;

            smsRecipientContact = ConatctPhones
                                         //.OrderBy(x => x.RelationshipCode).ThenBy(x => x.ClassificationCode).ThenBy(x => x.DisplaySequence)
                                         .OrderBy(x => x.DisplayOrder)
                                         .ThenBy(x => x.RelationshipCode)
                                         .ThenBy(x => x.ClassificationCode)
                                         .Select(i => new SmsRecipientDto
                                         {
                                             SmsRecipientType = i.RelationshipCode == "01" ? Domain.Enum.SmsRecipientType.Patient
                                                                                            : Domain.Enum.SmsRecipientType.Guardian,
                                             Mobile = i.PhoneNumber,
                                             Name = i.Name,
                                             ActorId = i.Id,
                                             PatientContactRelationShipCode = i.RelationshipCode,
                                             PatientContactClassificationCode = i.ClassificationCode
                                         }).ToList();
            //.FirstOrDefault();
            return smsRecipientContact;

        }


        /// <summary>
        /// 환자 관련 전화번호 조회 , relationshipcode, classificationcode 가 명시적으로 있을 경우
        /// </summary>
        /// <param name="patientId">환자아이디</param>
        /// <returns></returns>
        public async Task<SmsRecipientDto> FindPatientContact(string patientId, string relationShipCode, string classificationCode)
        {

            
            var contactList = from patient in _notificationCenterContext.Query<PatientReadModel>()
                               join patientContact in _notificationCenterContext.Query<ContactReadModel>()
                                .Where(p => p.TenantId == _callContext.TenantId
                                        && p.HospitalId == _callContext.HospitalId
                                )
                               on patient.DemographicId equals patientContact.Demographicid into contactProjection
                               from contactPj in contactProjection.DefaultIfEmpty()

                               join patientContactTelephone in _notificationCenterContext.Query<ContactTelephoneReadModel>()
                                .Where(p => p.ClassificationCode == "01")
                               on contactPj.Id equals patientContactTelephone.ContactId into contactTelephoneProjection
                               from contactTelephonePj in contactTelephoneProjection.DefaultIfEmpty()

                               where patient.TenantId == _callContext.TenantId
                                    && patient.HospitalId == _callContext.HospitalId
                                    //&& contactTelephonePj.ClassificationCode == classificationCode
                                    && contactPj.ClassificationCode == classificationCode
                                    && contactPj.RelationshipCode == relationShipCode
                                    //&& contactPj.ClassificationCode == "01"
                                    && !string.IsNullOrEmpty(contactTelephonePj.PhoneNumber)
                                    && string.Equals(patient.Id, patientId, StringComparison.Ordinal)
                               select new
                               {
                                   patient.Id,
                                   contactPj.RelationshipCode,
                                   contactPj.ClassificationCode,
                                   contactPj.Name,
                                   contactTelephonePj.PhoneNumber,
                                   contactTelephonePj.DisplaySequence,
                               };

            var ConatctPhones = await contactList.ToListAsync().ConfigureAwait(false);

            SmsRecipientDto smsRecipientContact = null;

            smsRecipientContact = ConatctPhones
                                         .Select(i => new SmsRecipientDto
                                         {
                                             SmsRecipientType = i.RelationshipCode == "01" ? Domain.Enum.SmsRecipientType.Patient
                                                                                            : Domain.Enum.SmsRecipientType.Guardian,
                                             Mobile = i.PhoneNumber,
                                             Name = i.Name,
                                             ActorId = i.Id,
                                             PatientContactRelationShipCode = i.RelationshipCode,
                                             PatientContactClassificationCode = i.ClassificationCode
                                         }).FirstOrDefault();
            return smsRecipientContact;

        }

        /// <summary>
        /// 멀티 환자 sms 전송 ui에서 사용
        /// </summary>
        /// <param name="patientIds"></param>
        /// <returns></returns>
        public async Task<List<SmsRecipientDto>> SearchPatientContacts(List<string> patientIds)
        {

            var contactList = from patient in _notificationCenterContext.Query<PatientReadModel>()
                               where patient.TenantId == _callContext.TenantId
                                    && patient.HospitalId == _callContext.HospitalId
                                    && patientIds.Contains(patient.Id)
                               select new
                               {
                                   Id = patient.Id,
                                   RelationshipCode = "01",
                                   ClassificationCode = "01",
                                   Name = patient.PatientFullName,
                                   PhoneNumber = patient.SmsTelephoneNumber ?? "",
                                   DisplaySequence = 1
                               };




            List < SmsRecipientDto > smsRecipientContacts = null;

            var ConatctPhones = await contactList.OrderBy(p => p.Name).ToListAsync().ConfigureAwait(false);



            smsRecipientContacts = ConatctPhones
                                         .Select(i => new SmsRecipientDto
                                         {
                                             SmsRecipientType = Domain.Enum.SmsRecipientType.Patient,
                                             Mobile = i.PhoneNumber,
                                             Name = i.Name,
                                             ActorId = i.Id,
                                             PatientContactRelationShipCode = i.RelationshipCode,
                                             PatientContactClassificationCode = i.ClassificationCode
                                         }).ToList();

            return smsRecipientContacts;

        }

        /// <summary>
        /// encounter participant 조회
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public async Task<List<EncounterParticipantDto>> RetrievePatientParticipant(string encounterId)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            //List<ParticipantReadModel> participantList = await GetEncounterPaticipantList(encounterId);
            //List<ParticipantReadModel> participantList = await _encounterPaticipantQuerues.RetrievesEncounterPaticipantList(encounterId: encounterId).ConfigureAwait(false);
            List<ParticipantReadModel> participantList = await _encounterPaticipantQueries.RetrievesEncounterPaticipantList(encounterId: encounterId).ConfigureAwait(false);

            List<PolicyProtocolModel> recipientPolicyProtocolList = _notificationCenterContext.RecipientPolicyProtocols
                           .Where(p => p.TenantId == tenantId && p.Type == RecipientPolicyType.Encounter)
                           .Select(p => new PolicyProtocolModel { policyCode = p.PolicyCode, name = p.Name })
                           .ToList();

           
            List<ParticipantReadModel> convertParticipantList = participantList.ToList();
            List<string> actorIds = participantList.Select(p => p.ActorId).ToList();

            //######################################
            /*
            01 : 진료의
            03 : 입원 지시의
            05 : 주치의
            06 : 퇴원 지시의
            07 : 담당 간호사 (encounter에 정보없음)
            */
            //######################################
            //string[] participantProtocolCodeArr = { "01", "03", "05", "06" };
            
            var participantEmployeeList = 
                        (from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        .Where(
                            p => p.TenantId == tenantId 
                            && p.HospitalId == hospitalId
                            && actorIds.Contains(p.Id))
                        select new EncounterParticipantDto
                        {
                            EncounterId = encounterId,
                            EmployeeId = employee.Id,
                            EmployeeDisplayId = employee.DisplayId,
                            EmployeeName = employee.FullName,
                            Contact = "",
                            ActorTypeCode = "",

                            DepartmentId = employee.DepartmentId,
                            DepartmentName = employee.DepartmentName,
                            JobPositionId = employee.JobPositionCode,
                            JobPositionName = employee.JobPositionName,

                            ActorTypeName = ""
                        }).ToList();

            var resultList = (from staffList in convertParticipantList
                             join empList in participantEmployeeList
                             on staffList.ActorId equals empList.EmployeeId
                             select new EncounterParticipantDto
                             {
                                 EncounterId = staffList.EncounterId,
                                 EmployeeId = empList.EmployeeId,
                                 EmployeeDisplayId = empList.EmployeeDisplayId,
                                 EmployeeName = empList.EmployeeName,
                                 Contact = "",
                                 ActorTypeCode = staffList.TypeCode.Replace("E", "", StringComparison.Ordinal),
                                 

                                 DepartmentId = empList.DepartmentId,
                                 DepartmentName = empList.DepartmentName,
                                 JobPositionId = empList.JobPositionId,
                                 JobPositionName = empList.JobPositionName,
                                 ActorTypeName = getActorTypeName(recipientPolicyProtocolList, staffList.TypeCode)
                             }).ToList();


            #region prevSource
            //var list =


            //           from employee in _notificationCenterContext.Query<EmployeeReadModel>().Where(p => p.TenantId == tenantId && p.HospitalId == hospitalId)
            //           join participant in convertParticipantList

            //           //   on employee.Id equals participant.ActorId  into participantProjection
            //           //from participantPj in participantProjection.DefaultIfEmpty()

            //           on employee.Id equals participant.ActorId 

            //            //on new { empId = employee.Id, tenantId = employee.TenantId, hospitalId = employee.HospitalId } equals 
            //            //    new { empId = participant.ActorId, tenantId, hospitalId }

            //           /*
            //           join recipientPolicyProtocols in _notificationCenterContext.RecipientPolicyProtocols.Where(p => p.Type == RecipientPolicyType.Encounter)
            //              //on participant.TypeCode equals recipientPolicyProtocols.PolicyCode


            //              on new { typeCode = participant.TypeCode, tenantId = participant.TenantId } equals
            //                new { typeCode = recipientPolicyProtocols.PolicyCode, tenantId = recipientPolicyProtocols.TenantId }
            //            */

            //           /*
            //           where 
            //                //participant.EncounterId == encounterId
            //                //&& participant.HospitalId == _callContext.HospitalId
            //                //&& participant.TenantId == _callContext.TenantId
            //                //recipientPolicyProtocols.TenantId == tenanatId

            //                //&& employee.TenantId == tenanatId
            //                //&& employee.HospitalId == hospitalId
            //                string.Equals(employee.TenantId, tenantId, StringComparison.Ordinal)
            //                && string.Equals(employee.HospitalId, hospitalId, StringComparison.Ordinal)
            //           */
            //           select new EncounterParticipantDto
            //              {
            //                  EncounterId = participant.EncounterId,
            //                  EmployeeId = employee.Id,
            //                  EmployeeDisplayId = employee.DisplayId,
            //                  EmployeeName = employee.FullName,
            //                  Contact = "",
            //                  ActorTypeCode = participant.TypeCode.Replace("E", "", StringComparison.Ordinal),
            //               //ActorTypeName = recipientPolicyProtocols.Name,
            //                  //ActorTypeName = (_notificationCenterContext.RecipientPolicyProtocols.OrderBy(p => p.PolicyCode).First(p => p.Type == RecipientPolicyType.Encounter && p.TenantId == tenantId && p.PolicyCode == participant.TypeCode ).Name),

            //                  DepartmentId = employee.DepartmentId,
            //                  DepartmentName = employee.DepartmentName,
            //                  JobPositionId = employee.JobPositionCode,
            //                  JobPositionName = employee.JobPositionName,
            //                   //ActorTypeName = (
            //                   //  recipientPolicyProtocolList
            //                   //  .FirstOrDefault(p => p.policyCode == participant.TypeCode)
            //                   //  .name),
            //                   ActorTypeName = getActorTypeName(recipientPolicyProtocolList, participant.TypeCode)
            //           }; 
            #endregion

            #region ## 변경쿼리 (불필요한 encounterReadmodel 제거)
            /*
            var list =
                            from encounter in _notificationCenterContext.Query<EncounterReadModel>()
                                //join participant in _notificationCenterContext.Query<ParticipantReadModel>()
                        join participant in participantList
                            on encounter.Id equals participant.EncounterId into encounterProjection
                            from encounterPj in encounterProjection.DefaultIfEmpty()

                            join employee in _notificationCenterContext.Query<EmployeeReadModel>()
                            on encounterPj.ActorId equals employee.Id into employeeProjection
                            from employeePj in employeeProjection.DefaultIfEmpty()

                            join recipientPolicyProtocols in _notificationCenterContext.RecipientPolicyProtocols
                            //on encounterPj.TypeCode equals recipientPolicyProtocols.PolicyCode.Replace("E", "", StringComparison.Ordinal)
                            on encounterPj.TypeCode equals recipientPolicyProtocols.PolicyCode


                            where encounterPj.EncounterId == encounterId
                                && recipientPolicyProtocols.Type == Domain.Enum.RecipientPolicyType.Encounter
                                //&& (new string[] { "01", "02", "03", "04", "05", "06" }).Contains(encounterPj.TypeCode)
                                && participantProtocolCodeArr.Contains(encounterPj.TypeCode)
                                && employeePj.Id != null
                                && encounterPj.TenantId == _callContext.TenantId
                                && encounterPj.HospitalId == _callContext.HospitalId

                                //0122
                                && encounter.TenantId == _callContext.TenantId
                                && encounter.HospitalId == _callContext.HospitalId

                                && recipientPolicyProtocols.TenantId == _callContext.TenantId

                            select new EncounterParticipantDto
                            {
                                EncounterId = encounterPj.EncounterId,
                                EmployeeId = employeePj.Id,
                                EmployeeDisplayId = employeePj.DisplayId,
                                EmployeeName = employeePj.FullName,
                                Contact = "",
                                ActorTypeCode = encounterPj.TypeCode,
                                ActorTypeName = recipientPolicyProtocols.Name,
                                DepartmentId = employeePj.DepartmentId,
                                DepartmentName = employeePj.DepartmentName,
                                JobPositionId = employeePj.JobPositionCode,
                                JobPositionName = employeePj.JobPositionName
                            }; 
            */
            #endregion

            //var ParticipantList = resultList.ToList();
            //var ParticipantList = await list.ToListAsync().ConfigureAwait(false);

            #region 주석
            /*
                // ▼ 진료의와 주치의가 겹칠때 진료의만 남기도록 변경
                var ParticipantList = await list.ToListAsync().ConfigureAwait(false);
                var Emp01 = ParticipantList.FirstOrDefault(i => i.ActorTypeCode == "01");   //진료의 / 외래 접수 진료의
                var Emp02 = ParticipantList.FirstOrDefault(i => i.ActorTypeCode == "02");   //진료의 / 응급실 접수 진료의
                var Emp03 = ParticipantList.FirstOrDefault(i => i.ActorTypeCode == "03");   //입원 지시의 / 입원지시의사
                var Emp04 = ParticipantList.FirstOrDefault(i => i.ActorTypeCode == "04");   //진료의 / 응급실 진료의
                var Emp05 = ParticipantList.FirstOrDefault(i => i.ActorTypeCode == "05");   //주치의 / 입원주치의
                var Emp06 = ParticipantList.FirstOrDefault(i => i.ActorTypeCode == "06");   //진료의 / 입원진료의

                //▲ 진료의와 주치의가 겹칠때 진료의만 남기도록 변경
                // DESC : 응급실 접수 진료의(02) 와 응급실 진료의(04)가 동일인일경우 04 리턴 
                if (Emp02 != null && Emp04 != null && Emp02.EmployeeId == Emp04.EmployeeId)
                {
                    ParticipantList.Remove(Emp02);
                }
                // DESC : 입원지시의 와 입원주치의가 동일인일경우 05 리턴
                if (Emp03 != null && Emp05 != null && Emp03.EmployeeId == Emp05.EmployeeId)
                {
                    ParticipantList.Remove(Emp03);
                }
                // DESC : 입원지시의 와 입원진료의가 동일인일경우 06 리턴
                if (Emp03 != null && Emp06 != null && Emp03.EmployeeId == Emp06.EmployeeId)
                {
                    ParticipantList.Remove(Emp03);
                }
                // DESC : 입원주치의와 입원진료의가 동일인일경우 06 리턴
                if (Emp05 != null && Emp06 != null && Emp05.EmployeeId == Emp06.EmployeeId)
                {
                    ParticipantList.Remove(Emp05);
                }
                */
            #endregion

            return resultList;
            //throw new NotImplementedException();
        }

        private string getActorTypeName(List<PolicyProtocolModel> recipientPolicyProtocolList, string typeCode)
        {
            var returnRow = recipientPolicyProtocolList.FirstOrDefault(p => p.policyCode == typeCode).name;

            return returnRow;
        }

        public PatientReadModel RetrievePatientInfomationWithPatientId(string patientId)
        {
            //throw new NotImplementedException();

            var row = from patientRow in _notificationCenterContext.Query<PatientReadModel>()
                      where patientRow.Id == patientId
                        && patientRow.TenantId == _callContext.TenantId
                        && patientRow.HospitalId == _callContext.HospitalId
                      select patientRow;

            var patientInfo = row.FirstOrDefault();
                      

            //var patientInfo = await _notificationCenterContext.Query<PatientReadModel>()
            //    .Where(p => p.Id == patientId
            //        && p.TenantId == _callContext.TenantId
            //        && p.HospitalId == _callContext.HospitalId
            //    )
            //    .FirstOrDefaultAsync().ConfigureAwait(false);

            return patientInfo;
        }

        /// <summary>
        /// 직원정보로 수신된 인박스의 환자정보 목록리턴
        /// 현재 미사용(20200409)
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<List<PatientInformationView>> FindPatientListForInbox(string employeeId)
        {

            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            DateTime currentDateTimeLocal = _timeManager.GetNow();

            var innerRows = from emi in _notificationCenterContext.EmployeeMessageInstances
                           join mdi in _notificationCenterContext.MessageDispatchItems
                           on emi.MessageDispatchItemId equals mdi.Id
                           where emi.EmployeeId == employeeId
                               && emi.TenantId == tenantId && emi.HospitalId == hospitalId
                               && mdi.TenantId == tenantId && mdi.HospitalId == hospitalId
                               && mdi.IsCanceled == false
                           group mdi  by  new { mdi.PatientId }  into grp
                           select new { patientId = grp.Key.PatientId  };
                           

             var patientInfoRow = from patientRows in _notificationCenterContext.Query<PatientReadModel>()
                                 .Where(p => p.TenantId == _callContext.TenantId
                                       && p.HospitalId == _callContext.HospitalId
                                    )
                                 join idRows in innerRows
                                 on patientRows.Id equals idRows.patientId
                                 select new PatientInformationView
                                 {
                                     PatientId = patientRows.Id,
                                     PatientDisplayId = patientRows.PatientDisplayId,
                                     PatientName = patientRows.PatientFullName,
                                     BirthDay = patientRows.BirthDate,
                                     GenderType = string.Equals(patientRows.GenderCode, "01", StringComparison.Ordinal) ? "M" :
                                    string.Equals(patientRows.GenderCode, "02", StringComparison.Ordinal) ? "F" : "",
                                     Age = patientRows.BirthDate.Value == null ? "" :
                                EdgeRiumAge.Age(Share.MedicalAge.AgeTypes.MedicalAge, patientRows.BirthDate.Value, currentDateTimeLocal, new UnitMark("세", "개월", "일")),
                                 };


            var returnList = await patientInfoRow.ToListAsync().ConfigureAwait(false);

            return returnList;
        }

        /// <summary>
        /// encounter의 participant목록
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        //private async Task<List<ParticipantReadModel>> GetEncounterPaticipantList(string encounterId)
        //{
        //    var encounterParticipantRow = await
        //                            _notificationCenterContext.Query<EncounterReadModel>()
        //                            .Where(p => p.Id == encounterId
        //                                && p.HospitalId == _callContext.HospitalId
        //                                && p.TenantId == _callContext.TenantId
        //                            ).FirstOrDefaultAsync().ConfigureAwait(false);

        //    List<ParticipantReadModel> participantList = new List<ParticipantReadModel>();
        //    #region ## staff 조회, List add
        //    if (encounterParticipantRow != null)
        //    {
        //        //입원지시의(03)
        //        if (encounterParticipantRow.AdmittingPhysicianId != null)
        //        {
        //            participantList.Add(new ParticipantReadModel()
        //            {
        //                TypeCode = "01",
        //                ActorId = encounterParticipantRow.AdmittingPhysicianId,
        //                EncounterId = encounterId
        //            });
        //        }

        //        //진료의(01)
        //        if (encounterParticipantRow.AttendingPhysicianId != null)
        //        {
        //            participantList.Add(new ParticipantReadModel()
        //            {
        //                TypeCode = "03",
        //                ActorId = encounterParticipantRow.AttendingPhysicianId,
        //                EncounterId = encounterId
        //            });
        //        }

        //        //주치의(05)
        //        if (encounterParticipantRow.PrimaryCarePhysicianId != null)
        //        {
        //            participantList.Add(new ParticipantReadModel()
        //            {
        //                TypeCode = "05",
        //                ActorId = encounterParticipantRow.PrimaryCarePhysicianId,
        //                EncounterId = encounterId
        //            });
        //        }

        //        //퇴원지시의(06)
        //        if (encounterParticipantRow.DischargeOrdererId != null)
        //        {
        //            participantList.Add(new ParticipantReadModel()
        //            {
        //                TypeCode = "06",
        //                ActorId = encounterParticipantRow.DischargeOrdererId,
        //                EncounterId = encounterId
        //            });
        //        }
        //    } 
        //    #endregion

        //    return participantList;
        //}


        #region 주석
        /// <summary>
        /// 보호자 전화번호만 조회
        /// </summary>
        /// <param name="patientId">환자아이디</param>
        /// <param name="relationShipCode">환자와의 관계</param>
        /// <param name="classificationCode">우선순위</param>
        /// <returns></returns>
        //public async Task<SmsRecipientDto> GetGuardianContact(string patientId, string relationShipCode, string classificationCode)
        //{
        //    var contactList =
        //         from patient in _notificationCenterContext.Patients
        //         join contact in _notificationCenterContext.Contacts
        //         on patient.Id equals contact.PatientId

        //         join contactTelephone in _notificationCenterContext.ContactTelephones
        //         on contact.Id equals contactTelephone.ContactId into contactTelephonesProjection
        //         from contactTelephonePj in contactTelephonesProjection.DefaultIfEmpty()

        //         where patient.Id == patientId
        //         && patient.TenantId == _callContext.TenantId
        //         && patient.HospitalId == _callContext.HospitalId
        //         && contact.RelationshipCode == relationShipCode
        //         && contact.ClassificationCode == classificationCode
        //         && contactTelephonePj.ClassificationCode == "01" // 휴대폰 필터 조건
        //         && !string.IsNullOrEmpty(contactTelephonePj.PhoneNumber)
        //         orderby contact.DisplaySequence
        //         select new
        //         {
        //             contact.PatientId,
        //             contact.RelationshipCode,
        //             contact.ClassificationCode,
        //             contact.Name,
        //             contactTelephonePj.PhoneNumber,
        //             contactTelephonePj.DisplaySequence,
        //         };

        //    // DESC : 전화번호가 찾아지지 않을경우 대비하여 디폴트 설정을 위한 Dummy Result. 
        //    //        기본 환자정보만 있고 전화번호 없음.
        //    var patientQuery = from patient in _notificationCenterContext.Patients
        //                       where patient.Id == patientId
        //                       select new SmsRecipientDto()
        //                       {
        //                           ActorId = patient.Id,
        //                           Name = patient.Name,
        //                           PatientContactRelationShipCode = "01",
        //                           PatientContactClassificationCode = "01"
        //                       };

        //    // DESC : 보호자 연락처 조건으로 검색.
        //    var ConatctPhones = await contactList.ToListAsync().ConfigureAwait(false);
        //    SmsRecipientDto smsRecipientContactBase = await patientQuery.FirstOrDefaultAsync().ConfigureAwait(false);

        //    SmsRecipientDto smsRecipientContact = null;

        //    smsRecipientContact = ConatctPhones.OrderBy(i => i.DisplaySequence)
        //                                 .Select(i => new SmsRecipientDto
        //                                 {
        //                                     SmsRecipientType = Domain.Enum.SmsRecipientType.Patient
        //                                         ,Mobile = i.PhoneNumber
        //                                         ,Name = i.Name
        //                                         ,ActorId = i.PatientId
        //                                         ,PatientContactRelationShipCode = i.RelationshipCode
        //                                         ,PatientContactClassificationCode = i.ClassificationCode
        //                                 }).FirstOrDefault();

        //    if (smsRecipientContact == null)
        //    {
        //        smsRecipientContactBase.SmsRecipientType = Domain.Enum.SmsRecipientType.Guardian;
        //        smsRecipientContact = smsRecipientContactBase;
        //    }

        //    return smsRecipientContact;

        //} 
        #endregion
    }

    internal class PolicyProtocolModel
    {
        public string policyCode { get; set; }
        public string name { get; set; }
    }
}
