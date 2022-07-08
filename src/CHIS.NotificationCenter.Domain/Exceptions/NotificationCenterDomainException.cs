using System;
using System.Runtime.Serialization;

namespace CHIS.NotificationCenter.Domain.Exceptions
{
    [Serializable]
    public class NotificationCenterDomainException : Exception
    {
        public string MessageCode { get; set; }
        public Type MessageType { get; set; }
        public string MessageText { get; set; }
        public int TimeSpan { get; set; }

        public NotificationCenterDomainException()
        {
            MessageCode = "500";
            MessageType = InnerException?.GetType() ?? typeof(string);
            MessageText = InnerException?.Message ?? string.Empty;
            TimeSpan = 3;
        }

        public NotificationCenterDomainException(string message) : base(message)
        {
            MessageCode = "500";
            MessageType = InnerException?.GetType() ?? typeof(string);
            MessageText = message;
            TimeSpan = 3;
        }

        public NotificationCenterDomainException(string message, Exception innerException) : base(message, innerException)
        {
            MessageCode = "500";
            MessageType = InnerException?.GetType() ?? typeof(string);
            MessageText = message;
            TimeSpan = 3;
        }

        protected NotificationCenterDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
