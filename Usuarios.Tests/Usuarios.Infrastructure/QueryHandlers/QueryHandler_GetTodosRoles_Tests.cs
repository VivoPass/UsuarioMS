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
    public class QueryHandler_GetTodosRoles_Tests
    {
        private readonly Mock<IRolRepository> MockRolRepository;
        private readonly Mock<ILog> MockLogger;
        private readonly GetTodosRolesQueryHandler Handler;

        // --- DATOS ---
        private readonly Rol RolCliente;
        private readonly Rol RolAdmin;
        private readonly List<Rol> ListaRolesExistentes;
        private readonly GetTodosRolesQuery Query;

        public QueryHandler_GetTodosRoles_Tests()
        {
            MockRolRepository = new Mock<IRolRepository>();
            MockLogger = new Mock<ILog>();
            Handler = new GetTodosRolesQueryHandler(MockRolRepository.Object, MockLogger.Object);

            // --- DATOS ---
            // Rol 1: Cliente
            RolCliente = new Rol(
                new VORolId(Guid.NewGuid().ToString()),
                new VORolNombre("Cliente")
            );
            // Rol 2: Administrador
            RolAdmin = new Rol(
                new VORolId(Guid.NewGuid().ToString()),
                new VORolNombre("Administrador")
            );
            ListaRolesExistentes = new List<Rol> { RolCliente, RolAdmin };

            Query = new GetTodosRolesQuery();
        }

        #region Handle_RolesExist_ShouldReturnListOfRolDTOs()
        [Fact]
        public async Task Handle_RolesExist_ShouldReturnListOfRolDTOs()
        {
            // ARRANGE
            MockRolRepository.Setup(r => r.GetTodos()).ReturnsAsync(ListaRolesExistentes);

            // ACT
            var resultDto = await Handler.Handle(Query, CancellationToken.None);

            // ASSERT
            Assert.Equal(2, resultDto.Count);
        }
        #endregion

        #region Handle_EmptyListFromRepository_ShouldReturnEmptyRolDTOList()
        [Fact]
        public async Task Handle_EmptyListFromRepository_ShouldReturnEmptyRolDTOList()
        {
            // ARRANGE
            MockRolRepository.Setup(r => r.GetTodos()).ReturnsAsync(new List<Rol>());

            // ACT
            var resultDto = await Handler.Handle(Query, CancellationToken.None);

            // ASSERT
            Assert.NotNull(resultDto);
        }
        #endregion

        #region Handle_NullFromRepository_ShouldReturnEmptyRolDTOList()
        [Fact]
        public async Task Handle_NullFromRepository_ShouldReturnEmptyRolDTOList()
        {
            // ARRANGE
            MockRolRepository.Setup(r => r.GetTodos()).ReturnsAsync((List<Rol>)null); // Retorna NULL

            // ACT
            var resultDto = await Handler.Handle(Query, CancellationToken.None);

            // ASSERT
            Assert.Empty(resultDto);
        }
        #endregion

        #region Handle_RepositoryFails_ShouldThrowGetTodosRolesQueryHandlerException()
        [Fact]
        public async Task Handle_RepositoryFails_ShouldThrowGetTodosRolesQueryHandlerException()
        {
            // ARRANGE
            var dbException = new InvalidOperationException("Simulated database connection failure.");

            MockRolRepository
                .Setup(r => r.GetTodos()).ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<GetTodosRolesQueryHandlerException>(
                () => Handler.Handle(Query, CancellationToken.None));
        }
        #endregion
    }
}
