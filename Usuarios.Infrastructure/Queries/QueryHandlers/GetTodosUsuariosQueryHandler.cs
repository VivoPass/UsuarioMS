using MediatR;
using log4net;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetTodosUsuariosQueryHandler : IRequestHandler<GetTodosUsuariosQuery, List<UsuarioDTO>>
    {
        private readonly IUsuarioRepository UsuarioRepository; private readonly ILog Logger;

        public GetTodosUsuariosQueryHandler(IUsuarioRepository usuarioRepository, ILog logger)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<List<UsuarioDTO>> Handle(GetTodosUsuariosQuery request, CancellationToken cancellationToken)
        {
            Logger.Info("Iniciando GetTodosUsuariosQuery: Solicitud de la lista completa de Usuarios.");
            try
            {
                Logger.Debug("Consultando el repositorio para obtener todas las entidades de Usuario.");
                var usuario = await UsuarioRepository.GetTodos();

                if (usuario == null || !usuario.Any())
                {
                    Logger.Info("Consulta exitosa: No se encontraron Usuarios en la base de datos. Devolviendo lista vacía.");
                    return new List<UsuarioDTO>();
                }

                var count = usuario.Count;
                Logger.Debug($"Recuperados {count} Usuarios. Iniciando mapeo a lista de UsuarioDTO.");

                var usuarios = usuario.Select(u => new UsuarioDTO
                {
                    Id = u.Id.Valor,
                    Nombre = u.Nombre.Valor,
                    Apellido = u.Apellido.Valor,
                    FechaNacimiento = u.FechaNacimiento.Valor,
                    Correo = u.Correo.Valor,
                    Telefono = u.Telefono.Valor,
                    Direccion = u.Direccion.Valor,
                    FotoPerfil = u.FotoPerfil.Valor,
                    Rol = u.Rol.Valor
                }).ToList();

                Logger.Info($"Consulta exitosa. Se devolvió una lista de {usuarios.Count} UsuarioDTOs.");
                return usuarios;
            }
            catch (Exception ex)
            {
                Logger.Error("Fallo crítico al ejecutar GetTodosUsuariosQuery.", ex);
                throw new GetTodosUsuariosQueryHandlerException(ex);
            }
        }
    }
}
