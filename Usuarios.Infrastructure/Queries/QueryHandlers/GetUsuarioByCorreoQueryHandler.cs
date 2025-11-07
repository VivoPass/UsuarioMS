using log4net;
using MediatR;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Infrastructure.Queries.QueryHandlers
{
    public class GetUsuarioByCorreoQueryHandler : IRequestHandler<GetUsuarioByCorreoQuery, UsuarioDTO>
    {
        private readonly IUsuarioRepository UsuarioRepository; private readonly ILog Logger;

        public GetUsuarioByCorreoQueryHandler(IUsuarioRepository usuarioRepository, ILog logger)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<UsuarioDTO> Handle(GetUsuarioByCorreoQuery request, CancellationToken cancellationToken)
        {
            Logger.Info($"Iniciando GetUsuarioByCorreoQuery para correo: {request.Correo}.");
            try
            {
                Logger.Debug($"Buscando usuario con correo: {request.Correo} en el repositorio.");
                var usuario = await UsuarioRepository.GetByCorreo(request.Correo);

                if (usuario == null)
                {
                    Logger.Warn($"Consulta fallida. Usuario con correo '{request.Correo}' no encontrado (CorreoUsuarioNotFoundException).");
                    throw new CorreoUsuarioNotFoundException();
                }

                Logger.Debug($"Usuario con correo '{request.Correo}' encontrado (ID: {usuario.Id.Valor}). Mapeando a DTO.");
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

                Logger.Info($"Consulta exitosa. Usuario ID {usuarioDto.Id} devuelto.");
                return usuarioDto;
            }
            catch (CorreoUsuarioNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Fallo crítico al ejecutar GetUsuarioByCorreoQuery para correo: {request.Correo}.", ex);
                throw new GetUsuarioByCorreoQueryHandlerException(ex);
            }
        }
    }
}
