using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.SeedWork
{
    public class MergingPatientGround : ValueObjectBase
    {
        public string PatientMergingPrimaryId { get; set; }
        public string ClosingPatientId { get; set; }
        public string MergingPatientId { get; set; }
        public string WorkerEmployeeId { get; set; }
        public DateTime WorkDateTime { get; set; }
        public UtcPack WorkDateTimeUtcPack { get; set; }

        public MergingPatientGround()
        { }

        public MergingPatientGround(string patientMergingPrimaryId,
                                    string closingPatientId,
                                    string mergingPatientId,
                                    string workerEmployeeId,
                                    DateTime workDateTime,
                                    UtcPack workDateTimeUtcPack)
        {
            PatientMergingPrimaryId = patientMergingPrimaryId;
            ClosingPatientId = closingPatientId;
            MergingPatientId = mergingPatientId;
            WorkerEmployeeId = workerEmployeeId;
            WorkDateTime = workDateTime;
            WorkDateTimeUtcPack = workDateTimeUtcPack;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return PatientMergingPrimaryId;
            yield return ClosingPatientId;
            yield return MergingPatientId;
            yield return WorkerEmployeeId;
            yield return WorkDateTime;
            yield return WorkDateTimeUtcPack;
        }
    }
}
