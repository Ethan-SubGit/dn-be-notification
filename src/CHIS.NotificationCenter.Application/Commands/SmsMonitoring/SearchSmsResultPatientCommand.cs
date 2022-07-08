using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.Enum;
namespace CHIS.NotificationCenter.Application.Commands.SmsMonitoring
{
    /// <summary>
    /// ReceiveResultStatusFilter 파라미터에 대한 조건
    /// 실패	0	x.CallStatusCode != "200" || x.StatusName == "fail"
    /// 확인중	1	x.CallStatusCode == "200" && string.IsNullOrEmpty(x.StatusName)
    /// 성공	2	x.StatusName == "success"
    /// 전체	3	필터 없음
    /// </summary>
    public class SearchSmsResultPatientCommand
    {
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public string EmployeeId { get; set; }
        public string PatientId { get; set; }
        public List<string> ServiceCodeFilter { get; set; }
        public string SearchText { get; set; }
        public string SearchTelno { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public SmsResultFilterType SmsResultFilterType { get; set; } 
    }
}
