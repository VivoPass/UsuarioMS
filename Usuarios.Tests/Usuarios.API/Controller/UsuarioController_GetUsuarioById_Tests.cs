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
    public class UsuarioController_GetUsuarioById_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private readonly string ExistingUserId = Guid.NewGuid().ToString();
        private readonly UsuarioDTO ExpectedDto;

        public UsuarioController_GetUsuarioById_Tests()
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
                Correo = "test@gmail.com"
            };
        }

        #region Exito_ShouldReturn200OKAndUsuarioDTO()
        [Fact]
        public async Task GetUsuarioById_UsuarioEncontrado_ShouldReturn200OKAndUsuarioDTO()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(
                    It.Is<GetUsuarioByIdQuery>(q => q.IdUsuario == ExistingUserId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedDto);

            // ACT
            var result = await Controller.GetUsuarioById(ExistingUserId);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var returnedDto = Assert.IsType<UsuarioDTO>(okResult.Value);
            Assert.Equal(ExpectedDto.Id, returnedDto.Id);
        }
        #endregion

        #region NoEncontrado_ShouldReturn404NotFound()
        [Fact]
        public async Task GetUsuarioById_UsuarioNoEncontrado_ShouldReturn404NotFound()
        {
            // ARRANGE
            const string NonExistingId = "nonexistent-id-123"; 
            
            MockMediator.Setup(m => m.Send(It.IsAny<GetUsuarioByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((UsuarioDTO)null);

            // ACT
            var result = await Controller.GetUsuarioById(NonExistingId);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);

            Assert.Contains(NonExistingId, notFoundResult.Value.ToString());
        }
        #endregion

        #region Excepcion_ShouldReturn500InternalServerError()
        [Fact]
        public async Task GetUsuarioById_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var exception = new Exception("Error de conexión a la base de datos.");

            MockMediator
                .Setup(m => m.Send(It.IsAny<GetUsuarioByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // ACT
            var result = await Controller.GetUsuarioById(ExistingUserId);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
        #endregion
    }
}
