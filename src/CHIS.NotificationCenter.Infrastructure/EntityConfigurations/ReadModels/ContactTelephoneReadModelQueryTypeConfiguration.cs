using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations.ReadModels
{
    public class ContactTelephoneReadModelQueryTypeConfiguration : IQueryTypeConfiguration<ContactTelephoneReadModel>
    {
        /// <summary>
        ///  Configure 
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(QueryTypeBuilder<ContactTelephoneReadModel> builder)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            builder.ToView(ReadModelConstants.Domains.PatientDemographic + "_" + ReadModelConstants.DomainTables.ContactTelephone);
        }
    }
}
