using MediatR;
using System;
using System.Collections.Generic;

namespace CHIS.NotificationCenter.Domain.SeedWork
{
    public abstract class EntityBase
    {
        private int? _requestedHashCode { get; set; }
        string _Id;
        public virtual string Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        private List<INotification> _domainEvents;
        
        public List<INotification> DomainEvents => _domainEvents;

        protected EntityBase()
        {
            _Id = Guid.NewGuid().ToString();
        }

        protected EntityBase(string id)
        {
            _Id = id;
        }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }
        public void RemoveDomainEvent(INotification eventItem)
        {
            if (_domainEvents is null)
            {
                return;
            }

            _domainEvents.Remove(eventItem);
        }

        public bool IsTransient()
        {
            return this.Id == default(string);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is EntityBase))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            EntityBase item = (EntityBase)obj;

            if (item.IsTransient() || this.IsTransient())
            {
                return false;
            }
            else
            {
                return item.Id == this.Id;
            }
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                {
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                }

                return _requestedHashCode.Value;
            }
            else
            {
                return new Object().GetHashCode();
            }
        }

        //public static bool operator ==(EntityBase left, EntityBase right)
        //{
        //    if (Object.Equals(left, null))
        //    {
        //        return (Object.Equals(right, null));
        //    }
        //    else
        //    {
        //        return left.Equals(right);
        //    }
        //}

        //public static bool operator !=(EntityBase left, EntityBase right)
        //{
        //    return !(left == right);
        //}
    }
}
