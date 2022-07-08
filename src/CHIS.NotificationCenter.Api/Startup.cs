using Autofac;
using Autofac.Extensions.DependencyInjection;
using CHIS.Framework.Core;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Abstractions;
using CHIS.Framework.Core.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using CHIS.NotificationCenter.Api.Infrastructure.AutofacModules;
using CHIS.NotificationCenter.Infrastructure;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.NotificationCenter.Application.IntegrationMessages.EventHandling;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using CHIS.Share.NotificationCenter.Event;
using CHIS.NotificationCenter.Application;
using CHIS.NotificationCenter.Application.DomainEventHandlers.InboxMessageDispatchStartedEvent;

namespace CHIS.NotificationCenter.Api
{
    public class Startup : StartUpHost
    {
        public Startup(IHostingEnvironment env)
            : base(env)
        {
        }

        public IContainer ApplicationContainer { get; private set; }
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log, IApplicationLifetime alt, IDistributedCache cache)
        {
            base.UseBackgroundJobService();
            base.Configure(app, env, log, alt, cache);
        }
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            base.AddBackgroundJobService(services);
            services.AddDbContext<NotificationCenterContext>();

            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new ApplicationModule());
            container.RegisterModule(new MediatorModule());


            this.ApplicationContainer = container.Build();

            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        protected override void AddSwaggerGen(IServiceCollection services)
        {
            services.AddSwagger();
        }

        protected override void ConfigureEventBus(IEventBus eventBus)
        {
            if (eventBus == null)
            {
                return;
            }

            eventBus.Subscribe<PatientSmsMessageSendIntegrationEvent, PatientSmsMessageSendIntegrationEventHandler>();
            //eventBus.Subscribe<EmployeeSmsMessageSendIntegrationEvent, EmployeeSmsMessageSendIntegrationEventHandler>();
            eventBus.Subscribe<PatientMergingRequestedIntegrationEvent, PatientMergingRequestedIntegrationEventHandler>();
            eventBus.Subscribe<PatientMergingRollbackIntegrationEvent, PatientMergingRollbackIntegrationEventHandler>();

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnShutdown()
        {
            this.ApplicationContainer.Dispose();

            base.OnShutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        protected override void UseSwagger(IApplicationBuilder app)
        {
            app.UseSwaggerBuilder();
        }

        /// <summary>주석 표기위함</summary>
        protected override void IncludeXmlComments(SwaggerGenOptions options, params string[] files)
        {
            if (options == null)
            {
                return;
            }
            options.DescribeAllEnumsAsStrings();
            options.AddXmlComments(files);
        }
    }
}