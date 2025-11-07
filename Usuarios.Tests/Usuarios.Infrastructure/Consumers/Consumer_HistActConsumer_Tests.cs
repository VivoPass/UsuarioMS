using MassTransit;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;
using Usuarios.Application.Events;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Consumers;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Tests.Usuarios.Infrastructure.Consumers
{
    public class Consumer_HistActConsumer_Tests
    {
        private readonly Mock<IUsuarioHistorialActividad> MockRepo;
        private readonly HistActConsumer Consumer;


        private readonly string TestUserId = Guid.NewGuid().ToString();
        private readonly DateTime TestTimestamp = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        private readonly HistorialActividadEvent TestMessage;

        public Consumer_HistActConsumer_Tests()
        {
            MockRepo = new Mock<IUsuarioHistorialActividad>();
            Consumer = new HistActConsumer(MockRepo.Object);

            TestMessage = new HistorialActividadEvent
                (usuarioId: TestUserId, accion: "Usuario", timestamp: TestTimestamp);
        }

        #region Constructor_ValidacionNulo
        [Fact]
        public void Constructor_RepoIsNull_ShouldThrowHistActRepositoryNullException()
        {
            //ACT & ASSERT
            Assert.Throws<HistActRepositoryNullException>(() => new HistActConsumer(null));
        }
        #endregion

        #region Consume_Exito_ShouldCallAgregarHistActWithCorrectBsonDocument()
        [Fact]
        public async Task Consume_Exito_ShouldCallAgregarHistActWithCorrectBsonDocument()
        {
            // ARRANGE
            var mockContext = new Mock<ConsumeContext<HistorialActividadEvent>>();
            mockContext.Setup(c => c.Message).Returns(TestMessage);

            // ACT
            await Consumer.Consume(mockContext.Object);

            // ASSERT
            MockRepo.Verify(r => r.AgregarHistAct(It.IsAny<BsonDocument>()), Times.Once);

            MockRepo.Verify(r => r.AgregarHistAct(
                It.Is<BsonDocument>(doc =>
                    // Verifica que los campos cruciales del mensaje se mapearon
                    doc["_idUsuario"].AsString == TestMessage.UsuarioId &&
                    doc["accion"].AsString == TestMessage.Accion &&
                    doc["timestamp"].ToUniversalTime() == TestMessage.Timestamp &&
                    // Verifica que se generó un nuevo _id (no nulo)
                    doc["_id"].IsString && !string.IsNullOrEmpty(doc["_id"].AsString)
                )),
                Times.Once
            );
        }
        #endregion

        #region Consume_FalloRepositorio_ShouldThrowHistActConsumerException()
        [Fact]
        public async Task Consume_FalloRepositorio_ShouldThrowHistActConsumerException()
        {
            var mockContext = new Mock<ConsumeContext<HistorialActividadEvent>>();
            mockContext.Setup(c => c.Message).Returns(TestMessage);

            var dbException = new Exception("Fallo de conexión a la base de datos de historial.");
            MockRepo
                .Setup(r => r.AgregarHistAct(It.IsAny<BsonDocument>()))
                .ThrowsAsync(dbException);

            // ACT & ASSERT
            var exception = await Assert.ThrowsAsync<HistActConsumerException>(() =>
                Consumer.Consume(mockContext.Object));
        }
        #endregion
    }
}
