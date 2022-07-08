using System.Collections.Generic;
using System.Text;
using MediatR;
using CHIS.NotificationCenter.Domain.Enum;
using System;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{

    public class RegisterMessageTemplateCommand : IRequest<string>
    {
        #region property
        /// <summary>
        /// messageSpecification 참조키
        /// </summary>
        public string MessageSpecificationId { get; set; }

        /// <summary>
        /// 공용/개별 컨텐츠 여부
        /// </summary>
        public ContentTemplateScopeType ContentTemplateScope { get; set; }

        /// <summary>
        /// 제목
        /// </summary>
        public string TemplateTitle { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string ContentTemplate { get; set; }

        /// <summary>
        /// 삭제여부
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 등록직원
        /// </summary>
        //public string EmployeeId { get; set; }

        ///// <summary>
        ///// TenanatId
        ///// </summary>
        //public string TenantId { get; set; }

        ///// <summary>
        ///// HospitalId
        ///// </summary>
        //public string HospitalId { get; set; }

        ///// <summary>
        ///// 데이타입력 시간
        ///// </summary>
        //public DateTime DataFirstRegisteredDateTimeUtc { get; set; }

        ///// <summary>
        ///// 데이타 수정 시간
        ///// </summary>
        //public DateTime DataLastModifiedDateTimeUtc { get; set; }

        ///// <summary>
        ///// 변경 Data Trace
        ///// </summary>
        //public Trace Trace { get; set; } 
        #endregion

        public RegisterMessageTemplateCommand()
        {
            //
        }

       
    }

}