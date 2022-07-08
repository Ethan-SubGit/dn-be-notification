using CHIS.Framework.Core;
using CHIS.Framework.Data.ORM;
using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Queries.ReadModels.PatientInformation;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.Share.MedicalAge;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Infrastructure.Queries
{
    public class MedicalRecordMergingQueries : DALBase, IMedicalRecordMergingQueries
    {
        private readonly ICallContext _callContext;
        //private readonly ITimeManager _timeManager;
        private readonly NotificationCenterContext _notificationCenterContext;

        public MedicalRecordMergingQueries(ICallContext callContext
            , ITimeManager timeManager
            , NotificationCenterContext notificationCenterContext) : base(callContext)
        {
            this.DBCatalog = NotificationCenterContext.DOMAIN_NAME;
            _notificationCenterContext = notificationCenterContext;
            //_timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
          
        }

        public async Task<MedicalRecordMergingReadModel> FindPatientMergingResultAsync(string primaryId, string classificationId)
        {
            //var mergingRow = _notificationCenterContext.Query<MedicalRecordMergingReadModel>()
            //var row = _notificationCenterContext.Query<MedicalRecordMergingReadModel>().
            //MedicalRecordMergingReadModel row = _notificationCenterContext.

            var row = await _notificationCenterContext.Query<MedicalRecordMergingReadModel>()
               .Where(p => p.PatientMergingPrimaryId == primaryId
                   && p.TenantId == _callContext.TenantId
                   && p.HospitalId == _callContext.HospitalId
                   && p.TargetClassificationId == classificationId).FirstOrDefaultAsync().ConfigureAwait(false);

            MedicalRecordMergingReadModel merginReadModel = new MedicalRecordMergingReadModel(targetClassificationId: row.TargetClassificationId, mergingDate: row.MergingDate
                , domainName: row.DomainName
                , isCompleted: row.IsCompleted
                , errorContent: row.ErrorContent
                , mergeResult: row.MergeResult
                , tenantId: row.TenantId
                , hospitalId: row.HospitalId
                , patientMergingPrimaryId: row.PatientMergingPrimaryId);


            return merginReadModel;
        }
    }
}
