using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class RegisterMessageCallbackNoCommand : IRequest<string>
    {
        
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
        public bool? IsDeleted { get; set; }

        /// <summary>
        /// 시스템 속성(삭제 비활성)
        /// </summary>
        public bool? IsSystemProperty { get; set; }

        /// <summary>
        /// 병원대표번호 여부
        /// </summary>
        public bool? IsMaster { get; set; }

        public RegisterMessageCallbackNoCommand()
        {
            //
        }
    }
}
