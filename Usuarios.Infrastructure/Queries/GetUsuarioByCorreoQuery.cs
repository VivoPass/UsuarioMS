using MediatR;
using Usuarios.Application.DTOs;

namespace Usuarios.Infrastructure.Queries
{
    public class GetUsuarioByCorreoQuery : IRequest<UsuarioDTO>
    {
        public string Correo { get; set; }

        public GetUsuarioByCorreoQuery(string correo)
        {
            Correo = correo;
        }
    }
}
