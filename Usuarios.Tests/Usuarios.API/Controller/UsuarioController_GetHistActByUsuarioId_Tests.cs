using log4net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Usuarios.API.Controllers;
using Usuarios.Application.Commands;
using Usuarios.Application.DTOs;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Queries;

namespace Usuarios.Tests.Usuarios.API.Controller
{
    public class UsuarioController_GetHistActByUsuarioId_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private readonly string ExistingUserId = Guid.NewGuid().ToString();
        private readonly List<HistActUsuarioDTO> ExpectedActivities;

        public UsuarioController_GetHistActByUsuarioId_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object, MockPublishEndpoint.Object, MockCloudinaryService.Object, MockLogger.Object);

            // --- DATOS ---
            ExpectedActivities = new List<HistActUsuarioDTO>
        {
            new HistActUsuarioDTO { IdUsuario = Guid.NewGuid().ToString(), Accion = "Log in", Timestamp = DateTime.UtcNow.AddHours(-2) },
            new HistActUsuarioDTO { IdUsuario = Guid.NewGuid().ToString(), Accion = "Profile update", Timestamp = DateTime.UtcNow.AddHours(-1) }
        };
        }

        #region Exito_ShouldReturn200OKAndListOfActivities()
        [Fact]
        public async Task GetHistActByUsuarioId_ActividadesEncontradas_ShouldReturn200OKAndListOfActivities()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(
                    It.Is<HistActUsuarioQuery>(q =>
                        q.IdUsuario == ExistingUserId &&
                        q.Timestamp == DateTime.MaxValue),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedActivities);

            // ACT
            var result = await Controller.GetHistActByUsuarioId(ExistingUserId);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedList = Assert.IsAssignableFrom<IEnumerable<HistActUsuarioDTO>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }
        #endregion

        #region NoEncontrado_MediatorReturnsEmptyList_ShouldReturn404NotFound()
        [Fact]
        public async Task GetHistActByUsuarioId_ListaVacia_ShouldReturn404NotFound()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(It.IsAny<HistActUsuarioQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<HistActUsuarioDTO>());

            // ACT
            var result = await Controller.GetHistActByUsuarioId(ExistingUserId);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Contains(ExistingUserId, notFoundResult.Value.ToString());
        }
        #endregion

        #region NoEncontrado_MediatorReturnsNull_ShouldReturn404NotFound()
        [Fact]
        public async Task GetHistActByUsuarioId_MediatorReturnsNull_ShouldReturn404NotFound()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(It.IsAny<HistActUsuarioQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<HistActUsuarioDTO>)null);

            // ACT
            var result = await Controller.GetHistActByUsuarioId(ExistingUserId);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Contains(ExistingUserId, notFoundResult.Value.ToString());
        }
        #endregion

        #region Excepcion_ShouldReturn500InternalServerError()
        [Fact]
        public async Task GetHistActByUsuarioId_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var exception = new Exception("Error al consultar el historial de actividad.");

            MockMediator
                .Setup(m => m.Send(It.IsAny<HistActUsuarioQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // ACT
            var result = await Controller.GetHistActByUsuarioId(ExistingUserId);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
        #endregion
    }
}
