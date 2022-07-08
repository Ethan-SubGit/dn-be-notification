using System.Collections.Generic;
using System.Linq;

namespace CHIS.NotificationCenter.Domain.SeedWork
{
    public abstract class ValueObjectBase
    {
        protected static bool EqualOperator(ValueObjectBase left, ValueObjectBase right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        protected static bool NotEqualOperator(ValueObjectBase left, ValueObjectBase right)
        {
            return !(EqualOperator(left, right));
        }

        protected abstract IEnumerable<object> GetAtomicValues();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            ValueObjectBase other = (ValueObjectBase)obj;
            IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();
            IEnumerator<object> otherValues = other.GetAtomicValues().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }

                if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }

            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public override int GetHashCode()
        {
            return GetAtomicValues()
             .Select(x => x != null ? x.GetHashCode() : 0)
             .Aggregate((x, y) => x ^ y);
        }
        public bool IsEmpty()
        {
            IEnumerator<object> thisValues = GetAtomicValues().GetEnumerator();

            while (thisValues.MoveNext())
            {
                if (thisValues.Current != null)
                {
                    return false;
                }
            }
            return true;
        }
        public ValueObjectBase GetCopy()
        {
            return this.MemberwiseClone() as ValueObjectBase;
        }
    }
}
