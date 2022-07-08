using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.SeedWork;

namespace CHIS.NotificationCenter.Application.Services
{
    public interface IUtcService
    {
        UtcPack GetUtcPack(DateTime? currentLocalDateTime);
        DateTime GetCurrentLocalTime();
    }

}
