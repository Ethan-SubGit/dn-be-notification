using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations.ReadModels
{
    public class EncounterReadModelQueryTypeConfiguration : IQueryTypeConfiguration<EncounterReadModel>
    {
        /// <summary>
        ///  Configure 
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(QueryTypeBuilder<EncounterReadModel> builder)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            builder.ToView(ReadModelConstants.Domains.Encountering + "_" + nameof(EncounterReadModel));
        }
    }
}
