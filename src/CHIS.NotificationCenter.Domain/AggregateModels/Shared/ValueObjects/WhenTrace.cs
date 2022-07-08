using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects
{
    public class WhenTrace : ValueObjectBase
    {
        public DateTime DateTime { get; private set; }
        public string TimeZoneId { get; private set; }
        public DateTime DateTimeUtc { get; private set; }

        public WhenTrace()
        {

        }

        public WhenTrace(DateTime dateTime, string timeZoneId, DateTime dateTimeUtc)
        {
            DateTime = dateTime;
            TimeZoneId = timeZoneId;
            DateTimeUtc = dateTimeUtc;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return DateTime;
            yield return TimeZoneId;
            yield return DateTimeUtc;
        }
    }
}
