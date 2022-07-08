using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.SeedWork
{
    public class UtcPack : ValueObjectBase
    {
        #region [Property]
        /// <summary>
        /// TimeZoneId info
        /// </summary>
        public string TimeZoneId { get; set; }
        /// <summary>
        /// DateTimeUtc info
        /// </summary>
        public DateTime? DateTimeUtc { get; set; }
        #endregion

        #region [Constructor]
        /// <summary>
        /// UtcPack Constructor
        /// </summary>
        protected UtcPack() { }
        /// <summary>
        /// UtcPack Constructor
        /// </summary>
        public UtcPack(string timeZoneId, DateTime? dateTimeUtc)
        {
            TimeZoneId = timeZoneId;
            DateTimeUtc = dateTimeUtc;
        }
        #endregion

        #region[Event]  
        /// <summary>
        /// Init
        /// </summary>
        public static UtcPack Init()
        {
            return new UtcPack(string.Empty, null);
        }
        /// <summary>
        /// create
        /// </summary>
        public static UtcPack Create(string timeZoneId
                                 , DateTime? dateTimeUtc)
        {
            return new UtcPack(timeZoneId, dateTimeUtc);
        }
        /// <summary>
        /// Copay Of
        /// </summary>
        /// <returns></returns>
        public UtcPack CopyOf()
        {
            return new UtcPack(TimeZoneId, DateTimeUtc);
        }

        /// <summary>
        /// GetAtomicValues
        /// </summary>
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TimeZoneId;
            yield return DateTimeUtc;
        }
        #endregion
    }
}
