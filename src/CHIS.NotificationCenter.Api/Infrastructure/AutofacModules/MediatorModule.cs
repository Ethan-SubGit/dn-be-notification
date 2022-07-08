using Autofac;
//using CHIS.NotificationCenter.Application.Behaviors;
using CHIS.NotificationCenter.Application.Commands.MessageSpecification;
using CHIS.NotificationCenter.Application.Commands.MessageDispatcher;
using CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.DomainEventHandlers.InboxMessageDispatchStartedEvent;
//using CHIS.NotificationCenter.Application.DomainEventHandlers.SmsMessageDispatchStartedEvent;
//using CHIS.NotificationCenter.Application.Commands.Contract;
//using CHIS.NotificationCenter.Application.Commands.Tenant;
//using CHIS.NotificationCenter.Application.Validations.Certification;
//using CHIS.NotificationCenter.Application.Validations.Contract;
//using CHIS.NotificationCenter.Application.Validations.Tenant;

using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Reflection;

namespace CHIS.NotificationCenter.Api.Infrastructure.AutofacModules
{
    /// <summary>
    /// 
    /// </summary>
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            // Register all the Command classes (they implement IRequestHandler) in assembly holding the Commands
            builder.RegisterAssemblyTypes(typeof(RegisterMessageSpecificationCommand).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
            //builder.RegisterAssemblyTypes(typeof(RequestSmsMessageNotificationCommand).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
            //builder.RegisterAssemblyTypes(typeof(RequestSmsMessageNotificationCommand).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
            //builder.RegisterAssemblyTypes(typeof(RegisterMessageExclusionFilterCommand).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(IRequestHandler<,>));
            //IRequestHandler<RequestSmsMessageNotificationCommand, bool>

            /*등록하면 두번실행됨.*/
            //builder.RegisterAssemblyTypes(typeof(InboxMessageDispatchStartedDomainEventHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(INotificationHandler<>));


            builder.RegisterAssemblyTypes(typeof(RequestPatientSmsMessageNotificationCommandHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(INotificationHandler<>));

            //builder.RegisterAssemblyTypes(typeof(InboxMessageDispatchStartedDomainEventHandler).GetTypeInfo().Assembly).AsClosedTypesOf(typeof(INotificationHandler<>));

            //builder.RegisterAssemblyTypes(typeof(SmsMessageDispatchStartedDomainEventHandler).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IAsyncNotificationHandler<>));
            //builder.RegisterAssemblyTypes(typeof(RegisterIndividualRecipientCommand).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register all the event classes (they implement IAsyncNotificationHandler) in assembly holding the Commands
            //builder.RegisterAssemblyTypes(typeof(OrderCodeDomainEventHandler).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IAsyncNotificationHandler<>));
            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => componentContext.Resolve(t);
            });
            
            //builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}
