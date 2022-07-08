using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate
{
    public class MessageAttachment : EntityBase
    {
        public string   ContentType     { get; set; }
        public string   Extension       { get; set; }
        public string   FileKey             { get; set; }
        public string   OriginalFileName{ get; set; }
        public string   SavedFileName   { get; set; }
        public string   SavedFilePath   { get; set; }
        public int FileSize { get; set; }
        public string   Url             { get; set; }

        public string   MessageDispatchItemId { get; set; }

        public string TenantId { get; set; }
        public string HospitalId { get; set; }

        //20191113 신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; }
    }
}
