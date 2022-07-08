using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects
{
    public class Trace : ValueObjectBase
    {
        public WhoTrace Who { get; private set; }
        public WhenTrace When { get; private set; }
        public WhereTrace Where { get; private set; }
        public WhatTrace What { get; private set; }

        public Trace()
        {

        }

        public Trace(WhoTrace who, WhenTrace when, WhereTrace where, WhatTrace what)
        {
            Who = who;
            When = when;
            Where = where;
            What = what;
        }


        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Who;
            yield return When;
            yield return Where;
            yield return What;
        }
    }
}
