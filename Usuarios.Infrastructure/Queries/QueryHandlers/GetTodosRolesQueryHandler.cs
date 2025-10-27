using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetTodosRolesQueryHandler : IRequestHandler<GetTodosRolesQuery, List<RolDTO>>
    {
        private readonly IRolRepository RolRepository;

        public GetTodosRolesQueryHandler(IRolRepository rolRepository)
        {
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
        }

        public async Task<List<RolDTO>> Handle(GetTodosRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var rol = await RolRepository.GetTodos();

                if (rol == null || !rol.Any())
                {
                    return new List<RolDTO>();
                }

                var roles = rol.Select(u => new RolDTO
                {
                    IdRol = u.IdRol.Valor,
                    NombreRol = u.NombreRol.Valor
                }).ToList();

                return roles;
            }
            catch (Exception ex)
            {
                throw new GetTodosRolesQueryHandlerException(ex);
            }
        }
    }
}
