using MassTransit;
using MediatR;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Application.Events.EventHandlers
{
    public class HistorialActividadEventHandler : INotificationHandler<HistorialActividadEvent>
    {
        private readonly ISendEndpointProvider PublishEndpoint;

        public HistorialActividadEventHandler(ISendEndpointProvider publishEndpoint)
        {
            PublishEndpoint = publishEndpoint ?? throw new SendEndpointProviderNullException();
        }

        public async Task Handle(HistorialActividadEvent userCreatedEvent, CancellationToken cancellationToken)
        {
            try
            {
                await PublishEndpoint.Send(userCreatedEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new HistorialActividadEventHandlerException(ex);
            }
        }
    }
}
