using MediatR;
using log4net;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Application.Commands.CommandHandlers
{
    public class ModificarUsuarioCommandHandler : IRequestHandler<ModificarUsuarioCommand, bool>
    {
        private readonly IUsuarioRepository UsuarioRepository; private readonly ILog Logger;

        public ModificarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, ILog logger)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<bool> Handle(ModificarUsuarioCommand request, CancellationToken cancellationToken)
        {
            Logger.Info($"Iniciando ModificarUsuarioCommand para ID: {request.id}. Datos" +
                        $" recibidos: Nombre={request.UsuarioDto.Nombre}, Teléfono={request.UsuarioDto.Telefono}.");
            try
            {
                Logger.Debug($"Buscando usuario con ID: {request.id} en el repositorio.");
                var usuario = await UsuarioRepository.GetById(request.id);

                if (usuario == null)
                {
                    Logger.Warn($"Modificación cancelada. Usuario ID {request.id} no encontrado en la base de datos (IDUsuarioNotFoundException).");
                    throw new IDUsuarioNotFoundException();
                }

                Logger.Debug($"Usuario ID {request.id} encontrado. Mutando entidad con nuevos datos.");
                usuario.Modificar(
                    new string(request.UsuarioDto.Nombre),
                    new string(request.UsuarioDto.Apellido),
                    new string(request.UsuarioDto.Telefono),
                    new string(request.UsuarioDto.Direccion),
                    new string(request.UsuarioDto.FotoPerfil)
                );

                Logger.Debug($"Entidad Usuario ID {request.id} modificada en memoria.");
                await UsuarioRepository.ModificarUsuario(usuario);

                Logger.Info($"Usuario ID {request.id} modificado y persistido exitosamente.");
                return true;
            }
            catch (IDUsuarioNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Fallo crítico al ejecutar ModificarUsuarioCommand para ID: {request.id}.", ex);
                throw new ModificarUsuarioCommandHandlerException(ex);
            }
        }
    }
}
