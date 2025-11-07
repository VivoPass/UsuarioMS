using log4net;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using Usuarios.API.Controllers;
using Usuarios.Application.DTOs;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Queries;

namespace Usuarios.Tests.Usuarios.API.Controller
{
    public class UsuarioController_GetTodosRoles_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private readonly List<RolDTO> ExpectedList;

        public UsuarioController_GetTodosRoles_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object, MockPublishEndpoint.Object, MockCloudinaryService.Object, MockLogger.Object);

            // --- DATOS ---
            ExpectedList = new List<RolDTO>
            {
                new RolDTO { IdRol = Guid.NewGuid().ToString(), NombreRol = "Admin" },
                new RolDTO { IdRol = Guid.NewGuid().ToString(), NombreRol = "User" }
            };
        }

        #region Exito_ShouldReturn200OKAndListOfDTOs()
        [Fact]
        public async Task GetTodosRoles_RolesEncontrados_ShouldReturn200OKAndListOfDTOs()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(
                    It.IsAny<GetTodosRolesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedList);

            // ACT
            var result = await Controller.GetTodosRoles();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedList = Assert.IsAssignableFrom<IEnumerable<RolDTO>>(okResult.Value);
            Assert.Equal(2, returnedList.Count());
        }
        #endregion

        #region NoEncontrado_MediatorReturnsEmptyList_ShouldReturn404NotFound()
        [Fact]
        public async Task GetTodosRoles_ListaVacia_ShouldReturn404NotFound()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(It.IsAny<GetTodosRolesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<RolDTO>());

            // ACT
            var result = await Controller.GetTodosRoles();

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Equal("No se encontraron roles.", notFoundResult.Value);
        }
        #endregion

        #region NoEncontrado_MediatorReturnsNull_ShouldReturn404NotFound()
        [Fact]
        public async Task GetTodosRoles_MediatorReturnsNull_ShouldReturn404NotFound()
        {
            // ARRANGE
            MockMediator.Setup(m => m.Send(It.IsAny<GetTodosRolesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<RolDTO>)null);

            // ACT
            var result = await Controller.GetTodosRoles();

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Equal("No se encontraron roles.", notFoundResult.Value);
        }
        #endregion

        #region Excepcion_ShouldReturn500InternalServerError()
        [Fact]
        public async Task GetTodosRoles_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var exception = new Exception("Error de conexión a la base de datos de roles.");

            MockMediator
                .Setup(m => m.Send(It.IsAny<GetTodosRolesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // ACT
            var result = await Controller.GetTodosRoles();

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
        #endregion
    }
}
