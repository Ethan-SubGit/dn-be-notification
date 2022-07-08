using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects
{
    public class WhereTrace : ValueObjectBase
    {
        public string IpAddress { get; private set; }
        public string ViewId { get; private set; }
        public string MethodName { get; private set; }

        public WhereTrace()
        {

        }

        public WhereTrace(string ipAddress, string viewId, string methodName)
        {
            IpAddress = ipAddress;
            ViewId = viewId;
            MethodName = methodName;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return IpAddress;
            yield return ViewId;
            yield return MethodName;
        }
    }
}
