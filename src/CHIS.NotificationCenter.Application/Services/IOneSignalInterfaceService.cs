using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Services
{
    public interface IOneSignalInterfaceService
    {
        Task<string> SendMessage(string title, string contents, Dictionary<string, string> data, List<string> employeeList);
    }
}
