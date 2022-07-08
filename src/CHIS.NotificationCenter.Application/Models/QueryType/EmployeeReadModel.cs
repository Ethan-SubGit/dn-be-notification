using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class EmployeeReadModel
    {
        #region property
        /// <summary>
        /// employeeId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 사번
        /// </summary>
        public string DisplayId { get; set; }

        /// <summary>
        /// 직종코드
        /// </summary>
        public string OccupationCode { get; set; }

        /// <summary>
        /// 직종명
        /// </summary>
        public string OccupationName { get; set; }

        /// <summary>
        /// 직급코드
        /// </summary>
        public string JobPositionCode { get; set; }

        /// <summary>
        /// 직급명
        /// </summary>
        public string JobPositionName { get; set; }

        /// <summary>
        /// 발령부서 ID
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 발령부서 코드
        /// </summary>
        public string DepartmentCode { get; set; }

        /// <summary>
        /// 발령부서명
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 발령근무지 ID
        /// </summary>
        public string WorkplaceId { get; set; }

        /// <summary>
        /// 발령근무지 코드
        /// </summary>
        public string WorkplaceCode { get; set; }

        /// <summary>
        /// 발령근무지명
        /// </summary>
        public string WorkplaceName { get; set; }

        /// <summary>
        /// 입사일자
        /// </summary>
        public DateTime? EmploymentDate { get; set; }

        /// <summary>
        /// 입사일자 UTC
        /// </summary>
        public DateTime? EmploymentDateUtc { get; set; }

        /// <summary>
        /// 퇴직일자
        /// </summary>
        public DateTime? RetirementDate { get; set; }

        /// <summary>
        /// 퇴직일자 UTC
        /// </summary>
        public DateTime? RetirementDateUtc { get; set; }

        /// <summary>
        /// 퇴직여부
        /// </summary>
        public bool IsRetirement { get; set; }

        /// <summary>
        /// Person ID
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// 내선번호
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 가상사번여부
        /// </summary>
        public bool IsExistEmployee { get; set; }

        /// <summary>
        /// 직원성명
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 직원성명(실명)
        /// </summary>
        public string RealName { get; set; } 
        #endregion

    }
}
