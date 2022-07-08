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


    public class RemoveEmployeeRecipientCommandHandler : IRequestHandler<RemoveEmployeeRecipientCommand, bool>
    {
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly ILoggingService _loggingService;
        // Using DI to inject infrastructure persistence Repositories
        public RemoveEmployeeRecipientCommandHandler(
            IMessageSpecificationRepository messageSpecificationRepository
            , ILoggingService loggingService)
        {
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task<bool> Handle(RemoveEmployeeRecipientCommand request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification messageSpecification = 
                    await _messageSpecificationRepository.Retrieve(request.MessageSpecificationId).ConfigureAwait(false);

                EmployeeRecipient employeeRecipient = messageSpecification.EmployeeRecipients.First(x => x.Id == request.Id);

                // TO-DO : 로직확인해볼것
                
                messageSpecification.RemoveEmployeeRecipient(employeeRecipient);

                //_messageSpecificationRepository.Update(messageSpecification);
                _messageSpecificationRepository.DeleteEmployeeRecipient(employeeRecipient);


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
