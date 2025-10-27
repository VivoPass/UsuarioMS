using MediatR;
using Usuarios.Application.DTOs;

namespace Usuarios.Infrastructure.Queries
{
    public class GetTodosUsuariosQuery : IRequest<List<UsuarioDTO>>
    {
    }
}
