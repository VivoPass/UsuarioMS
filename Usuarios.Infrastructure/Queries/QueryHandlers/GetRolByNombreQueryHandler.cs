using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Infrastructure.Persistences.Repositories.MongoDB;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetRolByNombreQueryHandler : IRequestHandler<GetRolByNombreQuery, RolDTO>
    {
        private readonly IRolRepository RolRepository;

        public GetRolByNombreQueryHandler(IRolRepository rolRepository)
        {
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
        }

        public async Task<RolDTO> Handle(GetRolByNombreQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var rol = await RolRepository.GetByNombre(request.Nombre);

                if (rol == null)
                {
                    throw new NombreRolNotFoundException();
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
                throw new GetRolByNombreQueryHandlerException(ex);
            }
        }
    }
}
