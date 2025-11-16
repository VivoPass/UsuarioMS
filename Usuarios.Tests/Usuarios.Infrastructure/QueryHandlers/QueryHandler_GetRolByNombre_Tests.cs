using log4net;
using Moq;
using System.Reflection.Metadata;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Queries;
using Usuarios.Infrastructure.Queries.QueryHandlers;

namespace Usuarios.Tests.Usuarios.Infrastructure.QueryHandlers
{
    public class QueryHandler_GetRolByNombre_Tests
    {
        private readonly Mock<IRolRepository> MockRolRepository;
        private readonly Mock<ILog> MockLogger;
        private readonly GetRolByNombreQueryHandler Handler;

        // --- DATOS ---
        private readonly string ExistingRolNombre;
        private readonly string ExistingRolIdString;
        private readonly VORolId ExistingRolIdVO;
        private readonly VORolNombre ExistingRolNombreVO;
        private readonly VORolKeycloakId ExistingRolKeycloakIdVO;
        private readonly Rol ExistingRol;
        private readonly GetRolByNombreQuery ValidQuery;

        public QueryHandler_GetRolByNombre_Tests()
        {
            MockRolRepository = new Mock<IRolRepository>();
            MockLogger = new Mock<ILog>();
            Handler = new GetRolByNombreQueryHandler(MockRolRepository.Object, MockLogger.Object);

            // --- DATOS ---
            ExistingRolNombre = "Supervisor";
            ExistingRolIdString = Guid.NewGuid().ToString();
            ExistingRolIdVO = new VORolId(ExistingRolIdString);
            ExistingRolNombreVO = new VORolNombre(ExistingRolNombre);
            ExistingRolKeycloakIdVO = new VORolKeycloakId("Cliente_Simulado");

            // Entidad Rol Existente (Simulación del output del repositorio)
            ExistingRol = new Rol(ExistingRolIdVO, ExistingRolNombreVO, ExistingRolKeycloakIdVO);
            // Query de Entrada Válida
            ValidQuery = new GetRolByNombreQuery(ExistingRolNombre);
        }

        #region Handle_ExistingNombre_ShouldReturnRolDTO()
        [Fact]
        public async Task Handle_ExistingNombre_ShouldReturnRolDTO()
        {
            // ARRANGE
            MockRolRepository.Setup(r => r.GetByNombre(ValidQuery.Nombre)).ReturnsAsync(ExistingRol);

            // ACT
            var resultDto = await Handler.Handle(ValidQuery, CancellationToken.None);

            // ASSERT
            Assert.Equal(ExistingRolIdString, resultDto.IdRol);
        }
        #endregion

        #region Handle_NonExistingNombre_ShouldThrowNombreRolNotFoundException()
        [Fact]
        public async Task Handle_NonExistingNombre_ShouldThrowNombreRolNotFoundException()
        {
            // ARRANGE
            var nonExistentNombre = "RolInexistente";
            var invalidQuery = new GetRolByNombreQuery(nonExistentNombre);

            MockRolRepository.Setup(r => r.GetByNombre(nonExistentNombre)).ReturnsAsync((Rol)null);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<NombreRolNotFoundException>(() => Handler.Handle(invalidQuery, CancellationToken.None));
        }
        #endregion

        #region Handle_RepositoryFails_ShouldThrowGetRolByNombreQueryHandlerException()
        [Fact]
        public async Task Handle_RepositoryFails_ShouldThrowGetRolByNombreQueryHandlerException()
        {
            // ARRANGE
            var dbException = new InvalidOperationException("Simulated database connection failure.");

            MockRolRepository
                .Setup(r => r.GetByNombre(It.IsAny<string>())).ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<GetRolByNombreQueryHandlerException>(
                () => Handler.Handle(ValidQuery, CancellationToken.None));

        }
        #endregion
    }
}
