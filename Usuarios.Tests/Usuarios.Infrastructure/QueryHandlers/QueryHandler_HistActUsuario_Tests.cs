using log4net;
using MongoDB.Bson;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Queries;
using Usuarios.Infrastructure.Queries.QueryHandlers;

namespace Usuarios.Tests.Usuarios.Infrastructure.QueryHandlers
{
    // Crear una clase simulada para permitir el uso de ConvertAll si no es un List<T> estándar
    public static class BsonDocumentListExtensions
    {
        // Simulación del método ConvertAll para List<BsonDocument> 
        // En un proyecto real, se usaría Select().ToList() o un Mapper adecuado.
        public static List<HistActUsuarioDTO> ConvertAll
            (this List<BsonDocument> activities, Func<BsonDocument, HistActUsuarioDTO> converter)
        {
            return activities.Select(converter).ToList();
        }
    }

    public class QueryHandler_HistActUsuario_Tests
    {
        private readonly Mock<IUsuarioHistorialActividad> MockUsuarioHistActRepository;
        private readonly Mock<ILog> MockLogger;
        private readonly HistActUsuarioQueryHandler Handler;

        // --- DATOS ---
        private readonly string ExistingUserId;
        private readonly GetTodosRolesQuery Query;
        private readonly List<BsonDocument> ListaActividadesExistentes;
        private readonly HistActUsuarioQuery ValidQuery;

        public QueryHandler_HistActUsuario_Tests()
        {
            MockUsuarioHistActRepository = new Mock<IUsuarioHistorialActividad>();
            MockLogger = new Mock<ILog>();
            Handler = new HistActUsuarioQueryHandler(MockUsuarioHistActRepository.Object, MockLogger.Object);

            // --- DATOS ---
            ExistingUserId = Guid.NewGuid().ToString();

            var actividadUno = new BsonDocument
            {
                { "_id", Guid.NewGuid().ToString() },
                { "_idUsuario", ExistingUserId },
                { "accion", "LOGIN_EXITOSO" },
                { "timestamp", DateTime.Now.ToLocalTime() }
            };
            var actividadDos = new BsonDocument
            {
                { "_id", Guid.NewGuid().ToString() },
                { "_idUsuario", ExistingUserId },
                { "accion", "ACTUALIZACION_PERFIL" },
                { "timestamp", DateTime.Now.ToLocalTime().AddMinutes(-10) }
            };

            ListaActividadesExistentes = new List<BsonDocument> { actividadUno, actividadDos };

            ValidQuery = new HistActUsuarioQuery(ExistingUserId, DateTime.Today);
        }

        #region Handle_ActivitiesExist_ShouldReturnListOfDTOs()
        [Fact]
        public async Task Handle_ActivitiesExist_ShouldReturnListOfDTOs()
        {
            // ARRANGE
            MockUsuarioHistActRepository
                .Setup(r => r.GetByIdUsuarioHistAct(ValidQuery.IdUsuario, DateTime.Today))
                .ReturnsAsync(ListaActividadesExistentes);

            // ACT
            var resultDto = await Handler.Handle(ValidQuery, CancellationToken.None);

            // ASSERT
            Assert.Equal(2, resultDto.Count());
        }
        #endregion

        #region Handle_EmptyListFromRepository_ShouldReturnEmptyDTOList()
        [Fact]
        public async Task Handle_EmptyListFromRepository_ShouldReturnEmptyDTOList()
        {
            // ARRANGE
            MockUsuarioHistActRepository
                .Setup(r => r.GetByIdUsuarioHistAct(It.IsAny<string>(), DateTime.Today))
                .ReturnsAsync(new List<BsonDocument>());

            // ACT
            var resultDto = await Handler.Handle(ValidQuery, CancellationToken.None);

            // ASSERT
            Assert.Empty(resultDto);
        }
        #endregion

        #region Handle_NullFromRepository_ShouldReturnEmptyDTOList()

        [Fact]
        public async Task Handle_NullFromRepository_ShouldReturnEmptyDTOList()
        {
            // ARRANGE
            MockUsuarioHistActRepository
                .Setup(r => r.GetByIdUsuarioHistAct(It.IsAny<string>(), DateTime.Today))
                .ReturnsAsync((List<BsonDocument>)null);

            // ACT
            var resultDto = await Handler.Handle(ValidQuery, CancellationToken.None);

            // ASSERT
            Assert.Empty(resultDto);
        }

        #endregion

        #region Handle_RepositoryFails_ShouldThrowHistActUsuarioQueryHandlerException()
        [Fact]
        public async Task Handle_RepositoryFails_ShouldThrowHistActUsuarioQueryHandlerException()
        {
            // ARRANGE
            var dbException = new InvalidOperationException("Simulated database connection failure.");

            MockUsuarioHistActRepository
                .Setup(r => r.GetByIdUsuarioHistAct(It.IsAny<string>(), DateTime.Today))
                .ThrowsAsync(dbException);

            // ACT & ASSERT
            var ex = await Assert.ThrowsAsync<HistActUsuarioQueryHandlerException>(
                () => Handler.Handle(ValidQuery, CancellationToken.None));
        }
        #endregion
        
    }
}
