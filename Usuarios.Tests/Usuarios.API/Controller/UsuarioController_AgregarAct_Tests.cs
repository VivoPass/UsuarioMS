using log4net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Usuarios.API.Controllers;
using Usuarios.Application.Commands;
using Usuarios.Application.DTOs;
using Usuarios.Application.Events;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Queries;

namespace Usuarios.Tests.Usuarios.API.Controller
{
    public class UsuarioController_AgregarAct_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private readonly CrearActUsuarioDTO ValidDto;
        private readonly string TestUserId = Guid.NewGuid().ToString();

        public UsuarioController_AgregarAct_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object,
                MockPublishEndpoint.Object,
                MockCloudinaryService.Object,
                MockLogger.Object
            );

            // --- DATOS ---
            ValidDto = new CrearActUsuarioDTO{IdUsuario = TestUserId,Accion = "Cierre de sesión exitoso"};
        }

        #region Exito_ShouldReturn200OKAndPublishEvent()
        [Fact]
        public async Task AgregarAct_PublicacionExitosa_ShouldReturn200OKAndPublishEvent()
        {
            // ACT
            var result = await Controller.AgregarAct(ValidDto);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            MockPublishEndpoint.Verify(
                p => p.Publish(
                    It.Is<HistorialActividadEvent>(e => e.UsuarioId == ValidDto.IdUsuario && e.Accion == ValidDto.Accion),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }
        #endregion

        #region Excepcion_ShouldReturn500InternalServerError()
        [Fact]
        public async Task AgregarAct_FalloEnPublicacion_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var exception = new Exception("Error de conexión con el bus de mensajes.");

            MockPublishEndpoint
                .Setup(p => p.Publish(It.IsAny<HistorialActividadEvent>(),It.IsAny<CancellationToken>()
                ))
                .ThrowsAsync(exception);

            // ACT
            var result = await Controller.AgregarAct(ValidDto);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            Assert.Equal(exception.Message, statusCodeResult.Value);
        }
        #endregion
    }
}
