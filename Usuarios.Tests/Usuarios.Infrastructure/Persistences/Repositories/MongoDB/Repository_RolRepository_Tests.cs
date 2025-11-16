using CloudinaryDotNet.Actions;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Persistences.Repositories.MongoDB;

namespace Usuarios.Tests.Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    public class Repository_RolRepository_Tests
    {
        private readonly Mock<IMongoDatabase> MockMongoDb;
        private readonly Mock<IMongoCollection<BsonDocument>> MockRolCollection;
        private readonly Mock<IRolFactory> MockRolFactory;
        private readonly Mock<ILog> MockLogger;

        private readonly RolRepository Repository;

        // --- DATOS ---
        private readonly string TestRolId = Guid.NewGuid().ToString();
        private readonly string TestRolId2 = Guid.NewGuid().ToString();
        private const string TestRolNombre = "Administrador";
        private const string TestRolNombre2 = "Cliente";
        private const string TestRolKeycloakId = "administrador";
        private const string TestRolKeycloakId2 = "cliente";
        private readonly BsonDocument FoundBsonDocument;
        private readonly BsonDocument FoundBsonDocument2;
        private readonly List<BsonDocument> ListaBsonDocuments;
        private readonly Rol ExpectedRol;
        private readonly Rol ExpectedRol2;
        private readonly List<Rol> ListaRoles;

        public Repository_RolRepository_Tests()
        {
            Environment.SetEnvironmentVariable("MONGODB_CNN", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_NAME", "test_database");

            MockMongoDb = new Mock<IMongoDatabase>();
            MockRolCollection = new Mock<IMongoCollection<BsonDocument>>();
            MockRolFactory = new Mock<IRolFactory>();
            MockLogger = new Mock<ILog>();

            MockMongoDb.Setup(d => d.GetCollection<BsonDocument>("roles", It.IsAny<MongoCollectionSettings>()))
                .Returns(MockRolCollection.Object);

            var MongoConfigInstance = new MongoDBConfig();
            MongoConfigInstance.db = MockMongoDb.Object;

            Repository = new RolRepository(MongoConfigInstance, MockRolFactory.Object, MockLogger.Object);

            // --- DATOS ---
            FoundBsonDocument = new BsonDocument
            {
                { "_id", TestRolId },
                { "nombre", TestRolNombre },
                { "idKeycloak", TestRolKeycloakId }
            };
            FoundBsonDocument2 = new BsonDocument
            {
                { "_id", TestRolId2 },
                { "nombre", TestRolNombre2 },
                { "idKeycloak", TestRolKeycloakId2 }
            };
             
            ExpectedRol = new Mock<Rol>(
                new VORolId(TestRolId),
                new VORolNombre(TestRolNombre),
                new VORolKeycloakId(TestRolKeycloakId)
            ).Object;

            ExpectedRol2 = new Mock<Rol>(
                new VORolId(TestRolId2),
                new VORolNombre(TestRolNombre2),
                new VORolKeycloakId(TestRolKeycloakId2)
            ).Object;

            ListaBsonDocuments = new List<BsonDocument> { FoundBsonDocument, FoundBsonDocument2 };
            ListaRoles = new List<Rol> { ExpectedRol, ExpectedRol2 };
        }


        #region GetById_RolEncontrado_ShouldReturnRol()
        [Fact]
        public async Task GetById_RolEncontrado_ShouldReturnRol()
        {
            // ARRANGE
            MockRolFactory.Setup(f => f.Load(It.IsAny<VORolId>(), It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>())).Returns(ExpectedRol);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true).ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns(new List<BsonDocument> { FoundBsonDocument });

            MockRolCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(), default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetById(TestRolKeycloakId);

            // ASSERT
            Assert.Same(ExpectedRol, result);
        }
        #endregion

        #region GetById_RolNoEncontrado_ShouldReturnNull()
        [Fact]
        public async Task GetById_RolNoEncontrado_ShouldReturnNull()
        {
            // ARRANGE
            MockRolFactory.Setup(f => f.Load(It.IsAny<VORolId>(), It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>())).Returns(ExpectedRol);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(false);

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetById(TestRolKeycloakId);

            // ASSERT
            Assert.Null(result);
        }
        #endregion

        #region GetById_LanzaExcepcionMongo_ShouldThrowRolRepositoryException()
        [Fact]
        public async Task GetById_LanzaExcepcionMongo_ShouldThrowRolRepositoryException()
        {
            // ARRANGE
            var mongoException = new MongoException("Error de conexión simulado.");

            MockRolFactory.Setup(f => f.Load(It.IsAny<VORolId>(), It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>())).Returns(ExpectedRol);

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<RolRepositoryException>(
                () => Repository.GetById(TestRolKeycloakId)
            );

        }
        #endregion


        #region GetByNombre_RolEncontrado_ShouldReturnRol()
        [Fact]
        public async Task GetByNombre_RolEncontrado_ShouldReturnRol()
        {
            // ARRANGE
            MockRolFactory.Setup(f => f.Load(It.IsAny<VORolId>(), It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>())).Returns(ExpectedRol);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true).ReturnsAsync(false);
            cursorMock.Setup(c => c.Current).Returns(new List<BsonDocument> { FoundBsonDocument });

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetByNombre(TestRolNombre);

            // ASSERT
            Assert.Same(ExpectedRol, result);
        }
        #endregion

        #region GetByNombre_RolNoEncontrado_ShouldReturnNull()
        [Fact]
        public async Task GetByNombre_RolNoEncontrado_ShouldReturnNull()
        {
            // ARRANGE
            MockRolFactory.Setup(f => f.Load(It.IsAny<VORolId>(), It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>())).Returns(ExpectedRol);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(false);

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetByNombre(TestRolNombre);

            // ASSERT
            Assert.Null(result);
        }
        #endregion

        #region GetByNombre_ExcepcionEnMongoDB_ShouldThrowRolRepositoryException()
        [Fact]
        public async Task GetByNombre_ExcepcionEnMongoDB_ShouldThrowRolRepositoryException()
        {
            // ARRANGE
            var mongoException = new MongoException("Error de conexión simulado.");

            MockRolFactory.Setup(f => f.Load(It.IsAny<VORolId>(), It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>())).Returns(ExpectedRol);

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<RolRepositoryException>(
                () => Repository.GetByNombre(TestRolNombre)
            );
        }
        #endregion


        #region GetTodos_RolesEncontrados_ShouldReturnListOfRoles()
        [Fact]
        public async Task GetTodos_RolesEncontrados_ShouldReturnListOfRoles()
        {
            // ARRANGE
            MockRolFactory.SetupSequence(f => f.Load(It.IsAny<VORolId>(), It.IsAny<VORolNombre>(),
                    It.IsAny<VORolKeycloakId>()))
                .Returns(ExpectedRol)
                .Returns(ExpectedRol2);

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            cursorMock.Setup(c => c.Current).Returns(ListaBsonDocuments);

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetTodos();

            // ASSERT
            Assert.Equal(2, result.Count);
            // Verifica que la fábrica fue llamada dos veces (una por cada documento)
            MockRolFactory.Verify(f => f.Load(
                It.IsAny<VORolId>(),
                It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>()
            ), Times.Exactly(2));

            // Comprueba que los resultados contienen los objetos esperados (por referencia simulada)
            Assert.Contains(result, r => r == ExpectedRol);
            Assert.Contains(result, r => r == ExpectedRol2);
        }
        #endregion

        #region GetTodos_NoRolesEncontrados_ShouldReturnEmptyList()
        [Fact]
        public async Task GetTodos_NoRolesEncontrados_ShouldReturnEmptyList()
        {
            // ARRANGE
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(false);

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetTodos();

            // ASSERT
            Assert.NotNull(result);
            Assert.Empty(result);
            MockLogger.Verify(l => l.Info("No se encontraron documentos de rol en MongoDB."), Times.Once);
            MockRolFactory.Verify(f => f.Load(
                It.IsAny<VORolId>(),
                It.IsAny<VORolNombre>(),
                It.IsAny<VORolKeycloakId>()
            ), Times.Never); // La fábrica no debe ser llamada
        }
        #endregion

        #region GetTodos_LanzaExcepcionMongo_ShouldThrowRolRepositoryException()
        [Fact]
        public async Task GetTodos_LanzaExcepcionMongo_ShouldThrowRolRepositoryException()
        {
            // ARRANGE
            var mongoException = new MongoException("Error de timeout simulado.");

            // MONGODB: Simula que FindAsync lanza una excepción
            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<RolRepositoryException>(
                () => Repository.GetTodos()
            );

            Assert.NotNull(exception.InnerException);
            Assert.Equal(mongoException, exception.InnerException);
            MockLogger.Verify(l => l.Error(
                "Error al obtener todos los roles de MongoDB.",
                It.IsAny<Exception>()
            ), Times.Once);
        }
        #endregion

    }
}
