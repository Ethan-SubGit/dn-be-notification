using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Commands.PatientInformation
{
    public class SearchPatientContactsCommnad
    {
        public List<string> PatientIds { get; set; }

        public SearchPatientContactsCommnad()
        {
            PatientIds = new List<string>();
        }
    }
}
