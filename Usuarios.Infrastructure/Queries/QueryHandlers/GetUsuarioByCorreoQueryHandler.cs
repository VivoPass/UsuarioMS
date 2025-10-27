using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetUsuarioByCorreoQueryHandler : IRequestHandler<GetUsuarioByCorreoQuery, UsuarioDTO>
    {
        private readonly IUsuarioRepository UsuarioRepository;

        public GetUsuarioByCorreoQueryHandler(IUsuarioRepository usuarioRepository)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
        }

        public async Task<UsuarioDTO> Handle(GetUsuarioByCorreoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await UsuarioRepository.GetByCorreo(request.Correo);

                if (usuario == null)
                {
                    throw new CorreoUsuarioNotFoundException();
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
                throw new GetUsuarioByCorreoQueryHandlerException(ex);
            }
        }
    }
}
