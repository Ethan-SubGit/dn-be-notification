using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class PatientReadModel 
    {
        #region property
        /// <summary>
        /// PatientId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// DemographicId
        /// </summary>
        public string DemographicId { get; set; }

        /// <summary>
        /// 환자등록번호
        /// </summary>
        public string PatientDisplayId { get; set; }

        /// <summary>
        /// 환자이름
        /// </summary>
        public string PatientFullName { get; set; }

        /// <summary>
        /// 성별코드
        /// </summary>
        public string GenderCode { get; set; }

        /// <summary>
        /// 생년월일
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SmsTelephoneNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? isClosed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsDeath { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DeathDatetime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HospitalId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsPersonalInfo { get; set; }

        /// <summary>
        /// sms 수신동의여부
        /// </summary>
        public bool? IsSmsReceptionRejection { get; set; }


        #endregion
    }
}
