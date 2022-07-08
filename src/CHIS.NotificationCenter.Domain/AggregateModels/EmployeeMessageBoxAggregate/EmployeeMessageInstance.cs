using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate
{
    public class EmployeeMessageInstance : EntityBase
    {
        #region property

        public bool IsInbound { get; set; }
        public bool IsHandled { get; set; }
        public DateTime? HandleTime { get; set; }
        public UtcPack HandleTimeUtcPack { get; set; }

        public bool IsReaded { get; set; }
        public DateTime? ReadTime { get; set; }
        public UtcPack ReadTimeUtcPack { get; set; }

        public string MessageDispatchItemId { get; set; }
        public string EmployeeMessageBoxId { get; set; }
        /// <summary>
        /// Performance Tunning을 위한 attribute
        /// </summary>
        public string EmployeeId { get; set; }
        /// <summary>
        /// Performance Tunning을 위한 attribute
        /// </summary>
        public DateTime? SentTimeStamp { get; set; }
        public string Title { get; set; } // 삭제예정
        public string Content { get; set; }
        /// <summary>
        /// Performance Tunning을 위한 attribute
        /// </summary>
        public string ServiceCode { get; set; }

        public string TenantId { get; set; }
        public string HospitalId { get; set; }

        //20191113 신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion

        public EmployeeMessageInstance()
        {

        }

    }
}
