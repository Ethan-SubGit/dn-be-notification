using Autofac;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Abstractions;
using CHIS.NotificationCenter.Application.IntegrationMessages.EventHandling;
using CHIS.NotificationCenter.Application.Proxies;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Infrastructure.Queries;
using CHIS.NotificationCenter.Infrastructure.Repositories;
using System.Reflection;

namespace CHIS.NotificationCenter.Api.Infrastructure.AutofacModules
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //Query 클래스들 
           
            builder.RegisterAssemblyTypes(typeof(EmployeeMessageBoxQueries).GetTypeInfo().Assembly)
                .Where(i => i.Namespace == typeof(EmployeeMessageBoxQueries).Namespace)
                .AsImplementedInterfaces().InstancePerLifetimeScope();
            
            //builder.RegisterType<EmployeeMessageBoxQueries>().As<IEmployeeMessageBoxQueries>().InstancePerLifetimeScope();
            //builder.RegisterType<MessageSpecificationQueries>().As<IMessageSpecificationQueries>().InstancePerLifetimeScope();
            //builder.RegisterType<MessageDispatcherQueries>().As<IMessageDispatcherQueries>().InstancePerLifetimeScope();

            //Repository 클래스들
            builder.RegisterAssemblyTypes(typeof(MessageSpecificationRepository).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRepository<>)).InstancePerLifetimeScope();

            //▼ RabbitMQ 쓸때 필수적인 코드블럭
            //builder.RegisterAssemblyTypes(typeof(EmployeeRegisteredIntegrationEventHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

            builder.RegisterAssemblyTypes(typeof(PatientMergingRequestedIntegrationEventHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
            //▲ RabbitMQ 쓸때 필수적인 코드블럭

            //Proxies
            //builder.RegisterType<EncounteringProxy>().As<IEncounteringProxy>().InstancePerLifetimeScope();
            builder.RegisterType<DutySchedulingAssignedNursesProxy>().As<IDutySchedulingAssignedNursesProxy>().InstancePerLifetimeScope();
            builder.RegisterType<HospitalBuilderProxy>().As<IHospitalBuilderProxy>().InstancePerLifetimeScope();
            builder.RegisterType<SmsSendProxy>().As<ISmsSendProxy>().InstancePerLifetimeScope();
            builder.RegisterType<AccessControlProxy>().As<IAccessControlProxy>().InstancePerLifetimeScope();
            builder.RegisterType<MergingPatientProxy>().As<IMergingPatientProxy>().InstancePerLifetimeScope();

            //Services
            builder.RegisterType<OneSignalInterfaceService>().As<IOneSignalInterfaceService>().InstancePerLifetimeScope();
            builder.RegisterType<SmsService>().As<ISmsService>().InstancePerLifetimeScope();
            builder.RegisterType<PatientMergingService>().As<IPatientMergingService>().InstancePerLifetimeScope();
            builder.RegisterType<UtcService>().As<IUtcService>().InstancePerLifetimeScope();
            builder.RegisterType<CHIS.Share.AuditTrail.Services.LoggingService>().As<CHIS.Share.AuditTrail.Services.ILoggingService>().InstancePerLifetimeScope();
            ////Sms Sender
            //builder.RegisterType<CHIS.Share.NotificationCenter.Services.SmsSendService>().As<CHIS.Share.NotificationCenter.Services.ISmsSendService>().InstancePerLifetimeScope();
        }
    }
}
