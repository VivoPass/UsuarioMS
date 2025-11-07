using MediatR;
using log4net;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetRolByIdQueryHandler : IRequestHandler<GetRolByIdQuery, RolDTO>
    {
        private readonly IRolRepository RolRepository; private readonly ILog Logger;

        public GetRolByIdQueryHandler(IRolRepository rolRepository, ILog logger)
        {
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<RolDTO> Handle(GetRolByIdQuery request, CancellationToken cancellationToken)
        {
            Logger.Info($"Iniciando GetRolByIdQuery para ID: {request.Id}.");
            try
            {
                Logger.Debug($"Buscando rol con ID: {request.Id} en el repositorio.");
                var rol = await RolRepository.GetById(request.Id);

                if (rol == null)
                {
                    Logger.Warn($"Consulta fallida. Rol ID {request.Id} no encontrado (IDRolNotFoundException).");
                    throw new IDRolNotFoundException();
                }
                Logger.Debug($"Rol ID {request.Id} encontrado. Mapeando a DTO.");

                var rolDto = new RolDTO
                {
                    IdRol = rol.IdRol.Valor,
                    NombreRol = rol.NombreRol.Valor
                };

                Logger.Info($"Consulta exitosa. Rol ID {request.Id} ('{rolDto.NombreRol}') devuelto.");
                return rolDto;
            }
            catch (IDRolNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Fallo crítico al ejecutar GetRolByIdQuery para ID: {request.Id}.", ex);
                throw new GetRolByIdQueryHandlerException(ex);
            }
        }
    }
}
