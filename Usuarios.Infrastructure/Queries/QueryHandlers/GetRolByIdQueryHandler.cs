using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetRolByIdQueryHandler : IRequestHandler<GetRolByIdQuery, RolDTO>
    {
        private readonly IRolRepository RolRepository;

        public GetRolByIdQueryHandler(IRolRepository rolRepository)
        {
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
        }

        public async Task<RolDTO> Handle(GetRolByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var rol = await RolRepository.GetById(request.Id);

                if (rol == null)
                {
                    throw new IDRolNotFoundException();
                }

                var rolDto = new RolDTO
                {
                    IdRol = rol.IdRol.Valor,
                    NombreRol = rol.NombreRol.Valor
                };

                return rolDto;
            }
            catch (Exception ex)
            {
                throw new GetRolByIdQueryHandlerException(ex);
            }
        }
    }
}
