using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate
{
    public class MessageCallbackNoConfig : EntityBase
    {
        #region sms message callback 번호관리

        /// <summary>
        /// messageSpecification Id
        /// </summary>
        //public string MessageSpecificationId { get; set; }

        /// <summary>
        /// 구분 제목
        /// </summary>
        public string CallbackTitle { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 회신번호
        /// </summary>
        public string CallbackNo { get; set; }

        /// <summary>
        /// 삭제여부(비활성화)
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 시스템 속성(삭제 비활성)
        /// </summary>
        public bool IsSystemProperty { get; set; }

        /// <summary>
        /// 병원대표번호 여부
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// TenantId
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// HospitalId
        /// </summary>
        public string HospitalId { get; set; }
        public DateTime? DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime? DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion

        public MessageCallbackNoConfig()
        {
            //
        }

        public MessageCallbackNoConfig(string callbackTitle
            , string callbackNo, string description
            ,string tenantId, string hospitalId, DateTime? dataFirstRegisteredDateTimeUtc
            ,DateTime? dataLastModifiedDateTimeUtc
            ,Trace trace, bool? isDeleted, bool? isSystemProperty, bool? isMaster)
        {
            //MessageSpecificationId = messageSpecificationId;
            CallbackTitle = callbackTitle;
            Description = description;
            CallbackNo = callbackNo;
            IsDeleted = isDeleted ?? false;
            IsSystemProperty = isSystemProperty ?? false;
            IsMaster = isMaster ?? false;

            TenantId = tenantId;
            HospitalId = hospitalId;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
            Trace = trace;
            
        }
    }
}
