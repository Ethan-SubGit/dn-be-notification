using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations.ReadModels
{
    public class PatientReadModelQueryTypeConfiguration : IQueryTypeConfiguration<PatientReadModel>
    {
        /// <summary>
        ///  Configure 
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(QueryTypeBuilder<PatientReadModel> builder)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            builder.ToView(ReadModelConstants.Domains.PatientDemographic + "_" + nameof(PatientReadModel));
        }
    }
}
