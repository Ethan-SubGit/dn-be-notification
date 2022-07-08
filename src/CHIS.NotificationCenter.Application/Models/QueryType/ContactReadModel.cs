using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    /// <summary>
    /// patient contact
    /// </summary>
    public class ContactReadModel
    {
        #region property
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Demographicid { get; set; }

        /// <summary>
        /// 순번
        /// </summary>
        public int DisplaySequence { get; set; }

        /// <summary>
        /// 연락처관계코드
        /// </summary>
        public string RelationshipCode { get; set; }

        /// <summary>
        /// 연락처관계 사람이름
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 순위구분
        /// </summary>
        public string ClassificationCode { get; set; }

        /// <summary>
        /// 주연락처
        /// </summary>
        public bool Isprimary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HospitalId { get; set; }

        public string TenantId { get; set; }

        //public bool isValidDataRow { get; set; }

        /// <summary>
        /// 사용하지않음
        /// </summary>
        //public string Telephone { get; set; } 
        #endregion

    }
}
