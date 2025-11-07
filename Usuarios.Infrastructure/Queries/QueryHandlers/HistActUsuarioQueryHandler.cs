using log4net;
using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class HistActUsuarioQueryHandler : IRequestHandler<HistActUsuarioQuery, IEnumerable<HistActUsuarioDTO>>
    {
        private readonly IUsuarioHistorialActividad UsuarioHistActRepository; private readonly ILog Logger;

        public HistActUsuarioQueryHandler(IUsuarioHistorialActividad usuarioHistActRepository, ILog logger)
        {
            UsuarioHistActRepository = usuarioHistActRepository ?? throw new HistActRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<IEnumerable<HistActUsuarioDTO>> Handle(HistActUsuarioQuery request, CancellationToken cancellationToken)
        {
            Logger.Info($"Iniciando HistActUsuarioQuery para Usuario ID: {request.IdUsuario}");
            try
            {
                Logger.Debug($"Consultando el historial de actividad del repositorio para ID {request.IdUsuario}.");
                var activities = await UsuarioHistActRepository.GetByIdUsuarioHistAct(request.IdUsuario, DateTime.Today);

                if (activities == null || !activities.Any())
                {
                    Logger.Info($"Consulta exitosa. No se encontraron actividades para el Usuario ID {request.IdUsuario}. Devolviendo lista vacía.");
                    return new List<HistActUsuarioDTO>();
                }

                var count = activities.Count;
                Logger.Debug($"Recuperadas {count} actividades para el Usuario ID {request.IdUsuario}. Iniciando mapeo a DTO.");
                var result = activities.ConvertAll(activity => new HistActUsuarioDTO
                {
                    Id = activity["_id"].AsString,
                    IdUsuario = activity["_idUsuario"].AsString,
                    Accion = activity["accion"].AsString,
                    Timestamp = activity["timestamp"].AsLocalTime
                });

                Logger.Info($"Consulta exitosa. Se devolvió una lista de {result.Count} registros de actividad para Usuario ID {request.IdUsuario}.");
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"Fallo crítico al ejecutar HistActUsuarioQuery para Usuario ID: {request.IdUsuario}.", ex);
                throw new HistActUsuarioQueryHandlerException(ex);
            }
        }
    }
}
