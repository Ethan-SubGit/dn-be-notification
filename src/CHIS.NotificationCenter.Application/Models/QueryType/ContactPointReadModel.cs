using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    /// <summary>
    /// employee contact
    /// </summary>
    public class ContactPointReadModel
    {
        #region property
        public string PersonId { get; set; }
        public int DisplaySequence { get; set; }
        public ContactPointSystem SystemType { get; set; }
        public string ContactValue { get; set; }
        public string TenantId { get; set; }
        #endregion
    }

    public enum ContactPointSystem
    {
        Mobile = 0,
        Extension = 1,
        EmergencyCall = 2,
        Home = 3
    }
}
