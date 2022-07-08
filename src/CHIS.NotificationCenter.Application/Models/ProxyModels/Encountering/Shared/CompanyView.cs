using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared
{
    public class CompanyView
    {
        public string Id { get; set; }
        public string DisplayCode { get; set; }
        public string Name { get; set; }
        public BusinessItemRdo Classification { get; set; }
        public string ClassificationCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Boolean IsValidDataRow { get; set; }

        public CompanyView()
        {

        }

        public CompanyView(string id, string displayCode, string name, BusinessItemRdo classification, string classificationCode, DateTime? startDate, DateTime? endDate, bool isValidDataRow)
        {
            Id = id;
            DisplayCode = displayCode;
            Name = name;
            Classification = classification;
            ClassificationCode = classificationCode;
            StartDate = startDate;
            EndDate = endDate;
            IsValidDataRow = isValidDataRow;
        }
    }
}
