using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects
{
    public class WhoTrace : ValueObjectBase
    {
        public string EmployeeId { get; private set; }
        public string EmployeeDisplayId { get; private set; }
        public string EmployeeName { get; private set; }

        public WhoTrace()
        {

        }

        public WhoTrace(string employeeId, string employeeDisplayId, string employeeName)
        {
            EmployeeId = employeeId;
            EmployeeDisplayId = employeeDisplayId;
            EmployeeName = employeeName;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return EmployeeId;
            yield return EmployeeDisplayId;
            yield return EmployeeName;
        }
    }
}
