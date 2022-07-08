using CHIS.Framework.Core.Configuration;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Layer;
using CHIS.Framework.Layer.Fluent;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.NotificationCenterConfigurationAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Infrastructure
{
    public class NotificationCenterContext : DCLBase, IUnitOfWork
    {


        private readonly IMediator _mediator;
        private readonly IMessagingService _messagingService;
        public const string DOMAIN_NAME = "notificationcenter"; //AppSetting에서 끌고오면 EF migration할때 AppSetting을 로드 할 수 없어 에러남. 그래서 하드코딩
        private readonly string ENTITY_CONFIG_NAMESPACE = typeof(AssignedDepartmentPolicyEntityConfiguration).Namespace;

        #region DbSet
        public DbSet<MessageSpecification> MessageSpecifications { get; set; }
        public DbSet<EmployeeRecipient> EmployeeRecipients { get; set; }
        public DbSet<DepartmentPolicy> DepartmentPolicies { get; set; }
        public DbSet<EncounterPolicy> EncounterPolicies { get; set; }
        public DbSet<RecipientPolicyProtocol> RecipientPolicyProtocols { get; set; }
        public DbSet<AssignedEmployeeRecipient> AssignedEmployeeRecipients { get; set; }
        public DbSet<AssignedEncounterPolicy> AssignedEncounterPolicies { get; set; }
        //public DbSet<AssignedInstantSmsRecipient> AssignedInstantSmsRecipients { get; set; }
        public DbSet<AssignedDepartmentPolicy> AssignedDepartmentPolicies { get; set; }
        
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<MessageCallbackNoConfig> MessageCallbackNoConfigs { get; set; }

        public DbSet<EmployeeMessageBox> EmployeeMessageBoxes { get; set; }
        public DbSet<EmployeeMessageInstance> EmployeeMessageInstances { get; set; }
        public DbSet<MessageDispatchItem> MessageDispatchItems { get; set; }

        public DbSet<MessageAttachment> MessageAttachments { get; set; }

        public DbSet<ContentParameter> ContentParameters { get; set; }

        //public DbSet<Patient> Patients { get; set; }
        //public DbSet<Contact> Contacts { get; set; }
        //public DbSet<ContactTelephone> ContactTelephones { get; set; }

        //public DbSet<Department> Departments { get; set; }
        //public DbSet<Employee> Employees { get; set; }

        //public DbSet<JobPosition> JobPositions { get; set; }
        //public DbSet<JobPositionMapping> JobPositionMapping { get; set; }
        //public DbSet<Occupation> Occupations { get; set; }


        public DbSet<SmsReceiveLog> SmsReceiveLogs { get; set; }
        public DbSet<SmsSendLog> SmsSendLogs { get; set; }

        public DbSet<NotificationCenterConfiguration> NotificationCenterConfiguration { get; set; }

        //Encounter
        //public DbSet<Encounter> Encounters { get; set; }
        //public DbSet<Participant> Participants { get; set; }
        #endregion

        #region DbQuery
        public DbQuery<PatientReadModel> PatientReadModels { get; set; }
        public DbQuery<ContactReadModel> ContactReadModels { get; set; }
        public DbQuery<ContactTelephoneReadModel> ContactTelelphoneModels { get; set; }
        public DbQuery<EncounterReadModel> EncounterReadModels { get; set; }
        public DbQuery<BusinessItemReadModel> BusinessItemReadModels { get; set; }
        public DbQuery<ContactPointReadModel> ContactPointReadModels { get; set; }
        public DbQuery<DepartmentReadModel> DepartmentReadModels { get; set; }
        public DbQuery<EmployeeReadModel> EmployeeReadModel { get; set; }
        public DbQuery<ParticipantReadModel> ParticipantReadModels { get; set; }

        public DbQuery<OccupationReadModel> OccupationReadModels { get; set; }
        public DbQuery<JobPositionMappingReadModel> JobPositionMappingReadModels { get; set; }
        public DbQuery<JobPositionReadModel> JobPositionReadModels { get; set; }
        public DbQuery<MedicalRecordMergingReadModel> MedicalRecordMergingReadModels { get; set; }
        public DbQuery<LocationRoomReadModel> LocationRoomReadModels { get; set; }
        #endregion

        public NotificationCenterContext(DbContextOptions<NotificationCenterContext> options) : base(options){ }

        public static readonly LoggerFactory LoggerFactory =
        new LoggerFactory(new[] { new DebugLoggerProvider((_, __) => true) });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder == null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }
            else
            {
                base.OnConfiguring(optionsBuilder);
#if DEBUG
                optionsBuilder.UseLoggerFactory(LoggerFactory);
#endif
            }

        }

        public NotificationCenterContext(ICallContext callContext, DbContextOptions<NotificationCenterContext> options
                                                        , IMediator mediator
                                            , IMessagingService messagingService) 
            : base(callContext, options, NotificationCenterContext.DOMAIN_NAME)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        public NotificationCenterContext(DbContextOptions<NotificationCenterContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder != null)
            {
                //modelBuilder.HasDefaultSchema(DEFAULT_SCHEMA);

                //var implementedConfigTypes = Assembly.GetExecutingAssembly()
                var implementedConfigTypes = typeof(NotificationCenterContext).Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract
                            && !t.IsGenericTypeDefinition
                            && t.Namespace == ENTITY_CONFIG_NAMESPACE
                            && t.GetTypeInfo().ImplementedInterfaces.Any(i =>
                                i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));


                foreach (var configType in implementedConfigTypes)
                {
                    dynamic config = Activator.CreateInstance(configType);
                    modelBuilder.ApplyConfiguration(config);
                }

                //readmodel entityConfiguration
                var queryTypeConfigTypes = typeof(NotificationCenterContext).Assembly
                        .GetTypes()
                        .Where(t => !t.IsAbstract
                            && !t.IsGenericTypeDefinition
                            && t.Namespace == "CHIS.NotificationCenter.Infrastructure.EntityConfigurations.ReadModels"
                            && t.GetTypeInfo().ImplementedInterfaces.Any(i =>
                                i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryTypeConfiguration<>)));

                foreach (var configType in queryTypeConfigTypes)
                {
                    dynamic config = Activator.CreateInstance(configType);
                    modelBuilder.ApplyConfiguration(config);
                }
                //modelBuilder.ApplyConfiguration(new PatientReadModelQueryTypeConfiguration());
            }
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 

            await _mediator.DispatchDomainEventsAsync(this).ConfigureAwait(false);
            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed throught the DbContext will be commited
            await base.SaveChangesAsync().ConfigureAwait(false);

            return true;
        }
        public async Task<bool> SaveEntitiesWithMessagingAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this).ConfigureAwait(false);
            bool success = false;
            success = await this._messagingService.ProcessPreparedMessageWithPublisherTaskAsync<bool>(
            this,
            async () =>
            {
                await base.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }).ConfigureAwait(false);
            if (success)
            {
                await this._messagingService.SendPreparedMessagesAsync(DOMAIN_NAME).ConfigureAwait(false);
            }
            return success;
        }

    }

    public class NotificationCenterContextDesignFactory : IDesignTimeDbContextFactory<NotificationCenterContext>
    {
        public NotificationCenterContext CreateDbContext(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["notificationcenter"];
            var optionsBuilder = new DbContextOptionsBuilder<NotificationCenterContext>()
                .UseDatabase(connectionString, x => x.MigrationsHistoryTable("notificationcenter__reqmigrationshistory"));

            return new NotificationCenterContext(optionsBuilder.Options);
        }
    }

}
