using MediatR;
using Usuarios.Application.DTOs;

namespace Usuarios.Infrastructure.Queries
{
    public class GetUsuarioByIdQuery : IRequest<UsuarioDTO>
    {
        public string IdUsuario { get; set; }

        public GetUsuarioByIdQuery(string idUsuario)
        {
            IdUsuario = idUsuario;
        }
    }
}
