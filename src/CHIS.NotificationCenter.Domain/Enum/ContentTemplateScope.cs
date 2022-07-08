using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.Enum
{
    /// <summary>
    /// message template 범위. (공용, 개인)
    /// 삭제방지 처리 필요함.
    /// </summary>
    public enum ContentTemplateScopeType
    {
        Common, //삭제방지 (사용권한 체크필요)
        Personal //UD 자유로움
    }
}
