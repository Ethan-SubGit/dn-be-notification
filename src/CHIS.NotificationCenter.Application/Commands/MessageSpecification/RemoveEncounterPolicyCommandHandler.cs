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

    public class RemoveEncounterPolicyCommandHandler : IRequestHandler<RemoveEncounterPolicyCommand, bool>
    {
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly ILoggingService _loggingService;
        // Using DI to inject infrastructure persistence Repositories
        public RemoveEncounterPolicyCommandHandler(
            IMessageSpecificationRepository messageSpecificationRepository
            , ILoggingService loggingService)
        {
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task<bool> Handle(RemoveEncounterPolicyCommand request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification messageSpecification =
                    await _messageSpecificationRepository.Retrieve(request.MessageSpecificationId).ConfigureAwait(false);

                EncounterPolicy encounterPolicy = messageSpecification.EncounterPolicies.First(x => x.Id == request.Id);


                //TO-DO : 로직 확인해볼것.
                messageSpecification.RemoveEncounterPolicy(encounterPolicy);
                //_messageSpecificationRepository.Update(messageSpecification);
                _messageSpecificationRepository.DeleteEncounterPolicy(encounterPolicy);

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
