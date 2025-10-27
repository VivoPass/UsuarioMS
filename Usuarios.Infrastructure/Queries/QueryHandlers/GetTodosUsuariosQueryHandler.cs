using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetTodosUsuariosQueryHandler : IRequestHandler<GetTodosUsuariosQuery, List<UsuarioDTO>>
    {
        private readonly IUsuarioRepository UsuarioRepository;

        public GetTodosUsuariosQueryHandler(IUsuarioRepository usuarioRepository)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
        }

        public async Task<List<UsuarioDTO>> Handle(GetTodosUsuariosQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await UsuarioRepository.GetTodos();

                if (usuario == null || !usuario.Any())
                {
                    return new List<UsuarioDTO>();
                }

                var usuarios = usuario.Select(u => new UsuarioDTO
                {
                    Id = u.Id.Valor,
                    Nombre = u.Nombre.Valor,
                    Apellido = u.Apellido.Valor,
                    FechaNacimiento = u.FechaNacimiento.Valor,
                    Correo = u.Correo.Valor,
                    Rol = u.Rol.Valor
                }).ToList();

                return usuarios;
            }
            catch (Exception ex)
            {
                throw new GetTodosUsuariosQueryHandlerException(ex);
            }
        }
    }
}
