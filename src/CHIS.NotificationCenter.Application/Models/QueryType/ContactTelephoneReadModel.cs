using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    /// <summary>
    /// patient contact telephone
    /// </summary>
    public class ContactTelephoneReadModel
    {
        #region property
        public string Id { get; set; }
        public string ContactId { get; set; }

        public int DisplaySequence { get; set; }

        public string ClassificationCode { get; set; }

        public string PhoneNumber { get; set; } 
        #endregion
    }
}
