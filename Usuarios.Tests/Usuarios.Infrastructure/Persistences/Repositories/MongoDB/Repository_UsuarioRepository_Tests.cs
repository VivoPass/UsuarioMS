using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Persistences.Repositories.MongoDB;

namespace Usuarios.Tests.Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    public class Repository_UsuarioRepository_Tests
    {
        private readonly Mock<IMongoDatabase> MockMongoDb;
        private readonly Mock<IMongoCollection<BsonDocument>> MockUsuarioCollection;
        private readonly Mock<IUsuarioFactory> MockUsuarioFactory;
        private readonly Mock<ILog> MockLogger;
        private readonly Mock<IAuditoriaRepository> MockAuditoria;

        private readonly UsuarioRepository Repository;

        // --- DATOS ---
        private readonly string TestUsuarioId1 = Guid.NewGuid().ToString();
        private const string TestUsuarioNombre1 = "Cliente";
        private const string TestUsuarioApellido1 = "Test";
        private readonly DateOnly TestUsuarioFechaN1 = new DateOnly(2002, 10, 14);
        private const string TestUsuarioCorreo1 = "cliente1@gmail.com";
        private const string TestUsuarioTelefono1 = "12345678901";
        private const string TestUsuarioDireccion1 = "Calle Test";
        private const string TestUsuarioFotoPerfil1 = "http://example.com/profile.jpg";
        private const string TestUsuarioRol1 = "cliente";

        private readonly string TestUsuarioId2 = Guid.NewGuid().ToString();
        private const string TestUsuarioNombre2 = "Cliente 2";
        private const string TestUsuarioApellido2 = "Test";
        private readonly DateOnly TestUsuarioFechaN2 = new DateOnly(2002, 08, 14);
        private const string TestUsuarioCorreo2 = "cliente2@gmail.com";
        private const string TestUsuarioTelefono2 = "09876543210";
        private const string TestUsuarioDireccion2 = "Calle Test 2";
        private const string TestUsuarioFotoPerfil2 = "http://example.com/profile1.jpg";
        private const string TestUsuarioRol2 = "administrador";

        private readonly Usuario ExpectedUser1;
        private readonly Usuario ExpectedUser2;
        private readonly List<Usuario> ListaUsuarios;
        private readonly BsonDocument TestBsonDocument1;
        private readonly BsonDocument TestBsonDocument2;
        private readonly List<BsonDocument> ListaBsonDocuments;

        public Repository_UsuarioRepository_Tests()
        {
            Environment.SetEnvironmentVariable("MONGODB_CNN", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_NAME", "test_database");

            MockMongoDb = new Mock<IMongoDatabase>();
            MockUsuarioCollection = new Mock<IMongoCollection<BsonDocument>>();
            MockUsuarioFactory = new Mock<IUsuarioFactory>();
            MockLogger = new Mock<ILog>();
            MockAuditoria = new Mock<IAuditoriaRepository>();

            MockMongoDb.Setup(d => d.GetCollection<BsonDocument>("usuarios", It.IsAny<MongoCollectionSettings>()))
                .Returns(MockUsuarioCollection.Object);

            var MongoConfigInstance = new MongoDBConfig();
            MongoConfigInstance.db = MockMongoDb.Object;

            Repository = new UsuarioRepository(MongoConfigInstance, MockUsuarioFactory.Object, MockLogger.Object, MockAuditoria.Object);

            // --- DATOS ---
            ExpectedUser1 = new Mock<Usuario>(
                new VOId(TestUsuarioId1),
                new VONombre(TestUsuarioNombre1),
                new VOApellido(TestUsuarioApellido1),
                new VOFechaNacimiento(TestUsuarioFechaN1),
                new VOCorreo(TestUsuarioCorreo1),
                new VOTelefono(TestUsuarioTelefono1),
                new VODireccion(TestUsuarioDireccion1),
                new VORolKeycloakId(TestUsuarioRol1),
                new VOFotoPerfil(TestUsuarioFotoPerfil1)

            ).Object;

            ExpectedUser2 = new Mock<Usuario>(
                new VOId(TestUsuarioId2),
                new VONombre(TestUsuarioNombre2),
                new VOApellido(TestUsuarioApellido2),
                new VOFechaNacimiento(TestUsuarioFechaN2),
                new VOCorreo(TestUsuarioCorreo2),
                new VOTelefono(TestUsuarioTelefono2),
                new VODireccion(TestUsuarioDireccion2),
                new VORolKeycloakId(TestUsuarioRol2),
                new VOFotoPerfil(TestUsuarioFotoPerfil2)

            ).Object;

            TestBsonDocument1 = new BsonDocument
            {
                { "_id", TestUsuarioId1 },
                { "nombre", TestUsuarioNombre1 },
                { "apellido", TestUsuarioApellido1 },
                { "fechaNacimiento", TestUsuarioFechaN1.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) },
                { "correo", TestUsuarioCorreo1 },
                {"telefono", TestUsuarioTelefono1},
                {"direccion", TestUsuarioDireccion1},
                {"fotoPerfil", TestUsuarioFotoPerfil1 ?? ""},
                { "rol", TestUsuarioRol1 },
                { "createdAt", DateTime.UtcNow },
                { "updatedAt", DateTime.UtcNow }
            };
            TestBsonDocument2 = new BsonDocument
            {
                { "_id", TestUsuarioId2 },
                { "nombre", TestUsuarioNombre2 },
                { "apellido", TestUsuarioApellido2 },
                { "fechaNacimiento", TestUsuarioFechaN2.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) },
                { "correo", TestUsuarioCorreo2 },
                {"telefono", TestUsuarioTelefono2 },
                {"direccion", TestUsuarioDireccion2 },
                {"fotoPerfil", TestUsuarioFotoPerfil2 ?? ""},
                { "rol", TestUsuarioRol2 },
                { "createdAt", DateTime.UtcNow },
                { "updatedAt", DateTime.UtcNow }
            };

            ListaBsonDocuments = new List<BsonDocument> { TestBsonDocument1, TestBsonDocument2 };
            ListaUsuarios = new List<Usuario> { ExpectedUser1, ExpectedUser2 };
        }

        #region CrearUsuario_InvocacionExitosa_DebeLlamarInsertOneAsyncUnaVez()
        [Fact]
        public async Task CrearUsuario_InvocacionExitosa_DebeLlamarInsertOneAsyncUnaVez()
        {
            // Arrange
            MockUsuarioCollection.Setup(c => c.InsertOneAsync(
                    It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            await Repository.CrearUsuario(ExpectedUser1);

            // Assert
            MockUsuarioCollection.Verify(c => c.InsertOneAsync(
                It.Is<BsonDocument>(doc => doc["_id"].AsString == ExpectedUser1.Id.Valor),
                It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);

        }
        #endregion

        #region CrearUsuario_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        [Fact]
        public async Task CrearUsuario_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        {
            // Arrange
            var mongoException = new MongoException("Error de inserción simulado.");

            MockUsuarioCollection.Setup(c => c.InsertOneAsync(
                    It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>())).ThrowsAsync(mongoException);

            // Act & Assert
            await Assert.ThrowsAsync<UsuarioRepositoryException>(() => Repository.CrearUsuario(ExpectedUser1));
        }
        #endregion


        #region GetByCorreo_UsuarioEncontrado_DebeRetornarUsuario()
        [Fact]
        public async Task GetByCorreo_UsuarioEncontrado_DebeRetornarUsuario()
        {
            // Arrange
            MockUsuarioFactory.Setup(f => f.Load(
                    It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                    It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), It.IsAny<VORolKeycloakId>(), It.IsAny<VOFotoPerfil>()))
                .Returns(ExpectedUser1);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true).ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns(new List<BsonDocument> { TestBsonDocument1 });

            MockUsuarioCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var resultado = await Repository.GetByCorreo(ExpectedUser1.Correo.Valor);

            // Assert
            Assert.Equal(ExpectedUser1.Correo.Valor, resultado!.Correo.Valor);
        }
        #endregion

        #region GetByCorreo_UsuarioNoEncontrado_DebeRetornarNull()
        [Fact]
        public async Task GetByCorreo_UsuarioNoEncontrado_DebeRetornarNull()
        {
            // Arrange
            MockUsuarioFactory.Setup(f => f.Load(
                    It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                    It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), It.IsAny<VORolKeycloakId>(), It.IsAny<VOFotoPerfil>()))
                .Returns(ExpectedUser1);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(false);

            MockUsuarioCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var resultado = await Repository.GetByCorreo(TestUsuarioCorreo1);

            // Assert
            Assert.Null(resultado);
            MockUsuarioFactory.Verify(f => f.Load(
                It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), 
                It.IsAny<VORolKeycloakId>(), It.IsAny<VOFotoPerfil>()), Times.Never);
        }
        #endregion

        #region GetByCorreo_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        [Fact]
        public async Task GetByCorreo_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        {
            // Arrange
            var mongoException = new MongoException("Error de conexión");
            MockUsuarioFactory.Setup(f => f.Load(
                    It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                    It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), It.IsAny<VORolKeycloakId>(), It.IsAny<VOFotoPerfil>()))
                .Returns(ExpectedUser1);

            MockUsuarioCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ThrowsAsync(mongoException);

            // Act & Assert
            await Assert.ThrowsAsync<UsuarioRepositoryException>(() => Repository.GetByCorreo(TestUsuarioCorreo1));
        }
        #endregion


        #region GetById_UsuarioEncontrado_DebeRetornarUsuario()
        [Fact]
        public async Task GetById_UsuarioEncontrado_DebeRetornarUsuario()
        {
            // Arrange
            MockUsuarioFactory.Setup(f => f.Load(
                    It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                    It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), It.IsAny<VORolKeycloakId>(), It.IsAny<VOFotoPerfil>()))
                .Returns(ExpectedUser1);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true).ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns(new List<BsonDocument> { TestBsonDocument1 });

            MockUsuarioCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var resultado = await Repository.GetById(ExpectedUser1.Id.Valor);

            // Assert
            Assert.Equal(ExpectedUser1.Id.Valor, resultado!.Id.Valor);
        }
        #endregion

        #region GetById_UsuarioNoEncontrado_DebeRetornarNull()
        [Fact]
        public async Task GetById_UsuarioNoEncontrado_DebeRetornarNull()
        {
            // Arrange
            MockUsuarioFactory.Setup(f => f.Load(
                    It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                    It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), It.IsAny<VORolKeycloakId>(), It.IsAny<VOFotoPerfil>()))
                .Returns(ExpectedUser1);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(false);

            MockUsuarioCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // Act
            var resultado = await Repository.GetById(TestUsuarioId1);

            // Assert
            Assert.Null(resultado);
        }
        #endregion

        #region GetById_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        [Fact]
        public async Task GetById_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        {
            // Arrange
            var mongoException = new MongoException("Error de conexión");
            MockUsuarioFactory.Setup(f => f.Load(
                    It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                    It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), It.IsAny<VORolKeycloakId>(), It.IsAny<VOFotoPerfil>()))
                .Returns(ExpectedUser1);

            MockUsuarioCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ThrowsAsync(mongoException);

            // Act & Assert
            await Assert.ThrowsAsync<UsuarioRepositoryException>(() => Repository.GetById(TestUsuarioId1));
        }
        #endregion


        #region ModificarUsuario_ActualizacionExitosa_DebeLlamarUpdateOneAsync()
        [Fact]
        public async Task ModificarUsuario_ActualizacionExitosa_DebeLlamarUpdateOneAsync()
        {
            // Arrange
            var mockResult = new Mock<UpdateResult>();
            mockResult.SetupGet(r => r.ModifiedCount).Returns(1);

            MockUsuarioCollection.Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(),
                It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult.Object);

            // Act
            await Repository.ModificarUsuario(ExpectedUser1);

            // Assert
            MockUsuarioCollection.Verify(c => c.UpdateOneAsync(
                It.Is<FilterDefinition<BsonDocument>>(filter => filter != null),
                It.Is<UpdateDefinition<BsonDocument>>(update => update != null),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        #endregion

        #region ModificarUsuario_DocumentoNoEncontrado_NoDebeLanzarExcepcion()
        [Fact]
        public async Task ModificarUsuario_DocumentoNoEncontrado_NoDebeLanzarExcepcion()
        {
            // Arrange
            var mockResult = new Mock<UpdateResult>();
            mockResult.SetupGet(r => r.ModifiedCount).Returns(0); // Simular 0 documentos modificados

            MockUsuarioCollection.Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(),
                It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResult.Object);

            // Act
            await Repository.ModificarUsuario(ExpectedUser1);

            // Assert
            MockLogger.Verify(l => l.Warn(It.IsRegex("Modificación de Usuario ID.*falló.*")), Times.Once);
            MockLogger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
        }
        #endregion

        #region ModificarUsuario_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        [Fact]
        public async Task ModificarUsuario_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        {
            // Arrange
            var mongoException = new MongoException("Error de conexión");
            MockUsuarioCollection.Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>(),
                It.IsAny<UpdateOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(mongoException);

            // Act & Assert
            await Assert.ThrowsAsync<UsuarioRepositoryException>(() => Repository.ModificarUsuario(ExpectedUser1));
        }
        #endregion


        #region GetTodos_DocumentosEncontrados_DebeRetornarListaDeUsuarios()
        [Fact]
        public async Task GetTodos_DocumentosEncontrados_DebeRetornarListaDeUsuarios()
        {
            // Arrange
            MockUsuarioFactory.SetupSequence(f => f.Load(
                    It.IsAny<VOId>(), It.IsAny<VONombre>(), It.IsAny<VOApellido>(), It.IsAny<VOFechaNacimiento>(),
                    It.IsAny<VOCorreo>(), It.IsAny<VOTelefono>(), It.IsAny<VODireccion>(), It.IsAny<VORolKeycloakId>(),
                    It.IsAny<VOFotoPerfil>()))
                .Returns(ExpectedUser1)
                .Returns(ExpectedUser2);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true).ReturnsAsync(false);

            cursorMock.Setup(c => c.Current).Returns(ListaBsonDocuments);

            MockUsuarioCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default)).ReturnsAsync(cursorMock.Object);

            // Act
            var resultado = await Repository.GetTodos();

            // Assert
            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, r => r == ExpectedUser1);
            Assert.Contains(resultado, r => r == ExpectedUser2);
        }
        #endregion

        #region GetTodos_ColeccionVacia_DebeRetornarListaVacia()
        [Fact]
        public async Task GetTodos_ColeccionVacia_DebeRetornarListaVacia()
        {
            // Arrange
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(false);

            MockUsuarioCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default)).ReturnsAsync(cursorMock.Object);

            // Act
            var resultado = await Repository.GetTodos();

            // Assert
            Assert.Empty(resultado);
        }
        #endregion

        #region GetTodos_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        [Fact]
        public async Task GetTodos_FalloDeMongoDB_DebeLanzarUsuarioRepositoryException()
        {
            // Arrange
            var mongoException = new MongoException("Error de timeout simulado.");

            MockUsuarioCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default)).ThrowsAsync(mongoException);


            // Act & Assert
            await Assert.ThrowsAsync<UsuarioRepositoryException>(() => Repository.GetTodos());
        }
        #endregion

    }
}
