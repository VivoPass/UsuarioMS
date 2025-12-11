using log4net;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;
using Usuarios.Application.Events;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Application.Commands.CommandHandlers
{
    public class ModificarPreferenciasUsuarioCommandHandler : IRequestHandler<ModificarPreferenciasUsuarioCommand, bool>
    {
        private readonly IPublishEndpoint PublishEndpoint;
        private readonly IUsuarioRepository UsuarioRepository; private readonly ILog Logger;

        public ModificarPreferenciasUsuarioCommandHandler(IUsuarioRepository usuarioRepository, ILog logger, IPublishEndpoint publishEndpoint)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
            PublishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        }

        public async Task<bool> Handle(ModificarPreferenciasUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuarioId = request.UsuarioId.ToString();
            try
            {
                var usuario = await UsuarioRepository.GetById(request.UsuarioId);

                if (usuario == null)
                {
                    Logger.Warn($"Modificación cancelada. Usuario ID {request.UsuarioId} no encontrado en la base de datos (IDUsuarioNotFoundException).");
                    throw new IDUsuarioNotFoundException();
                }

                usuario.SetPreferencias(request.Preferencias);
                await UsuarioRepository.ActualizarPreferencias(usuario);

                //Publicacion en el historial de actividad
                var createUserActivityDto = new CrearActUsuarioDTO { IdUsuario = usuarioId, Accion = "Preferencias modificadas." };
                Logger.Debug($"Publicando evento de actividad para Usuario: {createUserActivityDto.IdUsuario}. Acción: {createUserActivityDto.Accion}.");
                await PublishEndpoint.Publish(new HistorialActividadEvent(createUserActivityDto.IdUsuario, createUserActivityDto.Accion, DateTime.UtcNow));
                Logger.Info("Evento de actividad publicado correctamente.");

                Logger.Info($"Preferencias del usuario {request.UsuarioId} actualizadas exitosamente.");
                return true;
            }
            catch (IDUsuarioNotFoundException) { throw; }
            catch (Exception ex)
            {
                Logger.Error($"Error al ejecutar ModificarPreferenciasUsuarioCommand para ID {request.UsuarioId}.", ex);
                throw new ModificarPreferenciasUsuarioCommandHandlerException(ex);
            }
        }
    }
}

