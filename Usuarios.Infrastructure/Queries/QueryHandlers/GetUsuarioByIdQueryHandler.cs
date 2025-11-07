using log4net;
using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetUsuarioByIdQueryHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDTO>
    {
        private readonly IUsuarioRepository UsuarioRepository; private readonly ILog Logger;

        public GetUsuarioByIdQueryHandler(IUsuarioRepository usuarioRepository, ILog logger)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<UsuarioDTO> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
        {
            Logger.Info($"Iniciando GetUsuarioByIdQuery para ID: {request.IdUsuario}.");
            try
            {
                Logger.Debug($"Buscando usuario con ID: {request.IdUsuario} en el repositorio.");
                var usuario = await UsuarioRepository.GetById(request.IdUsuario);

                if (usuario == null)
                {
                    Logger.Warn($"Consulta fallida. Usuario ID {request.IdUsuario} no encontrado (IDUsuarioNotFoundException).");
                    throw new IDUsuarioNotFoundException();
                }

                Logger.Debug($"Usuario ID {request.IdUsuario} encontrado. Mapeando a DTO.");
                var usuarioDto = new UsuarioDTO
                {
                    Id = usuario.Id.Valor,
                    Nombre = usuario.Nombre.Valor,
                    Apellido = usuario.Apellido.Valor,
                    FechaNacimiento = usuario.FechaNacimiento.Valor,
                    Correo = usuario.Correo.Valor,
                    Telefono = usuario.Telefono.Valor,
                    Direccion = usuario.Direccion.Valor,
                    FotoPerfil = usuario.FotoPerfil.Valor,
                    Rol = usuario.Rol.Valor
                };

                Logger.Info($"Consulta exitosa. Usuario ID {usuarioDto.Id} ('{usuarioDto.Correo}') devuelto.");
                return usuarioDto;
            }
            catch (IDUsuarioNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Fallo crítico al ejecutar GetUsuarioByIdQuery para ID: {request.IdUsuario}.", ex);
                throw new GetUsuarioByIdQueryHandlerException(ex);
            }
        }
    }
}
