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
    public class UsuarioController_GetRolByNombre_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private const string ExistingRolNombre = "Administrador";
        private readonly RolDTO ExpectedDto;

        public UsuarioController_GetRolByNombre_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object, MockPublishEndpoint.Object, MockCloudinaryService.Object, MockLogger.Object);

            // --- DATOS ---
            ExpectedDto = new RolDTO
            { IdRol = Guid.NewGuid().ToString(), NombreRol = ExistingRolNombre };
        }

        #region Exito_ShouldReturn200OKAndRolDTO()
        [Fact]
        public async Task GetRolByNombre_RolEncontrado_ShouldReturn200OKAndRolDTO()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(
                    It.Is<GetRolByNombreQuery>(q => q.Nombre == ExistingRolNombre), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedDto);

            // ACT
            var result = await Controller.GetRolByNombre(ExistingRolNombre);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedDto = Assert.IsType<RolDTO>(okResult.Value);
            Assert.Equal(ExpectedDto.NombreRol, returnedDto.NombreRol);
        }
        #endregion

        #region NoEncontrado_ShouldReturn404NotFound()
        [Fact]
        public async Task GetRolByNombre_RolNoEncontrado_ShouldReturn404NotFound()
        {
            // ARRANGE
            const string NonExistingRol = "Visitante";

            MockMediator.Setup(m => m.Send(It.IsAny<GetRolByNombreQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RolDTO)null);

            // ACT
            var result = await Controller.GetRolByNombre(NonExistingRol);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Contains(NonExistingRol, notFoundResult.Value.ToString());
        }
        #endregion

        #region Excepcion_ShouldReturn500InternalServerError()
        [Fact]
        public async Task GetRolByNombre_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var exception = new Exception("Error de tiempo de espera en la capa de persistencia.");

            MockMediator
                .Setup(m => m.Send(It.IsAny<GetRolByNombreQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // ACT
            var result = await Controller.GetRolByNombre(ExistingRolNombre);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
        #endregion
    }
}
