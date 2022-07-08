using CHIS.Framework.Data.ORM;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Application.Proxies;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Queries.ReadModels;
using CHIS.NotificationCenter.Application.Queries.ReadModels.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.Queries.ReadModels.MessageDispatcher;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CHIS.NotificationCenter.Infrastructure.Queries
{
    public class MessageDispatcherQueries : DALBase, IMessageDispatcherQueries
    {

        private readonly NotificationCenterContext _notificationCenterContext;
        //private readonly IEncounteringProxy _encounteringProxy;
        private readonly IDutySchedulingAssignedNursesProxy _dutySchedulingAssignedNursesProxy;
        private readonly ICallContext _callContext;
        private readonly IEncounterParticipantQueries _encounterPaticipantQuerues;
        public MessageDispatcherQueries(ICallContext context
            , NotificationCenterContext notificationCenterContext
            //, IEncounteringProxy encounteringProxy
            , IEncounterParticipantQueries encounterParticipantQueries
            , IDutySchedulingAssignedNursesProxy dutySchedulingAssignedNursesProxy) : base(context)
        {
            this.DBCatalog = NotificationCenterContext.DOMAIN_NAME;
            _notificationCenterContext = notificationCenterContext;
            //_encounteringProxy = encounteringProxy;
            _encounterPaticipantQuerues = encounterParticipantQueries ?? throw new ArgumentNullException(nameof(encounterParticipantQueries));

            _dutySchedulingAssignedNursesProxy = dutySchedulingAssignedNursesProxy;
            _callContext = context ?? throw new ArgumentNullException(nameof(context));

        }

        /// <summary>
        /// 메시지 수신자 (직원) 추출
        /// 1.Message Specification에 정의된 수신자와 , 파라미터에 정의된 수신자를 병합함.
        /// </summary>
        /// <param name="messageItem"></param>
        /// <returns></returns>
        public async Task<IList<EmployeeRecipientView>> GetEmployeeRecipients(MessageDispatchItem messageItem)
        {
            if (messageItem == null)
            {
                throw new ArgumentNullException(nameof(messageItem));
            }

            // 1.Message Specification에서 수신설정정보 조회
            MessageSpecification MessageSpec =
                await _notificationCenterContext.MessageSpecifications
                .FirstOrDefaultAsync(i => i.ServiceCode == messageItem.ServiceCode
                    && i.HospitalId == _callContext.HospitalId
                    && i.TenantId == _callContext.TenantId
                ).ConfigureAwait(false);

            await _notificationCenterContext.Entry(MessageSpec).Collection(i => i.DepartmentPolicies).LoadAsync().ConfigureAwait(false);
            await _notificationCenterContext.Entry(MessageSpec).Collection(i => i.EncounterPolicies).LoadAsync().ConfigureAwait(false);
            await _notificationCenterContext.Entry(MessageSpec).Collection(i => i.EmployeeRecipients).LoadAsync().ConfigureAwait(false);


            List<string> employees = new List<string>();
            // 2.encounter policy에 해당하는 수신자 리스트 업
            var employeesOnEncounterPolicies = await BuildEncounterPolicies(MessageSpec, messageItem).ConfigureAwait(false);
            employees.AddRange(employeesOnEncounterPolicies);

            // 3.department policy에 해당하는 수신자 리스트 업
            var employeesOnDepartmentPolicies = await BuildDepartmentPolicies(MessageSpec, messageItem).ConfigureAwait(false);
            employees.AddRange(employeesOnDepartmentPolicies);

            // 4.employee recipient에 해당하는 수신자 리스트 업
            var employeesOnEmployeeRecipients = BuildEmployeeRecipients(MessageSpec, messageItem);
            employees.AddRange(employeesOnEmployeeRecipients);


            List<EmployeeRecipientView> employeeRecipientView = new List<EmployeeRecipientView>();

            // 5.추출된 employees 로 필요정보 조회하여 리턴
            //EncounterParticiantList로 쿼리 돌려야함. // 직원의 부서정보가 null일수 있어 outer 조인 확인.
            //퇴직한 사용자 제외한 명단 추출
            #region ## readmodel 이전
            /*
                var query = from employee in _notificationCenterContext.Employees
                            join departments in _notificationCenterContext.Departments
                            on employee.DepartmentId equals departments.Id into departmemtProjection
                            from departmentPj in departmemtProjection.DefaultIfEmpty()

                            where employees.Contains(employee.Id)
                             && employee.TenantId == _callContext.TenantId
                             && employee.HospitalId == _callContext.HospitalId
                            select new EmployeeRecipientView()
                            {
                                EmployeeId = employee.Id,
                                EmployeeName = employee.FullName,
                                Mobile = employee.Mobile,
                                DepartmentId = employee.DepartmentId,
                                DepartmentName = departmentPj.Name
                            };
                */
            #endregion
            var query = from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        join contactPoint in _notificationCenterContext.Query<ContactPointReadModel>()
                            .Where(p => p.TenantId == _callContext.TenantId
                                && p.SystemType == ContactPointSystem.Mobile
                            )
                        on employee.PersonId equals contactPoint.PersonId into contactPointProjection
                        from contactPointPj in contactPointProjection.DefaultIfEmpty()
                        where employees.Contains(employee.Id)
                        && employee.TenantId == _callContext.TenantId
                        && employee.HospitalId == _callContext.HospitalId
                        && employee.IsRetirement == false
                        select new EmployeeRecipientView()
                        {
                            EmployeeId = employee.Id,
                            EmployeeName = employee.FullName,
                            Mobile = contactPointPj.ContactValue ?? "",
                            DepartmentId = employee.DepartmentId,
                            DepartmentName = employee.DepartmentName
                        };
            employeeRecipientView.AddRange(await query.ToListAsync().ConfigureAwait(false));

            return employeeRecipientView;
        }

        /// <summary>
        /// Encounter Policy 수신자 매핑
        /// </summary>
        /// <param name="messageSpecification"></param>
        /// <param name="messageDispatchItem"></param>
        /// <returns></returns>
        public async Task<List<string>> BuildEncounterPolicies(MessageSpecification messageSpecification, MessageDispatchItem messageDispatchItem)
        {
            if (messageSpecification == null || messageDispatchItem == null)
            {
                throw new ArgumentNullException(nameof(messageSpecification));
            }
            //if (messageDispatchItem == null)
            //{
            //    throw new ArgumentNullException(nameof(messageDispatchItem));
            //}

            List<string> targetEmployees = new List<string>();

            var encounterPolicies = messageSpecification.EncounterPolicies.Select(i => new
            {
                ProtocolCode = i.ProtocolCode
            });


            var assignedEncounterPolicies = messageDispatchItem.AssignedEncounterPolicies.Select(i => new { ProtocolCode = i.ProtocolCode }).ToList();

            var mergedEncounterPolicies = encounterPolicies.Union(assignedEncounterPolicies).ToList();

            //encounter정보가 없거나 수신정책내용이 없으면 중단
            if (string.IsNullOrEmpty(messageDispatchItem.EncounterId) || mergedEncounterPolicies.Count < 1)
            {
                return targetEmployees;
            }
            #region 내원정보별 Encounter의 Participant 및 담당간호사(병동기준) 수신자 mapping 로직.(patientId, encounterId 기준)


            // DESC : ProtocolCode 가 E00001,E00002~ 형태로 되어있음. PatientAdmin->Encounter->ParticipantType과 동일 코드체계로 변경할것.
            //         E00007 : 담당간호사(병동에만 해당)
            //         ex) 01,02~
            //         recipientpolicyprotocol 테이블 코드 수정및 substring 로직 보완 =
            //         코드체계를 E00001 ~ E00007 에서 01~07로 변경함.

            // 담당간호사 처리 
            var picNursePolicy = mergedEncounterPolicies.FirstOrDefault(x => x.ProtocolCode == "E07");

            if (picNursePolicy != null && !string.IsNullOrEmpty(picNursePolicy.ProtocolCode))
            {
                var assignedNurses = _dutySchedulingAssignedNursesProxy.GetAssignedNurses(messageDispatchItem.PatientId, messageDispatchItem.EncounterId).ToList();

                foreach (var nurse in assignedNurses)
                {
                    if (nurse.IsCurrentWorker && !string.IsNullOrEmpty(nurse.EmployeeId)) { targetEmployees.Add(nurse.EmployeeId); }
                }

                mergedEncounterPolicies.Remove(picNursePolicy);
            }



            #region ## encounter staff정보
            List<ParticipantReadModel> participantList = await _encounterPaticipantQuerues.RetrievesEncounterPaticipantList(encounterId: messageDispatchItem.EncounterId).ConfigureAwait(false);

            if (participantList != null)
            {
                //targetEmployees.AddRange(participantList.Select(p => p.ActorId));
                foreach (var policy in mergedEncounterPolicies)
                {
                    //string empId = participantList.Where(p => p.TypeCode == policy.ProtocolCode.Replace("E", "", StringComparison.Ordinal)).Select(p => p.ActorId).FirstOrDefault();
                    string empId = participantList.Where(p => p.TypeCode == policy.ProtocolCode).Select(p => p.ActorId).FirstOrDefault();
                    if (!string.IsNullOrEmpty(empId))
                    {
                        targetEmployees.Add(empId);
                    }
                }
            } 
            #endregion

            #region encounter staff 정보 - 기존코드
            /*
            //encounter read model조회
            var encounterRow = _notificationCenterContext.Query<EncounterReadModel>()
                                .Where(p => p.Id == messageDispatchItem.EncounterId
                                        && p.HospitalId == _callContext.HospitalId
                                        && p.TenantId == _callContext.TenantId)
                                 .FirstOrDefault();

            //지시의정보가 있는경우
            if (encounterRow.AdmittingPhysicianId != null && mergedEncounterPolicies.Count(p => p.ProtocolCode == "E03") > 0)
            {
                targetEmployees.Add(encounterRow.AdmittingPhysicianId);
            }

            //진료의정보가 있는 경우
            if (encounterRow.AttendingPhysicianId != null && mergedEncounterPolicies.Count(p => p.ProtocolCode == "E01") > 0)
            {
                targetEmployees.Add(encounterRow.AttendingPhysicianId);
            }

            //주치의정보가 있는 경우
            if (encounterRow.PrimaryCarePhysicianId != null && mergedEncounterPolicies.Count(p => p.ProtocolCode == "E05") > 0)
            {
                targetEmployees.Add(encounterRow.PrimaryCarePhysicianId);
            }

            //주치의정보가 있는 경우
            if (encounterRow.DischargeOrdererId != null && mergedEncounterPolicies.Count(p => p.ProtocolCode == "E06") > 0)
            {
                targetEmployees.Add(encounterRow.DischargeOrdererId);
            }
            */
            #endregion


            #region encounter proxy 미사용. readmodel로 변경
            /*
                //DESC : Encounter Participants 처리
                //if (mergedEncounterPolicies.Any() && !string.IsNullOrEmpty(messageDispatchItem.EncounterId))

                var encounterBasics = _encounteringProxy.GetEncounterBasic(messageDispatchItem.PatientId, messageDispatchItem.EncounterId).FirstOrDefault();
                if (encounterBasics.Participant == null)
                {
                    return targetEmployees;
                }

                foreach (var policy in mergedEncounterPolicies)
                {
                    Application.Models.ProxyModels.Encountering.ParticipantBasicView participant =
                        encounterBasics.Participant.FirstOrDefault(item => item.Type?.Code == policy.ProtocolCode.Replace("E", "", StringComparison.OrdinalIgnoreCase));

                    if (participant == null || participant.Actor == null)
                    {
                        continue;
                    }
                    targetEmployees.Add(participant.Actor.Id);
                }
                */
            #endregion

            #endregion 내원정보별

            return targetEmployees;
        }

        /// <summary>
        /// Department Policy 수신자 매핑
        /// </summary>
        /// <param name="messageSpecification"></param>
        /// <param name="messageDispatchItem"></param>
        /// <returns></returns>
        public async Task<List<string>> BuildDepartmentPolicies(MessageSpecification messageSpecification, MessageDispatchItem messageDispatchItem)
        {
            if (messageSpecification == null)
            {
                throw new ArgumentNullException(nameof(messageSpecification));
            }
            if (messageDispatchItem == null)
            {
                throw new ArgumentNullException(nameof(messageDispatchItem));
            }

            List<string> EmlpoyeeIdList = new List<string>();

            var DPList = messageSpecification.DepartmentPolicies.Select(i => new
            {
                ProtocolCode = i.ProtocolCode,
                DepartmentId = i.DepartmentId,
                OccupationId = i.OccupationId,
                JobPositionId = i.JobPositionId
            });

            var ADPList = messageDispatchItem.AssignedDepartmentPolicies.Select(i => new
            {
                ProtocolCode = i.ProtocolCode,
                DepartmentId = i.DepartmentId,
                OccupationId = i.OccupationId,
                JobPositionId = i.JobPositionId
            }).ToList();
            var DepartmentPolicyList = DPList.Union(ADPList).ToList();

            //각 정책에서 직원 아이디만 추출하기위한 변수
            if (DepartmentPolicyList.Any())
            {

                foreach (var departmentPolicy in DepartmentPolicyList)
                {
                    switch (departmentPolicy.ProtocolCode)
                    {
                        case "D01": //DESC : Protocol Code > D01 (부서)
                            //var matcheD01Query = from employee in _notificationCenterContext.Employees.Where(x => x.DepartmentId == departmentPolicy.DepartmentId)
                            //                     select employee.Id;
                            var matcheD01Query = from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                                                 .Where(x => x.DepartmentId == departmentPolicy.DepartmentId
                                                    && x.HospitalId == _callContext.HospitalId
                                                    && x.TenantId == _callContext.TenantId
                                                    && x.IsRetirement == false
                                                 )
                                                 select employee.Id;

                            List<string> matchedD01Employees = await matcheD01Query.ToListAsync().ConfigureAwait(false);
                            EmlpoyeeIdList.AddRange(matchedD01Employees);
                            break;
                        case "D02": //DESC : Protocol Code > D02 (부서>직종)
                            //var matcheD02Query = from employee in _notificationCenterContext.Employees
                            //                     .Where(x => x.DepartmentId == departmentPolicy.DepartmentId &&
                            //                     x.OccupationId == departmentPolicy.OccupationId)
                            //                     select employee.Id;
                            var matcheD02Query = from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                                                 .Where(x => x.DepartmentId == departmentPolicy.DepartmentId 
                                                    && x.OccupationCode == departmentPolicy.OccupationId
                                                    && x.TenantId == _callContext.TenantId
                                                    && x.HospitalId == _callContext.HospitalId
                                                    && x.IsRetirement == false
                                                    )
                                                 select employee.Id;

                            List<string> matchedD02Employees = await matcheD02Query.ToListAsync().ConfigureAwait(false);
                            EmlpoyeeIdList.AddRange(matchedD02Employees);
                            break;
                        case "D03": //DESC : Protocol Code > D03 (부서>직종>직급)
                            var matcheD03Query = from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                                                 .Where(x => x.DepartmentId == departmentPolicy.DepartmentId 
                                                     && x.OccupationCode == departmentPolicy.OccupationId 
                                                     && x.JobPositionCode == departmentPolicy.JobPositionId
                                                     && x.TenantId == _callContext.TenantId
                                                     && x.HospitalId == _callContext.HospitalId
                                                     && x.IsRetirement == false
                                                 )
                                                 select employee.Id;

                            List<string> matchedD03Employees = await matcheD03Query.ToListAsync().ConfigureAwait(false);
                            EmlpoyeeIdList.AddRange(matchedD03Employees);
                            break;
                        case "D04": //DESC : Protocol Code > D04 (근무지)
                            //var matcheD04Query = from employee in _notificationCenterContext.Employees.Where(x => x.DepartmentId == departmentPolicy.DepartmentId)
                            //                     select employee.Id;
                            var matcheD04Query = from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                                                 .Where(x => x.DepartmentId == departmentPolicy.DepartmentId
                                                    && x.HospitalId == _callContext.HospitalId
                                                    && x.TenantId == _callContext.TenantId
                                                    && x.IsRetirement == false
                                                 )
                                                 select employee.Id;
                            List<string> matchedD04Employees = await matcheD04Query.ToListAsync().ConfigureAwait(false);
                            EmlpoyeeIdList.AddRange(matchedD04Employees);
                            break;
                        default:
                            break;
                    }

                }
            }

            return EmlpoyeeIdList;
        }

        /// <summary>
        /// 직원 수신자 매핑
        /// </summary>
        /// <param name="messageSpecification"></param>
        /// <param name="messageDispatchItem"></param>
        /// <returns></returns>
        public List<string> BuildEmployeeRecipients(MessageSpecification messageSpecification, MessageDispatchItem messageDispatchItem)
        {
            List<string> EmlpoyeeIdList = new List<string>();

            if (messageSpecification == null)
            {
                throw new ArgumentNullException(nameof(messageSpecification));
            }
            if (messageDispatchItem == null)
            {
                throw new ArgumentNullException(nameof(messageDispatchItem));
            }

            var ERList = messageSpecification.EmployeeRecipients.Select(i => new
            {
                EmployeeId = i.EmployeeId
            });

            var AERList = messageDispatchItem.AssignedEmployeeRecipients.Select(i => new { EmployeeId = i.EmployeeId }).ToList();
            var EmployeeRecipientList = ERList.Union(AERList).ToList();

            if (EmployeeRecipientList.Any())
            {
                #region 직원목록별

                EmlpoyeeIdList.AddRange(EmployeeRecipientList.Select(i => i.EmployeeId));

                #endregion 직원목록별
            }

            return EmlpoyeeIdList;
        }


        /// <summary>
        /// 수신대상자 직원정보 조회
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public async Task<IList<EmployeeRecipientView>> FindAllEmployeeRecipients(List<string> employees)
        {
            List<EmployeeRecipientView> employeeRecipientView;

            string inClause = string.Join(", ", $"'{employees}'");


            using (var connection = db.CreateConnection())
            {
                employeeRecipientView = (
                    await connection.SqlTranslateQueryAsync<EmployeeRecipientView>("FindAllEmployeeRecipients", param:
                    new { InClause = inClause, TenantId = _callContext.TenantId, HospitalId = _callContext.HospitalId, IsRetirement = false }
                    ).ConfigureAwait(false)).ToList();
            }

            return employeeRecipientView;
        }

        /// <summary>
        /// SMS전송위한 직원 연락처정보 조회
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<SmsRecipientDto> FindEmployeeSmsRecipient(string employeeId)
        {

            
            var query = from employee in _notificationCenterContext.Query<EmployeeReadModel>()
                        join contactPoint in _notificationCenterContext.Query<ContactPointReadModel>()
                            .Where(p => p.TenantId == _callContext.TenantId
                                && p.SystemType == ContactPointSystem.Mobile
                            )
                        on employee.PersonId equals contactPoint.PersonId into contactPointProjection
                        from contactPointPj in contactPointProjection.DefaultIfEmpty()
                        where employee.Id == employeeId
                        && employee.TenantId == _callContext.TenantId
                        && employee.HospitalId == _callContext.HospitalId
                        && employee.IsRetirement == false
                        //&& contactPointPj.TenantId == _callContext.TenantId
                        //&& contactPointPj.SystemType == ContactPointSystem.Mobile
                        select new SmsRecipientDto()
                        {
                            ActorId = employee.Id,
                            SmsRecipientType = SmsRecipientType.Employee,
                            Name = employee.FullName,
                            Mobile = contactPointPj.ContactValue ?? ""
                        };
            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
