using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class BusinessItemReadModel
    {
        #region property
        public string Id { get; set; }

        public int DisplaySequence { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string BusinessTypeCode { get; set; } 
        #endregion
    }
}
