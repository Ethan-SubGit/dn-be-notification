using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate
{
    /// <summary>
    /// TO-DO : Encounter Policy 는 Patient Admin에서 코드조회를 통한 로직으로 변경 필요.
    ///         Department Policy 만 여기서 정의해서 사용필요. 
    ///         Tenant Master로 구성살것.
    /// </summary>
    public class RecipientPolicyProtocol
    {

        #region property
        public string PolicyCode { get; set; }

        public RecipientPolicyType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        //public string ResolvingGroupCode { get; set; } // 삭제 예정

        public string TenantId { get; set; }
        //public string HospitalId { get; set; } // 삭제 예정 
        //신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion

        public RecipientPolicyProtocol()
        { }

    }
}
