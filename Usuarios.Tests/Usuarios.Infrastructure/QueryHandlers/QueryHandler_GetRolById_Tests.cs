using log4net;
using Moq;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Queries;
using Usuarios.Infrastructure.Queries.QueryHandlers;

namespace Usuarios.Tests.Usuarios.Infrastructure.QueryHandlers
{
    public class QueryHandler_GetRolById_Tests
    {
        private readonly Mock<IRolRepository> MockRolRepository;
        private readonly Mock<ILog> MockLogger;
        private readonly GetRolByIdQueryHandler Handler;

        // --- DATOS ---
        private readonly string ExistingRolIdString;
        private readonly VORolId ExistingRolIdVO;
        private readonly VORolNombre ExistingRolNombreVO;
        private readonly VORolKeycloakId ExistingRolKeycloakIdVO;
        private readonly Rol ExistingRol;
        private readonly GetRolByIdQuery ValidQuery;

        public QueryHandler_GetRolById_Tests()
        {
            MockRolRepository = new Mock<IRolRepository>();
            MockLogger = new Mock<ILog>();
            Handler = new GetRolByIdQueryHandler(MockRolRepository.Object, MockLogger.Object);

            // --- DATOS ---
            ExistingRolIdString = Guid.NewGuid().ToString();
            ExistingRolIdVO = new VORolId(ExistingRolIdString);
            ExistingRolNombreVO = new VORolNombre("Administrador");
            ExistingRolKeycloakIdVO = new VORolKeycloakId("Cliente_Simulado");

            ExistingRol = new Rol(ExistingRolIdVO, ExistingRolNombreVO, ExistingRolKeycloakIdVO);

            ValidQuery = new GetRolByIdQuery(ExistingRolIdString);
        }

        #region Handle_ExistingId_ShouldReturnRolDTO()
        [Fact]
        public async Task Handle_ExistingId_ShouldReturnRolDTO()
        {
            // ARRANGE
            MockRolRepository.Setup(r => r.GetById(ValidQuery.Id)).ReturnsAsync(ExistingRol);

            // ACT
            var resultDto = await Handler.Handle(ValidQuery, CancellationToken.None);

            // ASSERT
            Assert.Equal(ExistingRolNombreVO.Valor, resultDto.NombreRol);
        }
        #endregion

        #region Handle_NonExistingId_ShouldThrowIDRolNotFoundException()
        [Fact]
        public async Task Handle_NonExistingId_ShouldThrowIDRolNotFoundException()
        {
            // ARRANGE
            var nonExistentId = Guid.NewGuid().ToString();
            var invalidQuery = new GetRolByIdQuery(nonExistentId);

            MockRolRepository.Setup(r => r.GetById(nonExistentId)).ReturnsAsync((Rol)null);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<IDRolNotFoundException>(() => Handler.Handle(invalidQuery, CancellationToken.None));
        }
        #endregion

        #region Handle_RepositoryFails_ShouldThrowGetRolByIdQueryHandlerException()
        [Fact]
        public async Task Handle_RepositoryFails_ShouldThrowGetRolByIdQueryHandlerException()
        {
            // ARRANGE
            var dbException = new InvalidOperationException("Simulated database connection failure.");

            MockRolRepository
                .Setup(r => r.GetById(It.IsAny<string>())).ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<GetRolByIdQueryHandlerException>(
                () => Handler.Handle(ValidQuery, CancellationToken.None));
        }
        #endregion

    }
}
