using CHIS.NotificationCenter.Domain.AggregateModels.Shared.Enums;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects
{
    public class WhatTrace : ValueObjectBase
    {
        public WhatEvent Event { get; private set; }

        public WhatTrace()
        {

        }

        public WhatTrace(WhatEvent whateEvent)
        {
            Event = whateEvent;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Event;
        }
    }
}
