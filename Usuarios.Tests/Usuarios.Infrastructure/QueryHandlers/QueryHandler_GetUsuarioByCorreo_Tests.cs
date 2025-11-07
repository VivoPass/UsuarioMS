using log4net;
using Moq;
using System.Reflection.Metadata;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Queries;
using Usuarios.Infrastructure.Queries.QueryHandlers;

namespace Usuarios.Tests.Usuarios.Infrastructure.QueryHandlers
{
    public class QueryHandler_GetUsuarioByCorreo_Tests
    {
        private readonly Mock<IUsuarioRepository> MockUsuarioRepository;
        private readonly Mock<ILog> MockLogger;
        private readonly GetUsuarioByCorreoQueryHandler Handler;

        // --- DATOS ---
        private readonly string ExistingEmail;
        private readonly Usuario ExistingUsuario;
        private readonly GetUsuarioByCorreoQuery ValidQuery;

        public QueryHandler_GetUsuarioByCorreo_Tests()
        {
            MockUsuarioRepository = new Mock<IUsuarioRepository>();
            MockLogger = new Mock<ILog>();
            Handler = new GetUsuarioByCorreoQueryHandler(MockUsuarioRepository.Object, MockLogger.Object);

            // --- DATOS ---
            // Datos del Usuario
            ExistingEmail = "usuario.valido@sistema.com";
            var rolClienteId = new VORolId(Guid.NewGuid().ToString());

            // Entidad Usuario Existente (Simulación del output del repositorio)
            ExistingUsuario = new Usuario(
                id: new VOId(Guid.NewGuid().ToString()),
                nombre: new VONombre("Simulacro"),
                apellido: new VOApellido("Testing"),
                fechaNacimiento: new VOFechaNacimiento(new DateOnly(1988, 3, 15)),
                correo: new VOCorreo(ExistingEmail),
                telefono: new VOTelefono("987654321"),
                direccion: new VODireccion("Direccion Prueba"),
                fotoPerfil: new VOFotoPerfil("foto.jpg"),
                rol: rolClienteId
            );
            // Query de Entrada Válida
            ValidQuery = new GetUsuarioByCorreoQuery(ExistingEmail);
        }

        #region Handle_ExistingCorreo_ShouldReturnUsuarioDTO()
        [Fact]
        public async Task Handle_ExistingCorreo_ShouldReturnUsuarioDTO()
        {
            // ARRANGE
            MockUsuarioRepository.Setup(r => r.GetByCorreo(ValidQuery.Correo)).ReturnsAsync(ExistingUsuario);

            // ACT
            var resultDto = await Handler.Handle(ValidQuery, CancellationToken.None);

            // ASSERT
            Assert.Equal(ExistingUsuario.Id.Valor.ToString(), resultDto.Id);
        }
        #endregion

        #region Handle_NonExistingCorreo_ShouldThrowCorreoUsuarioNotFoundException()
        [Fact]
        public async Task Handle_NonExistingCorreo_ShouldThrowCorreoUsuarioNotFoundException()
        {
            // ARRANGE
            var nonExistentCorreo = "no.existe@dominio.com";
            var invalidQuery = new GetUsuarioByCorreoQuery(nonExistentCorreo);

            MockUsuarioRepository.Setup(r => r.GetByCorreo(nonExistentCorreo)).ReturnsAsync((Usuario)null);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<CorreoUsuarioNotFoundException>(() => Handler.Handle(invalidQuery, CancellationToken.None));
        }
        #endregion

        #region Handle_RepositoryFails_ShouldThrowGetUsuarioByCorreoQueryHandlerException()
        [Fact]
        public async Task Handle_RepositoryFails_ShouldThrowGetUsuarioByCorreoQueryHandlerException()
        {
            // ARRANGE
            var dbException = new InvalidOperationException("Simulated database connection failure.");

            MockUsuarioRepository
                .Setup(r => r.GetByCorreo(It.IsAny<string>())).ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<GetUsuarioByCorreoQueryHandlerException>(() => Handler.Handle(ValidQuery, CancellationToken.None));
        }
        #endregion
    }
}
