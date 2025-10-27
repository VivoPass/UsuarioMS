using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class HistActUsuarioQueryHandler : IRequestHandler<HistActUsuarioQuery, IEnumerable<HistActUsuarioDTO>>
    {
        private readonly IUsuarioHistorialActividad UsuarioHistActRepository;

        public HistActUsuarioQueryHandler(IUsuarioHistorialActividad usuarioHistActRepository)
        {
            UsuarioHistActRepository = usuarioHistActRepository ?? throw new HistActRepositoryNullException();
        }

        public async Task<IEnumerable<HistActUsuarioDTO>> Handle(HistActUsuarioQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activities = await UsuarioHistActRepository.GetByIdUsuarioHistAct(request.IdUsuario, DateTime.Today);

                if (activities == null || !activities.Any())
                {
                    return new List<HistActUsuarioDTO>();
                }

                var result = activities.ConvertAll(activity => new HistActUsuarioDTO
                {
                    Id = activity["_id"].AsString,
                    IdUsuario = activity["_idUsuario"].AsString,
                    Accion = activity["accion"].AsString,
                    Timestamp = activity["timestamp"].AsLocalTime
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new HistActUsuarioQueryHandlerException(ex);
            }
        }
    }
}
