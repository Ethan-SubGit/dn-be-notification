using CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Proxies
{
    public interface IEncounteringProxy
    {
        List<EncounterBasicView> GetEncounterBasic(string patientId, string encounterId);
    }
}
