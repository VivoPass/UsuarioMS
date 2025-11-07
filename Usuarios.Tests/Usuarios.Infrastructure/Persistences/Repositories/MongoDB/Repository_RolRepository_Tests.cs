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
        private readonly Mock<IAsyncCursor<BsonDocument>> MockCursor;
        private readonly Mock<MongoClient> MockMongoClient;

        private readonly RolRepository Repository;
        private readonly MongoDBConfig RealMongoConfigInstance;

        // --- DATOS ---
        private readonly string TestRolId = Guid.NewGuid().ToString();
        private const string TestRolNombre = "Administrador";
        private readonly BsonDocument FoundBsonDocument;
        private readonly Rol ExpectedRol;

        public Repository_RolRepository_Tests()
        {
            Environment.SetEnvironmentVariable("MONGODB_CNN", "mongodb://localhost:27017");
            Environment.SetEnvironmentVariable("MONGODB_NAME", "test_database");

            MockMongoDb = new Mock<IMongoDatabase>();
            MockRolCollection = new Mock<IMongoCollection<BsonDocument>>();
            MockRolFactory = new Mock<IRolFactory>();
            MockLogger = new Mock<ILog>();
            MockCursor = new Mock<IAsyncCursor<BsonDocument>>();

            MockMongoClient = new Mock<MongoClient>(
                MongoClientSettings.FromConnectionString("mongodb://localhost")
            );

            RealMongoConfigInstance = new MongoDBConfig();
            RealMongoConfigInstance.db = MockMongoDb.Object;
            RealMongoConfigInstance.client = MockMongoClient.Object;

            MockMongoDb
                .Setup(db => db.GetCollection<BsonDocument>("roles", null))
                .Returns(MockRolCollection.Object);

            // --- DATOS ---
            FoundBsonDocument = new BsonDocument
            {
                { "_id", TestRolId },
                { "nombre", TestRolNombre }
            };
            ExpectedRol = new Mock<Rol>(
                new VORolId(TestRolId),
                new VORolNombre(TestRolNombre)
            ).Object;

            Repository = new RolRepository(RealMongoConfigInstance, MockRolFactory.Object, MockLogger.Object);
        }

        /*
        #region GetById_RolEncontrado_ShouldReturnRolAndCallFactory
        [Fact]
        public async Task GetById_RolEncontrado_ShouldReturnRolAndCallFactory()
        {
            // ARRANGE
            MockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true).ReturnsAsync(false);
            MockCursor.Setup(c => c.Current).Returns(new List<BsonDocument> { FoundBsonDocument });

            MockRolCollection.Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<BsonDocument>>(),
                    It.IsAny<FindOptions<BsonDocument, BsonDocument>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(MockCursor.Object);

            MockRolFactory.Setup(f => f.Load(
                    It.Is<VORolId>(id => id.ToString() == TestRolId),
                    It.Is<VORolNombre>(n => n.ToString() == TestRolNombre)))
                .Returns(ExpectedRol);

            // ACT
            var result = await Repository.GetById(TestRolId);

            // ASSERT
            Assert.Same(ExpectedRol, result);
        }
        #endregion
        */

    }
}
