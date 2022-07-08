using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Domain.SeedWork;
using System.Linq;
using CHIS.Framework.Core;
using CHIS.Framework.Core.Localization;
using CHIS.Framework.Middleware;

namespace CHIS.NotificationCenter.Application.Services
{
    public class UtcService : IUtcService
    {
        private readonly ITimeManager _timeManager;

        public UtcService(
            ITimeManager timeManager
            )
        {
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
        }

        public UtcPack GetUtcPack(DateTime? currentLocalDateTime)
        {
            DateTime? dateTimeUtc = null; 
            string timeZoneId = _timeManager.GetTimeZone(currentLocalDateTime).TimeZoneId;
            if (currentLocalDateTime == null)
            {
                dateTimeUtc = _timeManager.GetUTCNow();

            }
            else
            {
                dateTimeUtc = _timeManager.ConvertToUTC(currentLocalDateTime.Value);
            }
            
            return UtcPack.Create(timeZoneId, dateTimeUtc);

        }

        public DateTime GetCurrentLocalTime()
        {
            return _timeManager.GetNow();
        }
    }
}
