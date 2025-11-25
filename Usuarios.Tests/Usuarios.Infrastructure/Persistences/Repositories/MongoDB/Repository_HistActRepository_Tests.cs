using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Persistences.Repositories.MongoDB;

namespace Usuarios.Tests.Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    public class Repository_HistActRepository_Tests
    {
        private readonly Mock<IMongoDatabase> MockMongoDb;
        private readonly Mock<IMongoCollection<BsonDocument>> MockHistActCollection;
        private readonly Mock<ILog> MockLogger;
        private readonly Mock<IAuditoriaRepository> MockAuditoria;

        private readonly UsuarioHistorialActividadRepository Repository;

        // --- DATOS ---
        private readonly string TestUserId = Guid.NewGuid().ToString();
        private const string TestAction = "LOGIN_EXITOSO";
        private readonly DateTime TestTimestamp = DateTime.UtcNow.AddHours(-1);

        private readonly BsonDocument ActivityBsonDocument;
        private readonly List<BsonDocument> ListActivityBsonDocuments;

        public Repository_HistActRepository_Tests()
        {
            Environment.SetEnvironmentVariable("MONGODB_CNN", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_NAME", "test_database");

            MockMongoDb = new Mock<IMongoDatabase>();
            MockHistActCollection = new Mock<IMongoCollection<BsonDocument>>();
            MockLogger = new Mock<ILog>();
            MockAuditoria = new Mock<IAuditoriaRepository>();

            MockMongoDb.Setup(d => d.GetCollection<BsonDocument>("historial_act_usuarios", It.IsAny<MongoCollectionSettings>()))
                .Returns(MockHistActCollection.Object);

            var MongoConfigInstance = new MongoDBConfig();
            MongoConfigInstance.db = MockMongoDb.Object;

            Repository = new UsuarioHistorialActividadRepository(MongoConfigInstance, MockLogger.Object, MockAuditoria.Object);

            // --- DATOS ---
            ActivityBsonDocument = new BsonDocument
            {
                { "idUsuario", TestUserId },
                { "_idUsuario", TestUserId },
                { "accion", TestAction },
                { "timestamp", TestTimestamp }
            };

            ListActivityBsonDocuments = new List<BsonDocument> { ActivityBsonDocument, ActivityBsonDocument };
        }

        #region AgregarHistAct_InsercionExitosa_ShouldCompleteTask()
        [Fact]
        public async Task AgregarHistAct_InsercionExitosa_ShouldCompleteTask()
        {
            // ARRANGE
            MockHistActCollection.Setup(c => c.InsertOneAsync(
                    It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(),
                    default)).Returns(Task.CompletedTask);

            // ACT
            await Repository.AgregarHistAct(ActivityBsonDocument);

            // ASSERT
            MockHistActCollection.Verify(c => c.InsertOneAsync(
                ActivityBsonDocument, It.IsAny<InsertOneOptions>(), default),
                Times.Once);
        }
        #endregion

        #region AgregarHistAct_LanzaExcepcionMongo_ShouldThrowHistActRepositoryException()
        [Fact]
        public async Task AgregarHistAct_LanzaExcepcionMongo_ShouldThrowHistActRepositoryException()
        {
            // ARRANGE
            var mongoException = new MongoException("Error de inserción simulado.");

            MockHistActCollection.Setup(c => c.InsertOneAsync(
                    It.IsAny<BsonDocument>(), It.IsAny<InsertOneOptions>(),
                    default)).ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<HistActRepositoryException>(
                () => Repository.AgregarHistAct(ActivityBsonDocument));

            Assert.Equal(mongoException, exception.InnerException);
        }
        #endregion


        #region GetTodosHistAct_ActividadesEncontradas_ShouldReturnListOfBsonDocuments()
        [Fact]
        public async Task GetTodosHistAct_ActividadesEncontradas_ShouldReturnListOfBsonDocuments()
        {
            // ARRANGE
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            cursorMock.Setup(c => c.Current).Returns(ListActivityBsonDocuments);

            MockHistActCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetTodosHistAct();

            // ASSERT
            Assert.Equal(2, result.Count);
        }
        #endregion

        #region GetTodosHistAct_NoActividadesEncontradas_ShouldReturnEmptyList()
        [Fact]
        public async Task GetTodosHistAct_NoActividadesEncontradas_ShouldReturnEmptyList()
        {
            // ARRANGE
            var emptyList = new List<BsonDocument>();

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(false)
                .ReturnsAsync(false);

            cursorMock.Setup(c => c.Current).Returns(emptyList);

            MockHistActCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetTodosHistAct();

            // ASSERT
            Assert.Empty(result);
        }
        #endregion

        #region GetTodosHistAct_LanzaExcepcionMongo_ShouldThrowHistActRepositoryException()
        [Fact]
        public async Task GetTodosHistAct_LanzaExcepcionMongo_ShouldThrowHistActRepositoryException()
        {
            // ARRANGE
            var mongoException = new MongoException("Error de timeout simulado.");

            MockHistActCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<HistActRepositoryException>(
                () => Repository.GetTodosHistAct());

            // Verifica que la excepción envuelta sea la de Mongo
            Assert.Equal(mongoException, exception.InnerException);
        }
        #endregion


        #region GetByIdUsuarioHistAct_ActividadesEncontradas_ShouldReturnFilteredList()
        [Fact]
        public async Task GetByIdUsuarioHistAct_ActividadesEncontradas_ShouldReturnFilteredList()
        {
            // ARRANGE
            var filterTimestamp = TestTimestamp.AddMinutes(5);
            var filteredList = new List<BsonDocument> { ActivityBsonDocument }; 
            
            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            cursorMock.Setup(c => c.Current).Returns(filteredList);

            MockHistActCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetByIdUsuarioHistAct(TestUserId, filterTimestamp);

            // ASSERT
            Assert.Single(result);
        }
        #endregion

        #region GetByIdUsuarioHistAct_NoActividadesEncontradas_ShouldReturnEmptyList()
        [Fact]
        public async Task GetByIdUsuarioHistAct_NoActividadesEncontradas_ShouldReturnEmptyList()
        {
            // ARRANGE
            var filterTimestamp = TestTimestamp.AddMinutes(5);
            var emptyList = new List<BsonDocument>();

            var cursorMock = new Mock<IAsyncCursor<BsonDocument>>();
            cursorMock.SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            cursorMock.Setup(c => c.Current).Returns(emptyList);

            MockHistActCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ReturnsAsync(cursorMock.Object);

            // ACT
            var result = await Repository.GetByIdUsuarioHistAct(TestUserId, filterTimestamp);

            // ASSERT
            Assert.Empty(result);
        }
        #endregion

        #region GetByIdUsuarioHistAct_LanzaExcepcionMongo_ShouldThrowHistActRepositoryException()
        [Fact]
        public async Task GetByIdUsuarioHistAct_LanzaExcepcionMongo_ShouldThrowHistActRepositoryException()
        {
            // ARRANGE
            var filterTimestamp = TestTimestamp.AddMinutes(5);
            var mongoException = new MongoException("Error de conexión simulado.");

            MockHistActCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    default))
                .ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<HistActRepositoryException>(
                () => Repository.GetByIdUsuarioHistAct(TestUserId, filterTimestamp));

            Assert.Equal(mongoException, exception.InnerException);
        }
        #endregion
    }
}
