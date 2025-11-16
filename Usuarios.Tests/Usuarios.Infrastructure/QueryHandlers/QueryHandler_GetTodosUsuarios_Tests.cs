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
    public class QueryHandler_GetTodosUsuarios_Tests
    {
        private readonly Mock<IUsuarioRepository> MockUsuarioRepository;
        private readonly Mock<ILog> MockLogger;
        private readonly GetTodosUsuariosQueryHandler Handler;

        // --- DATOS ---
        private readonly Usuario UsuarioUno;
        private readonly Usuario UsuarioDos;
        private readonly List<Usuario> ListaUsuariosExistentes;
        private readonly GetTodosUsuariosQuery Query;

        public QueryHandler_GetTodosUsuarios_Tests()
        {
            MockUsuarioRepository = new Mock<IUsuarioRepository>();
            MockLogger = new Mock<ILog>();
            Handler = new GetTodosUsuariosQueryHandler(MockUsuarioRepository.Object, MockLogger.Object);

            // --- DATOS ---
            var rolClienteId = new VORolKeycloakId("administrador");
            UsuarioUno = new Usuario(
                id: new VOId(Guid.NewGuid().ToString()),
                nombre: new VONombre("Juan"),
                apellido: new VOApellido("Pérez"),
                fechaNacimiento: new VOFechaNacimiento(new DateOnly(1985, 5, 10)),
                correo: new VOCorreo("juan.perez@test.com"),
                telefono: new VOTelefono("123456789"),
                direccion: new VODireccion("Direccion Prueba 2"),
                fotoPerfil: new VOFotoPerfil("foto1.jpg"),
                rol: rolClienteId
            );
            UsuarioDos = new Usuario(
                id: new VOId(Guid.NewGuid().ToString()),
                nombre: new VONombre("Ana"),
                apellido: new VOApellido("Gómez"),
                fechaNacimiento: new VOFechaNacimiento(new DateOnly(1992, 11, 25)),
                correo: new VOCorreo("ana.gomez@test.com"),
                telefono: new VOTelefono("987654321"),
                direccion: new VODireccion("Direccion Prueba 2"),
                fotoPerfil: new VOFotoPerfil("foto2.jpg"),
                rol: rolClienteId
            );
            // Lista de Entidades Usuario Existentes
            ListaUsuariosExistentes = new List<Usuario> { UsuarioUno, UsuarioDos };
            // La Query GetTodosUsuariosQuery no requiere parámetros
            Query = new GetTodosUsuariosQuery();
        }

        #region Handle_UsersExist_ShouldReturnListOfUsuarioDTOs()
        [Fact]
        public async Task Handle_UsersExist_ShouldReturnListOfUsuarioDTOs()
        {
            // ARRANGE
            MockUsuarioRepository.Setup(r => r.GetTodos()).ReturnsAsync(ListaUsuariosExistentes);

            // ACT
            var resultDto = await Handler.Handle(Query, CancellationToken.None);

            // ASSERT
            Assert.Equal(2, resultDto.Count);
        }
        #endregion

        #region Handle_EmptyListFromRepository_ShouldReturnEmptyUsuarioDTOList()
        [Fact]
        public async Task Handle_EmptyListFromRepository_ShouldReturnEmptyUsuarioDTOList()
        {
            // ARRANGE
            MockUsuarioRepository.Setup(r => r.GetTodos()).ReturnsAsync(new List<Usuario>());

            // ACT
            var resultDto = await Handler.Handle(Query, CancellationToken.None);

            // ASSERT
            Assert.Empty(resultDto);
        }
        #endregion

        #region Handle_NullFromRepository_ShouldReturnEmptyUsuarioDTOList()
        [Fact]
        public async Task Handle_NullFromRepository_ShouldReturnEmptyUsuarioDTOList()
        {
            // ARRANGE
            MockUsuarioRepository.Setup(r => r.GetTodos()).ReturnsAsync((List<Usuario>)null);

            // ACT
            var resultDto = await Handler.Handle(Query, CancellationToken.None);

            // ASSERT
            Assert.NotNull(resultDto);
        }
        #endregion

        #region Handle_RepositoryFails_ShouldThrowGetTodosUsuariosQueryHandlerException()
        [Fact]
        public async Task Handle_RepositoryFails_ShouldThrowGetTodosUsuariosQueryHandlerException()
        {
            // ARRANGE
            var dbException = new InvalidOperationException("Simulated database connection failure.");

            MockUsuarioRepository
                .Setup(r => r.GetTodos())
                .ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<GetTodosUsuariosQueryHandlerException>(
                () => Handler.Handle(Query, CancellationToken.None));
        }
        #endregion
    }
}
