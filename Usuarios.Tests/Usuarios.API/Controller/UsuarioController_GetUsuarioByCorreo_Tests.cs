using log4net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Usuarios.API.Controllers;
using Usuarios.Application.DTOs;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Queries;

namespace Usuarios.Tests.Usuarios.API.Controller
{
    public class UsuarioController_GetUsuarioByCorreo_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private const string ExistingEmail = "test@correo.com";
        private readonly UsuarioDTO ExpectedDto;

        public UsuarioController_GetUsuarioByCorreo_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object, MockPublishEndpoint.Object, MockCloudinaryService.Object, MockLogger.Object);

            // --- DATOS ---
            ExpectedDto = new UsuarioDTO
            {
                Id = Guid.NewGuid().ToString(),
                Nombre = "Prueba",
                Apellido = "Usuario",
                Correo = ExistingEmail
            };
        }

        #region Exito_ShouldReturn200OKAndUsuarioDTO()
        [Fact]
        public async Task GetUsuarioByCorreo_UsuarioEncontrado_ShouldReturn200OKAndUsuarioDTO()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(
                    It.Is<GetUsuarioByCorreoQuery>(q => q.Correo == ExistingEmail),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedDto);

            // ACT
            var result = await Controller.GetUsuarioByCorreo(ExistingEmail);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedDto = Assert.IsType<UsuarioDTO>(okResult.Value);
            Assert.Equal(ExpectedDto.Correo, returnedDto.Correo);
        }
        #endregion

        #region NoEncontrado_ShouldReturn404NotFound()
        [Fact]
        public async Task GetUsuarioByCorreo_UsuarioNoEncontrado_ShouldReturn404NotFound()
        {
            // ARRANGE
            const string NonExistingEmail = "nonexistent@correo.com";

            MockMediator
                .Setup(m => m.Send(It.IsAny<GetUsuarioByCorreoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UsuarioDTO)null);

            // ACT
            var result = await Controller.GetUsuarioByCorreo(NonExistingEmail);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Contains(NonExistingEmail, notFoundResult.Value.ToString());
        }
        #endregion

        #region Excepcion_ShouldReturn500InternalServerError()
        [Fact]
        public async Task GetUsuarioByCorreo_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var exception = new Exception("Error de conexión a la base de datos.");

            MockMediator
                .Setup(m => m.Send(It.IsAny<GetUsuarioByCorreoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // ACT
            var result = await Controller.GetUsuarioByCorreo(ExistingEmail);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
        #endregion
    }
}
