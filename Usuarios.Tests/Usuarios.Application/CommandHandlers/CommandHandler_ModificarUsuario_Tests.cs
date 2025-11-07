using MediatR;
using Moq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Usuarios.Application.Commands;
using Usuarios.Application.Commands.CommandHandlers;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Usuarios.Tests.Usuarios.Application.CommandHandlers
{
    public class CommandHandler_ModificarUsuario_Tests
    {
        private readonly Mock<IUsuarioRepository> MockUsuarioRepo;
        private readonly Mock<ILog> MockLog;
        private readonly ModificarUsuarioCommandHandler Handler;

        // --- DATOS ---
        private readonly Guid ExistingUserId;
        private readonly Usuario ExistingUser;
        private readonly ModificarUsuarioCommand ValidCommand;
        private readonly string NewNombre = "NuevoNombre";
        private readonly string NewApellido = "NuevoApellido";
        private readonly string NewTelefono = "12345678901";
        private readonly string NewDireccion = "Nueva Direccion 123";
        private readonly string NewFotoPerfil = "nueva_foto.jpg";

        public CommandHandler_ModificarUsuario_Tests()
        {
            MockUsuarioRepo = new Mock<IUsuarioRepository>();
            MockLog = new Mock<ILog>();
            Handler = new ModificarUsuarioCommandHandler(MockUsuarioRepo.Object, MockLog.Object);

            // --- DATOS ---

            // ID del usuario que siempre existirá
            ExistingUserId = Guid.NewGuid();

            // Entidad Usuario EXISTENTE
            ExistingUser = new Usuario(
                id: new VOId(ExistingUserId.ToString()),
                nombre: new VONombre("Original"),
                apellido: new VOApellido("Usuario"),
                fechaNacimiento: new VOFechaNacimiento(new DateOnly(1990, 1, 1)),
                correo: new VOCorreo("user@exists.com"),
                telefono: new VOTelefono("1234567890"),
                direccion: new VODireccion("Direccion Original"),
                fotoPerfil: new VOFotoPerfil("foto_original.jpg"),
                rol: new VORolId(Guid.NewGuid().ToString())
            );

            // Comando de Entrada Válido
            ValidCommand = new ModificarUsuarioCommand(
                ExistingUserId.ToString(),
                new ModificarUsuarioDTO
                {
                    Nombre = NewNombre,
                    Apellido = NewApellido,
                    Telefono = NewTelefono,
                    Direccion = NewDireccion,
                    FotoPerfil = NewFotoPerfil

                }
            );
        }

        #region Handle_ValidRequest_ShouldCallRepositoryAndUpdateUserAndReturnTrue()
        [Fact]
        public async Task Handle_ValidRequest_ShouldCallRepositoryAndUpdateUserAndReturnTrue()
        {
            // ARRANGE
            MockUsuarioRepo.Setup(r => r.GetById(ValidCommand.id)).ReturnsAsync(ExistingUser);

            MockUsuarioRepo.Setup(r => r.ModificarUsuario(ExistingUser)).Returns(Task.CompletedTask);

            // ACT
            var result = await Handler.Handle(ValidCommand, CancellationToken.None);

            // ASSERT
            Assert.True(result);
        }
        #endregion

        #region Handle_UserNotFound_ShouldThrowIDUsuarioNotFoundException()
        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowIDUsuarioNotFoundException()
        {
            // ARRANGE
            var nonExistentId = Guid.NewGuid().ToString();

            var command = new ModificarUsuarioCommand(
                nonExistentId,
                new ModificarUsuarioDTO { Nombre = "Cualquiera", Apellido = "Cualquiera" }
            );

            MockUsuarioRepo.Setup(r => r.GetById(nonExistentId)).ReturnsAsync((Usuario)null);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<IDUsuarioNotFoundException>(() => Handler.Handle(command, CancellationToken.None));
        }
        #endregion

        #region Handle_UnexpectedError_ShouldThrowModificarUsuarioCommandHandlerException()
        [Fact]
        public async Task Handle_UnexpectedError_ShouldThrowModificarUsuarioCommandHandlerException()
        {
            // ARRANGE
            MockUsuarioRepo.Setup(r => r.GetById(ValidCommand.id)).ReturnsAsync(ExistingUser);

            var persistenceException = new InvalidOperationException("Simulated unexpected database error.");

            MockUsuarioRepo
                .Setup(r => r.ModificarUsuario(ExistingUser)).ThrowsAsync(persistenceException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<ModificarUsuarioCommandHandlerException>(
                () => Handler.Handle(ValidCommand, CancellationToken.None));
        }
        #endregion
    }
}
