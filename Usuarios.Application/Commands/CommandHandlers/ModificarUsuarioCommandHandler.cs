using MediatR;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;

namespace Usuarios.Application.Commands.CommandHandlers
{
    public class ModificarUsuarioCommandHandler : IRequestHandler<ModificarUsuarioCommand, bool>
    {
        private readonly IUsuarioRepository UsuarioRepository;
        private readonly IMediator Mediator;

        public ModificarUsuarioCommandHandler(IUsuarioRepository usuarioRepository, IMediator mediator)
        {
            UsuarioRepository = usuarioRepository ?? throw new UsuarioRepositoryNullException();
            Mediator = mediator ?? throw new MediatorNullException();
        }

        public async Task<bool> Handle(ModificarUsuarioCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var usuario = await UsuarioRepository.GetById(request.id);

                if (usuario == null)
                {
                    throw new IDUsuarioNotFoundException();
                }

                usuario.Modificar(
                    new string(request.UsuarioDto.Nombre),
                    new string(request.UsuarioDto.Apellido)
                );

                await UsuarioRepository.ModificarUsuario(usuario);

                /*
                var userUpdatedEvent = new UserUpdatedEvent(user.Id, user.Name, user.LastName, user.Address, user.Phone);
                await Mediator.Publish(userUpdatedEvent);

                await Mediator.Publish(new UserActivityMadeEvent(user.Id.Value, "USER_UPDATED", DateTime.UtcNow));
                */

                return true;
            }
            catch (Exception ex)
            {
                throw new ModificarUsuarioCommandHandlerException(ex);
            }
        }
    }
}
