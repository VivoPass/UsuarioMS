using MassTransit;
using MongoDB.Bson;
using Usuarios.Application.Events;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Infrastructure.Consumers
{
    public class HistActConsumer(IUsuarioHistorialActividad usuarioHistorialActividad) : IConsumer<HistorialActividadEvent>
    {
        private readonly IUsuarioHistorialActividad UsuarioHistorialActividad = usuarioHistorialActividad ?? 
            throw new HistActRepositoryNullException();

        public async Task Consume(ConsumeContext<HistorialActividadEvent> @event)
        {
            try
            {
                var message = @event.Message;

                var bsonUser = new BsonDocument
                {
                    { "_id", Guid.NewGuid().ToString() },
                    { "_idUsuario", message.UsuarioId },
                    { "accion", message.Accion },
                    { "timestamp", message.Timestamp }
                };

                await UsuarioHistorialActividad.AgregarHistAct(bsonUser);
            }
            catch (Exception ex)
            {
                throw new HistActConsumerException(ex);
            }
        }
    }
}
