using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    public class SearchMessageCountCommand
    {
        public string EmployeeId { get; set; }
       
        public string PatientId { get; set; }
        public string SearchText { get; set; }
        public string PeriodFilter { get; set; } //1주전,2주전,한달전,6개월전,1년전,기간검색 {1W,2W,1M,6M,1Y,RG(Range),NA(없음)}
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public bool IsImportant { get; set; }
        public bool IsUnimportant { get; set; }
        public List<string> FilterByServiceCodes { get; set; } //선택한 분류 메시지만 필터링
        public string DepartmentId { get; set; } // 담당환자만 필터링
    }
}
