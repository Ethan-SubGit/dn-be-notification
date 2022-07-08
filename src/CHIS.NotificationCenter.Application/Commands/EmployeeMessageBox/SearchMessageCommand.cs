using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    /// <summary>
    /// 인박스 검색을 위한 Search Commamd
    /// </summary>
    public class SearchMessageCommand
    {
        public string EmployeeId { get; set; }
        public string MessageCategory { get; set; } // R (리뷰) ,D (문서) ,M (메시지)
        public string PatientId { get; set; }
        public string SearchText { get; set; }
        public string PeriodFilter { get; set; } //1주전,2주전,한달전,6개월전,1년전,기간검색 {1W,2W,1M,6M,1Y,RG(Range),NA(없음)}
        public int HandleStatusFilter { get; set; } // 0:미확인 , 1:확인 , 2:전체
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string ExclusionMessageInstanceId { get; set; }
        public List<string> FilterByServiceCodes { get; set; } //선택한 분류 메시지만 필터링
        public string DepartmentId { get; set; } // 담당환자만 필터링
    }
}
