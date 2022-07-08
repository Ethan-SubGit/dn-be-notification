using CHIS.NotificationCenter.Application.Commands.MessageSpecification;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CHIS.Share.AuditTrail.Services;
namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{

    public class RemoveDepartmentPolicyCommandHandler : IRequestHandler<RemoveDepartmentPolicyCommand, bool>
    {
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly ILoggingService _loggingService;
        // Using DI to inject infrastructure persistence Repositories
        public RemoveDepartmentPolicyCommandHandler(
            IMessageSpecificationRepository messageSpecificationRepository
            , ILoggingService loggingService)
        {
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task<bool> Handle(RemoveDepartmentPolicyCommand request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification messageSpecification = 
                    await _messageSpecificationRepository.Retrieve(request.MessageSpecificationId).ConfigureAwait(false);

                DepartmentPolicy departmentPolicy = messageSpecification.DepartmentPolicies.First(x => x.Id == request.Id);

                messageSpecification.RemoveDepartmentPolicy(departmentPolicy);
                _messageSpecificationRepository.DeleteDepartmentPolicy(departmentPolicy);

                await _loggingService.PrepareMessageWithAuditTrailAsync(
                     messageSpecification, "1.0.0", Share.AuditTrail.Enum.TypeOfActionType.Modification, Share.AuditTrail.Enum.EventCoverageType.Full).ConfigureAwait(false);


                await _messageSpecificationRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);

                return true;
            }
            else
            {
                throw new ArgumentNullException(nameof(request));
            }
        }


    }
}
