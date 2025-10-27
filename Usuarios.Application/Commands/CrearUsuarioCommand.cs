using MediatR;

using Usuarios.Application.DTOs;

namespace Usuarios.Application.Commands
{
    public class CrearUsuarioCommand : IRequest<String>
    {
        public CrearUsuarioDTO UsuarioDto { get; }

        public CrearUsuarioCommand(CrearUsuarioDTO usuarioDto)
        {
            UsuarioDto = usuarioDto ?? throw new ArgumentNullException(nameof(usuarioDto));
        }
    }
}
