using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Persistences.Repositories.MongoDB;

namespace Usuarios.Tests.Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    public class Repository_AuditoriaRepository_Tests
    {
        private readonly Mock<IMongoDatabase> MockMongoDb;
        private readonly Mock<IMongoCollection<BsonDocument>> MockAuditoriaCollection;
        private readonly Mock<ILog> MockLogger;

        private readonly AuditoriaRepository Repository;

        // --- DATOS ---
        private readonly string TestUserId = Guid.NewGuid().ToString();
        private const string TestLevelAuU = "INFO";
        private const string TestTypeAuU = "Au_Usuario_Exitoso";
        private const string TestMessageAuU = "Mensaje de Exito";

        public Repository_AuditoriaRepository_Tests()
        {
            Environment.SetEnvironmentVariable("MONGODB_CNN", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_NAME_AUDITORIAS", "test_database");

            MockMongoDb = new Mock<IMongoDatabase>();
            MockAuditoriaCollection = new Mock<IMongoCollection<BsonDocument>>();
            MockLogger = new Mock<ILog>();

            MockMongoDb.Setup(d => d.GetCollection<BsonDocument>("auditoriaUsuarios", It.IsAny<MongoCollectionSettings>()))
                .Returns(MockAuditoriaCollection.Object);

            var MongoConfigInstance = new AuditoriaDbConfig();
            MongoConfigInstance.db = MockMongoDb.Object;

            Repository = new AuditoriaRepository(MongoConfigInstance, MockLogger.Object);
        }

        #region InsertarAuditoriaUsuario_InsercionExitosa_ShouldCompleteTask()
        [Fact]
        public async Task InsertarAuditoriaUsuario_InsercionExitosa_ShouldCompleteTask()
        {
            //ARRANGE
            MockAuditoriaCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // ACT
            await Repository.InsertarAuditoriaUsuario(TestUserId, TestLevelAuU, TestTypeAuU, TestMessageAuU);

            // ASSERT
            MockAuditoriaCollection.Verify(c => c.InsertOneAsync(
                    It.IsAny<BsonDocument>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        #endregion

        #region InsertarAuditoriaUsuario_LanzaExcepcionMongo_ShouldThrowMongoException()
        [Fact]
        public async Task InsertarAuditoriaUsuario_LanzaExcepcionMongo_ShouldThrowMongoException()
        {
            //ARRANGE
            var mongoException = new MongoException("Error de inserción simulado.");
            MockAuditoriaCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(mongoException);

            // ACT & ASSERT
            var capturedException = await Assert.ThrowsAsync<MongoException>(
                () => Repository.InsertarAuditoriaUsuario(TestUserId, TestLevelAuU, TestTypeAuU, TestMessageAuU));

            Assert.Equal(mongoException.Message, capturedException.Message);
        }
        #endregion

        #region InsertarAuditoriaUsuario_LanzaExcepcion_ShouldThrowAuditoriaRepositoryException()
        [Fact]
        public async Task InsertarAuditoriaUsuario_LanzaExcepcion_ShouldThrowAuditoriaRepositoryException()
        {
            // ARRANGE
            var mongoException = new Exception("Error de inserción simulado.");
            MockAuditoriaCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<AuditoriaRepositoryException>(
                () => Repository.InsertarAuditoriaUsuario(TestUserId, TestLevelAuU, TestTypeAuU, TestMessageAuU));

            Assert.Equal(mongoException, exception.InnerException);
        }
        #endregion


        #region InsertarAuditoriaHistAct_InsercionExitosa_ShouldCompleteTask()
        [Fact]
        public async Task InsertarAuditoriaHistAct_InsercionExitosa_ShouldCompleteTask()
        {
            //ARRANGE
            MockAuditoriaCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // ACT
            await Repository.InsertarAuditoriaHistAct(TestUserId, TestLevelAuU, TestTypeAuU, TestMessageAuU);

            // ASSERT
            MockAuditoriaCollection.Verify(c => c.InsertOneAsync(
                    It.IsAny<BsonDocument>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        #endregion

        #region InsertarAuditoriaHistAct_LanzaExcepcionMongo_ShouldThrowMongoException()
        [Fact]
        public async Task InsertarAuditoriaHistAct_LanzaExcepcionMongo_ShouldThrowMongoException()
        {
            //ARRANGE
            var mongoException = new MongoException("Error de inserción simulado.");
            MockAuditoriaCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(mongoException);

            // ACT & ASSERT
            var capturedException = await Assert.ThrowsAsync<MongoException>(
                () => Repository.InsertarAuditoriaHistAct(TestUserId, TestLevelAuU, TestTypeAuU, TestMessageAuU));

            Assert.Equal(mongoException.Message, capturedException.Message);
        }
        #endregion

        #region InsertarAuditoriaHistAct_LanzaExcepcion_ShouldThrowAuditoriaRepositoryException()
        [Fact]
        public async Task InsertarAuditoriaHistAct_LanzaExcepcion_ShouldThrowAuditoriaRepositoryException()
        {
            // ARRANGE
            var mongoException = new Exception("Error de inserción simulado.");
            MockAuditoriaCollection.Setup(c => c.InsertOneAsync(
                It.IsAny<BsonDocument>(),
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(mongoException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<AuditoriaRepositoryException>(
                () => Repository.InsertarAuditoriaHistAct(TestUserId, TestLevelAuU, TestTypeAuU, TestMessageAuU));

            Assert.Equal(mongoException, exception.InnerException);
        }
        #endregion
    }
}
