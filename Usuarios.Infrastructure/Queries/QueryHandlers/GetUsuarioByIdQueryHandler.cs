using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetUsuarioByIdQueryHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDTO>
    {
        private readonly IUsuarioRepository UsuarioRepository;

        public GetUsuarioByIdQueryHandler(IUsuarioRepository usuarioRepository)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
        }

        public async Task<UsuarioDTO> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await UsuarioRepository.GetById(request.IdUsuario);

                if (usuario == null)
                {
                    throw new IDUsuarioNotFoundException();
                }

                var usuarioDto = new UsuarioDTO
                {
                    Id = usuario.Id.Valor,
                    Nombre = usuario.Nombre.Valor,
                    Apellido = usuario.Apellido.Valor,
                    FechaNacimiento = usuario.FechaNacimiento.Valor,
                    Correo = usuario.Correo.Valor,
                    Rol = usuario.Rol.Valor
                };

                return usuarioDto;
            }
            catch (Exception ex)
            {
                throw new GetUsuarioByIdQueryHandlerException(ex);
            }
        }
    }
}
