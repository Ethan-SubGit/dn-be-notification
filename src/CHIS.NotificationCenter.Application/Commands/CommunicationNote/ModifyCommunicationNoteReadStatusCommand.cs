using System.Collections.Generic;
using System.Text;
using MediatR;

namespace CHIS.NotificationCenter.Application.Commands.CommunicationNote
{
    public class ModifyCommunicationNoteReadStatusCommand : IRequest<bool>
    {
        public string MessageInstanceId { get; set; }

    }
}
