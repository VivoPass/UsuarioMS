using log4net;
using MediatR;
using Moq;
using Usuarios.Application.Commands;
using Usuarios.Application.Commands.CommandHandlers;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Tests.Usuarios.Application.CommandHandlers
{
    public class CommandHandler_CrearUsuario_Tests
    {
        private readonly Mock<IUsuarioRepository> MockUsuarioRepo;
        private readonly Mock<IRolRepository> MockRolRepo;
        private readonly Mock<IUsuarioFactory> MockUsuarioFactory;
        private readonly Mock<ILog> MockLog;
        private readonly CrearUsuarioCommandHandler Handler;

        // --- DATOS ---
        private readonly VORolId mockRolId;
        private readonly Rol rolExistente;
        private readonly string expectedId;
        private readonly CrearUsuarioCommand command;
        private readonly Usuario factoryOutput;

        public CommandHandler_CrearUsuario_Tests()
        {
            MockUsuarioRepo = new Mock<IUsuarioRepository>();
            MockRolRepo = new Mock<IRolRepository>();
            MockUsuarioFactory = new Mock<IUsuarioFactory>();
            MockLog = new Mock<ILog>();
            Handler = new CrearUsuarioCommandHandler(MockUsuarioRepo.Object, MockRolRepo.Object, MockUsuarioFactory.Object, MockLog.Object);

            // --- DATOS ---
            mockRolId = new VORolId(Guid.NewGuid().ToString());
            var mockRolNombre = new VORolNombre("Cliente Simulado");
            var mockRolKeycloakId = new VORolKeycloakId("Cliente_Simulado");
            rolExistente = new Rol(mockRolId, mockRolNombre, mockRolKeycloakId);

            expectedId = Guid.NewGuid().ToString();
            command = new CrearUsuarioCommand(new CrearUsuarioDTO
            {
                Nombre = "Test",
                Apellido = "User",
                Correo = "valid@test.com",
                FechaNacimiento = new DateOnly(2002, 10, 14),
                Telefono = "1234567890",
                Direccion = "123 Test St",
                FotoPerfil = "http://example.com/profile.jpg",
                Rol = "usuario_final"
            });

            factoryOutput = new Usuario(
                id: new VOId(expectedId),
                nombre: new VONombre(command.UsuarioDto.Nombre),
                apellido: new VOApellido(command.UsuarioDto.Apellido),
                fechaNacimiento: new VOFechaNacimiento(command.UsuarioDto.FechaNacimiento),
                correo: new VOCorreo(command.UsuarioDto.Correo),
                telefono: new VOTelefono(command.UsuarioDto.Telefono),
                direccion: new VODireccion(command.UsuarioDto.Direccion),
                fotoPerfil: new VOFotoPerfil(command.UsuarioDto.FotoPerfil),
                rol: new VORolKeycloakId(command.UsuarioDto.Rol)
            );
        }

        #region Handle_ValidRequest_ShouldCreateUserAndReturnId()
        [Fact]
        public async Task Handle_ValidRequest_ShouldCreateUserAndReturnId()
        {

            MockRolRepo.Setup(r => r.GetById(It.IsAny<string>())).ReturnsAsync(rolExistente);

            MockUsuarioRepo.Setup(u => u.GetByCorreo(It.IsAny<string>())).ReturnsAsync((Usuario)null);

            MockUsuarioFactory
                .Setup(f => f.Crear(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                    It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>()))
                .Returns(factoryOutput);

            MockUsuarioRepo.Setup(u => u.CrearUsuario(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

            // ACT
            var resultId = await Handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.Equal(expectedId.ToString(), resultId);
        }
        #endregion

        #region Handle_RolDoesNotExist_ShouldThrowIDRolNotFoundException()
        [Fact]
        public async Task Handle_RolDoesNotExist_ShouldThrowIDRolNotFoundException()
        {
            MockRolRepo.Setup(r => r.GetById(It.IsAny<string>())).ReturnsAsync((Rol)null);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<IDRolNotFoundException>(() => Handler.Handle(command, CancellationToken.None));
        }
        #endregion

        #region Handle_UserAlreadyExists_ShouldThrowCorreoUsuarioExistsException()
        [Fact]
        public async Task Handle_UserAlreadyExists_ShouldThrowCorreoUsuarioExistsException()
        {
            // ARRANGE
            var duplicateEmailCommand = new CrearUsuarioCommand(new CrearUsuarioDTO
            {
                Correo = command.UsuarioDto.Correo,
                Rol = mockRolId.Valor
            });

            MockRolRepo.Setup(r => r.GetById(It.IsAny<string>())).ReturnsAsync(rolExistente);

            MockUsuarioRepo.Setup(u => u.GetByCorreo(duplicateEmailCommand.UsuarioDto.Correo)).ReturnsAsync(factoryOutput);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<CorreoUsuarioExistsException>(() => Handler.Handle(command, CancellationToken.None));
        }
        #endregion

        #region Handle_PersistenceFails_ShouldThrowCrearUsuarioCommandHandlerException()
        [Fact]
        public async Task Handle_PersistenceFails_ShouldThrowCrearUsuarioCommandHandlerException()
        {
            // ARRANGE
            MockRolRepo.Setup(r => r.GetById(It.IsAny<string>())).ReturnsAsync(rolExistente);
            MockUsuarioRepo.Setup(u => u.GetByCorreo(It.IsAny<string>())).ReturnsAsync((Usuario)null);

            MockUsuarioFactory
                .Setup(f => f.Crear(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateOnly>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(factoryOutput);

            var dbException = new InvalidOperationException("Simulated DB connection error.");
            MockUsuarioRepo.Setup(u => u.CrearUsuario(It.IsAny<Usuario>())).ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<CrearUsuarioCommandHandlerException>(() => Handler.Handle(command, CancellationToken.None));
        }
        #endregion

    }
}
