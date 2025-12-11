using MassTransit;
using log4net;
using MediatR;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Application.Commands.CommandHandlers
{
    public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, string>
    {
        private readonly IUsuarioRepository UsuarioRepository; private readonly IRolRepository RolRepository; private readonly IUsuarioFactory UsuarioFactory;
        private readonly ILog Logger;

        public CrearUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IRolRepository rolRepository, IUsuarioFactory usuarioFactory, ILog logger)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
            UsuarioFactory = usuarioFactory ?? throw new UsuarioFactoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        public async Task<string> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
        {
            Logger.Info($"Iniciando CrearUsuarioCommand para correo: {request.UsuarioDto.Correo}. Rol solicitado: {request.UsuarioDto.Rol}.");
            try
            {
                Logger.Debug($"Verificando existencia de Rol ID: {request.UsuarioDto.Rol}.");
                var rolExiste = await RolRepository.GetById(request.UsuarioDto.Rol);
                if (rolExiste == null)
                {
                    Logger.Warn($"Creación de usuario cancelada. Rol ID {request.UsuarioDto.Rol} no encontrado (IDRolNotFoundException).");
                    throw new IDRolNotFoundException();
                }
                Logger.Debug($"Rol ID {request.UsuarioDto.Rol} encontrado. Continuando con la verificación de unicidad.");

                Logger.Debug($"Verificando si el correo {request.UsuarioDto.Correo} ya existe.");
                var usuarioExistente = await UsuarioRepository.GetByCorreo(request.UsuarioDto.Correo);
                if (usuarioExistente != null)
                {
                    Logger.Warn($"Creación de usuario cancelada. El correo {request.UsuarioDto.Correo} ya existe (CorreoUsuarioExistsException).");
                    throw new CorreoUsuarioExistsException();
                }

                Logger.Debug("Invocando IUsuarioFactory para crear la nueva entidad de Usuario.");
                var usuario = UsuarioFactory.Crear(
                    request.UsuarioDto.Id,
                    request.UsuarioDto.Nombre,
                    request.UsuarioDto.Apellido,
                    request.UsuarioDto.FechaNacimiento,
                    request.UsuarioDto.Correo,
                    request.UsuarioDto.Telefono,
                    request.UsuarioDto.Direccion,
                    request.UsuarioDto.FotoPerfil,
                    request.UsuarioDto.Rol,
                    request.UsuarioDto.Preferencias
                );
                Logger.Debug($"Entidad de Usuario creada en memoria con ID generado: {usuario.Id.Valor}.");

                await UsuarioRepository.CrearUsuario(usuario);

                Logger.Info($"Usuario {usuario.Id.Valor} creado y persistido exitosamente en la base de datos.");
                return usuario.Id.Valor;
            }
            catch (IDRolNotFoundException){ throw; }
            catch (CorreoUsuarioExistsException) { throw; }

            catch (FechaNacimientoUsuarioMenorException) { throw; }
            //catch (RolKeycloakInvalido) { throw; }

            catch (Exception ex)
            {
                Logger.Error($"Fallo crítico al ejecutar CrearUsuarioCommand para correo {request.UsuarioDto.Correo}.", ex);
                throw new CrearUsuarioCommandHandlerException(ex);
            }
        }
    }
}
