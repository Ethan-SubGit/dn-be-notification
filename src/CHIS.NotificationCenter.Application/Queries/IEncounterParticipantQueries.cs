using CHIS.NotificationCenter.Application.Models.QueryType;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Queries
{
    public interface IEncounterParticipantQueries
    {
        Task<List<ParticipantReadModel>> RetrievesEncounterPaticipantList(string encounterId);
    }
}
