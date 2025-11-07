using log4net;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using Usuarios.API.Controllers;
using Usuarios.Infrastructure.Queries;
using Usuarios.Application.DTOs;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Tests.Usuarios.API.Controller
{
    public class UsuarioController_GetTodosUsuarios_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private readonly List<UsuarioDTO> ExpectedList;

        public UsuarioController_GetTodosUsuarios_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object, MockPublishEndpoint.Object, MockCloudinaryService.Object, MockLogger.Object);

            // --- DATOS ---
            ExpectedList = new List<UsuarioDTO>
        {
            new UsuarioDTO { Id = Guid.NewGuid().ToString(), Nombre = "Usuario A" },
            new UsuarioDTO { Id = Guid.NewGuid().ToString(), Nombre = "Usuario B" }
        };
        }

        #region Exito_ShouldReturn200OKAndListOfDTOs()
        [Fact]
        public async Task GetTodosUsuarios_UsuariosEncontrados_ShouldReturn200OKAndListOfDTOs()
        {
            // ARRANGE
            MockMediator.Setup(m => m.Send(It.IsAny<GetTodosUsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedList);

            // ACT
            var result = await Controller.GetTodosUsuarios();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedList = Assert.IsAssignableFrom<IEnumerable<UsuarioDTO>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }
        #endregion

        #region NoEncontrado_MediatorReturnsEmptyList_ShouldReturn404NotFound()
        [Fact]
        public async Task GetTodosUsuarios_ListaVacia_ShouldReturn404NotFound()
        {
            // ARRANGE
            MockMediator.Setup(m => m.Send(It.IsAny<GetTodosUsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UsuarioDTO>());

            // ACT
            var result = await Controller.GetTodosUsuarios();

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Equal("No se encontraron usuarios.", notFoundResult.Value);
        }
        #endregion

        #region NoEncontrado_MediatorReturnsNull_ShouldReturn404NotFound()
        [Fact]
        public async Task GetTodosUsuarios_MediatorReturnsNull_ShouldReturn404NotFound()
        {
            // ARRANGE
            MockMediator.Setup(m => m.Send(It.IsAny<GetTodosUsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<UsuarioDTO>)null);

            // ACT
            var result = await Controller.GetTodosUsuarios();

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Equal("No se encontraron usuarios.", notFoundResult.Value);
        }
        #endregion

        #region Excepcion_ShouldReturn500InternalServerError()
        [Fact]
        public async Task GetTodosUsuarios_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var exception = new Exception("Error de tiempo de espera en la base de datos.");

            MockMediator
                .Setup(m => m.Send(It.IsAny<GetTodosUsuariosQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // ACT
            var result = await Controller.GetTodosUsuarios();

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
        #endregion
    }
}
