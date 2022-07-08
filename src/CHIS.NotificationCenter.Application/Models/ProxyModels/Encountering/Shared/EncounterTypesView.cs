using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class EncounterTypesView
    {
        public List<EncounterType> EncounterTypes { get; set; } = new List<EncounterType>();

        public EncounterTypesView()
        {

        }

        public EncounterTypesView(List<EncounterType> encounterTypes)
        {
            EncounterTypes = encounterTypes;
        }
    }
}
