using log4net;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Queries;
using Usuarios.Infrastructure.Queries.QueryHandlers;

namespace Usuarios.Tests.Usuarios.Infrastructure.QueryHandlers
{
    public class QueryHandler_GetUsuarioById_Tests
    {
        private readonly Mock<IUsuarioRepository> MockUsuarioRepository;
        private readonly Mock<ILog> MockLogger;
        private readonly GetUsuarioByIdQueryHandler Handler;

        // --- DATOS ---
        private readonly string ExistingUserIdString;
        private readonly VOId ExistingUserIdVO;
        private readonly Usuario ExistingUser;
        private readonly GetUsuarioByIdQuery ValidQuery;

        public QueryHandler_GetUsuarioById_Tests()
        {
            MockUsuarioRepository = new Mock<IUsuarioRepository>();
            MockLogger = new Mock<ILog>();
            Handler = new GetUsuarioByIdQueryHandler(MockUsuarioRepository.Object, MockLogger.Object);

            // --- DATOS ---
            ExistingUserIdString = Guid.NewGuid().ToString();
            ExistingUserIdVO = new VOId(ExistingUserIdString);

            ExistingUser = new Usuario(
                id: ExistingUserIdVO,
                nombre: new VONombre("Carlos"),
                apellido: new VOApellido("Rodríguez"),
                fechaNacimiento: new VOFechaNacimiento(new DateOnly(1995, 3, 1)),
                correo: new VOCorreo("carlos.r@test.com"),
                telefono: new VOTelefono("5551234"),
                direccion: new VODireccion("Av. Principal 456"),
                fotoPerfil: new VOFotoPerfil("foto_carlos.jpg"),
                rol: new VORolId(Guid.NewGuid().ToString())
            );

            ValidQuery = new GetUsuarioByIdQuery(ExistingUserIdString);
        }

        #region Handle_ExistingId_ShouldReturnUsuarioDTO()
        [Fact]
        public async Task Handle_ExistingId_ShouldReturnUsuarioDTO()
        {
            // ARRANGE
            MockUsuarioRepository.Setup(r => r.GetById(ValidQuery.IdUsuario)).ReturnsAsync(ExistingUser);

            // ACT
            var resultDto = await Handler.Handle(ValidQuery, CancellationToken.None);

            // ASSERT
            Assert.Equal(ExistingUserIdString, resultDto.Id);
        }
        #endregion

        #region Handle_NonExistingId_ShouldThrowIDUsuarioNotFoundException()
        [Fact]
        public async Task Handle_NonExistingId_ShouldThrowIDUsuarioNotFoundException()
        {
            // ARRANGE
            var nonExistentId = Guid.NewGuid().ToString();
            var invalidQuery = new GetUsuarioByIdQuery(nonExistentId);

            MockUsuarioRepository.Setup(r => r.GetById(nonExistentId)).ReturnsAsync((Usuario)null);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<IDUsuarioNotFoundException>(
                () => Handler.Handle(invalidQuery, CancellationToken.None));
        }
        #endregion

        #region Handle_RepositoryFails_ShouldThrowGetUsuarioByIdQueryHandlerException()
        [Fact]
        public async Task Handle_RepositoryFails_ShouldThrowGetUsuarioByIdQueryHandlerException()
        {
            // ARRANGE
            var dbException = new InvalidOperationException("Simulated database connection failure.");

            MockUsuarioRepository
                .Setup(r => r.GetById(It.IsAny<string>()))
                .ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<GetUsuarioByIdQueryHandlerException>(
                () => Handler.Handle(ValidQuery, CancellationToken.None));
        }
        #endregion
    }
}
