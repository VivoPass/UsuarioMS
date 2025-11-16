using MediatR;
using log4net;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetTodosRolesQueryHandler : IRequestHandler<GetTodosRolesQuery, List<RolDTO>>
    {
        private readonly IRolRepository RolRepository; private readonly ILog Logger;

        public GetTodosRolesQueryHandler(IRolRepository rolRepository, ILog logger)
        {
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<List<RolDTO>> Handle(GetTodosRolesQuery request, CancellationToken cancellationToken)
        {
            Logger.Info("Iniciando GetTodosRolesQuery: Solicitud de la lista completa de Roles.");
            try
            {
                Logger.Debug("Consultando el repositorio para obtener todas las entidades de Rol.");
                var rol = await RolRepository.GetTodos();

                if (rol == null || !rol.Any())
                {
                    Logger.Info("Consulta exitosa: No se encontraron Roles en la base de datos. Devolviendo lista vacía.");
                    return new List<RolDTO>();
                }

                var count = rol.Count;
                Logger.Debug($"Recuperados {count} Roles. Iniciando mapeo a lista de RolDTO.");

                var roles = rol.Select(u => new RolDTO
                {
                    IdRol = u.IdRol.Valor,
                    NombreRol = u.NombreRol.Valor,
                    IdRolKeycloak = u.RolKeycloakId.Valor
                }).ToList();

                Logger.Info($"Consulta exitosa. Se devolvió una lista de {roles.Count} RolDTOs.");
                return roles;
            }
            catch (Exception ex)
            {
                Logger.Error("Fallo crítico al ejecutar GetTodosRolesQuery.", ex);
                throw new GetTodosRolesQueryHandlerException(ex);
            }
        }
    }
}
