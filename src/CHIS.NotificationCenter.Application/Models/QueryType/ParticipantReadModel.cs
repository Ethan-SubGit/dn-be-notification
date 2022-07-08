using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    /// <summary>
    /// participant readmodel (read model 없음)를 직접이용하지않고 
    /// encounter readmodel 참여의사 정보를 가공해서 이용
    /// </summary>
    public class ParticipantReadModel
    {
        #region property
        public int DisplaySequence { get; set; }

        /// <summary>
        /// 01: 진료의, 03: 입원 지시의, 05: 주치의, 06: 퇴원 지시의
        /// </summary>
        public string TypeCode { get; set; }
        public string PositionCode { get; set; }
        public string DepartmentId { get; set; }

        /// <summary>
        /// employeeId
        /// </summary>
        public string ActorId { get; set; }
        public string Activation { get; set; }
        public bool IsValidDataRow { get; set; }

        public string EncounterId { get; set; }

        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        /*
        참여자 구분
        01 : 진료의
        02:  응급 접수 진료의 (useless)
        03:  입원 지시의
        04 : 응급실 진료의 (useless)
        05 : 주치의
        06 : 퇴원 지시의
        07:  배정 간호사
        08 : 담당 간호사
        */
        #endregion
    }
}
