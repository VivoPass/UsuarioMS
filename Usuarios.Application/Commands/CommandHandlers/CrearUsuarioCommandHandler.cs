using MediatR;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Application.Commands.CommandHandlers
{
    public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, string>
    {
        private readonly IUsuarioRepository UsuarioRepository;
        private readonly IRolRepository RolRepository;
        private readonly IUsuarioFactory UsuarioFactory;
        private readonly IMediator Mediator;

        public CrearUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IRolRepository rolRepository, IUsuarioFactory usuarioFactory, IMediator mediator)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            RolRepository = rolRepository ?? throw new RolRepositoryNullException();
            UsuarioFactory = usuarioFactory ?? throw new UsuarioFactoryNullException();
            Mediator = mediator ?? throw new MediatorNullException();
        }

        public async Task<string> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var rolExiste = await RolRepository.GetById(request.UsuarioDto.Rol);
                if (rolExiste == null)
                {
                    throw new IDRolNotFoundException();
                }

                var usuarioExistente = await UsuarioRepository.GetByCorreo(request.UsuarioDto.Correo);
                if (usuarioExistente != null)
                {
                    throw new CorreoUsuarioExistsException();
                }

                // Crear el usuario solo con el ID del rol
                var usuario = UsuarioFactory.Crear(
                    request.UsuarioDto.Nombre,
                    request.UsuarioDto.Apellido,
                    request.UsuarioDto.FechaNacimiento,
                    request.UsuarioDto.Correo,
                    request.UsuarioDto.Rol
                );

                await UsuarioRepository.CrearUsuario(usuario);
                /*
                // Publicar el evento de usuario creado
                var userCreatedEvent = new UserCreatedEvent(
                    user.Id.Value, user.Name.Value, user.LastName.Value, user.Email.Value, user.RoleId.Value,
                    user.Address?.Value, user.Phone?.Value
                );
                await _mediator.Publish(userCreatedEvent);
                */
                return usuario.Id.Valor;
            }
            catch (Exception ex)
            {
                throw new CrearUsuarioCommandHandlerException(ex);
            }
        }
    }
}
