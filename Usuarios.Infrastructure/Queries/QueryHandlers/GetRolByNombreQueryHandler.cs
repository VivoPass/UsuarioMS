using MediatR;
using log4net;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetRolByNombreQueryHandler : IRequestHandler<GetRolByNombreQuery, RolDTO>
    {
        private readonly IRolRepository RolRepository; private readonly ILog Logger;

        public GetRolByNombreQueryHandler(IRolRepository rolRepository, ILog logger)
        {
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<RolDTO> Handle(GetRolByNombreQuery request, CancellationToken cancellationToken)
        {
            Logger.Info($"Iniciando GetRolByNombreQuery para Nombre: '{request.Nombre}'.");
            try
            {
                Logger.Debug($"Buscando rol con nombre: '{request.Nombre}' en el repositorio.");
                var rol = await RolRepository.GetByNombre(request.Nombre);

                if (rol == null)
                {
                    Logger.Warn($"Consulta fallida. Rol con nombre '{request.Nombre}' no encontrado (NombreRolNotFoundException).");
                    throw new NombreRolNotFoundException();
                }

                Logger.Debug($"Rol con nombre '{request.Nombre}' encontrado (ID: {rol.IdRol.Valor}). Mapeando a DTO.");
                var rolDto = new RolDTO
                {
                    IdRol = rol.IdRol.Valor,
                    NombreRol = rol.NombreRol.Valor
                };

                Logger.Info($"Consulta exitosa. Rol ID {rol.IdRol.Valor} devuelto.");
                return rolDto;
            }
            catch (NombreRolNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Fallo crítico al ejecutar GetRolByNombreQuery para Nombre: '{request.Nombre}'.", ex);
                throw new GetRolByNombreQueryHandlerException(ex);
            }
        }
    }
}
