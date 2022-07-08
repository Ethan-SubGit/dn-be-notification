using CHIS.Framework.Core;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Application.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Infrastructure.Queries
{
    public class EncounterParticipantQueries : DALBase, IEncounterParticipantQueries
    {
        private readonly ICallContext _callContext;
        //private readonly ITimeManager _timeManager;
        private readonly NotificationCenterContext _notificationCenterContext;

        public EncounterParticipantQueries(ICallContext callContext
            //, ITimeManager timeManager
            , NotificationCenterContext notificationCenterContext) : base(callContext)
        {
            this.DBCatalog = NotificationCenterContext.DOMAIN_NAME;
            _notificationCenterContext = notificationCenterContext;
            //_timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));


        }

        /// <summary>
        /// encounter paticipant List 조회
        /// </summary>
        /// <param name="encounterId"></param>
        /// <returns></returns>
        public async Task<List<ParticipantReadModel>> RetrievesEncounterPaticipantList(string encounterId)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            EncounterReadModel encounterParticipantRow = await
                                    _notificationCenterContext.Query<EncounterReadModel>()
                                    .Where(p => p.Id == encounterId
                                        && p.HospitalId == _callContext.HospitalId
                                        && p.TenantId == _callContext.TenantId
                                    ).FirstOrDefaultAsync().ConfigureAwait(false);

            List<ParticipantReadModel> participantList = new List<ParticipantReadModel>();
            #region ## staff 조회, List add
            if (encounterParticipantRow != null)
            {
                //입원지시의(03)
                if (encounterParticipantRow.AdmittingPhysicianId != null)
                {
                    participantList.Add(new ParticipantReadModel()
                    {
                        TypeCode = "E03",
                        ActorId = encounterParticipantRow.AdmittingPhysicianId,
                        EncounterId = encounterId,
                        TenantId = tenantId,
                        HospitalId = hospitalId
                    });
                }

                //진료의(01)
                if (encounterParticipantRow.AttendingPhysicianId != null)
                {
                    participantList.Add(new ParticipantReadModel()
                    {
                        TypeCode = "E01",
                        ActorId = encounterParticipantRow.AttendingPhysicianId,
                        EncounterId = encounterId,
                        TenantId = tenantId,
                        HospitalId = hospitalId
                    });
                }

                //주치의(05)
                if (encounterParticipantRow.PrimaryCarePhysicianId != null)
                {
                    participantList.Add(new ParticipantReadModel()
                    {
                        TypeCode = "E05",
                        ActorId = encounterParticipantRow.PrimaryCarePhysicianId,
                        EncounterId = encounterId,
                        TenantId = tenantId,
                        HospitalId = hospitalId
                    });
                }

                //퇴원지시의(06)
                if (encounterParticipantRow.DischargeOrdererId != null)
                {
                    participantList.Add(new ParticipantReadModel()
                    {
                        TypeCode = "E06",
                        ActorId = encounterParticipantRow.DischargeOrdererId,
                        EncounterId = encounterId,
                        TenantId = tenantId,
                        HospitalId = hospitalId
                    });
                }
            }
            #endregion

            return participantList;
        }
    }
}
