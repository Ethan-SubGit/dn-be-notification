using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model
{
    public class ContactPointDto
    {
        public string PersonId { get; set; }
        public int DisplaySequence { get; set; }
        public ContactPointSystem SystemType { get; set; }
        public string ContactValue { get; set; }

        public ContactPointDto(string personId, int displaySequence, ContactPointSystem systemType, string contactValue)
        {
            this.PersonId = personId;
            this.DisplaySequence = displaySequence;
            this.SystemType = systemType;
            this.ContactValue = contactValue;
        }
    }

    public enum ContactPointSystem
    {
        Mobile = 0,
        Extension = 1,
        EmergencyCall = 2,
        Home = 3
    }
}
