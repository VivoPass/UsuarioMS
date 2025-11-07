using log4net;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MediatR;
using Usuarios.API.Controllers;
using Usuarios.Application.Commands;
using Usuarios.Application.DTOs;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Tests.Usuarios.API.Controller
{
    public class UsuarioController_CrearUsuario_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private readonly CrearUsuarioDTO ValidDto;
        private readonly Mock<IFormFile> MockImageFile;
        private readonly string ExpectedUserId = Guid.NewGuid().ToString();
        private const string ExpectedImageUrl = "https://cloudinary.com/new_profile_pic.jpg";

        public UsuarioController_CrearUsuario_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object, MockPublishEndpoint.Object, MockCloudinaryService.Object, MockLogger.Object);

            // --- DATOS ---
            ValidDto = new CrearUsuarioDTO
            {
                Nombre = "Test",
                Apellido = "User",
                Correo = "valid@test.com",
                FechaNacimiento = new DateOnly(2002, 10, 14),
                Telefono = "1234567890",
                Direccion = "123 Test St",
                FotoPerfil = null,
                Rol = Guid.NewGuid().ToString()

            };

            MockImageFile = new Mock<IFormFile>();
            MockImageFile.SetupGet(f => f.FileName).Returns("profile.jpg");
            MockImageFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));

            MockCloudinaryService
                .Setup(s => s.SubirImagen(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(ExpectedImageUrl);
        }

        #region Exito_ShouldReturn201CreatedAndUserId()
        [Fact]
        public async Task CrearUsuario_Exito_ShouldReturn201CreatedAndUserId()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(It.IsAny<CrearUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ExpectedUserId);

            // ACT
            var result = await Controller.CrearUsuario(ValidDto, MockImageFile.Object);

            // ASSERT
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
        }
        #endregion

        #region MediatorReturnsNull_ShouldReturn400BadRequest()
        [Fact]
        public async Task CrearUsuario_MediatorReturnsNull_ShouldReturn400BadRequest()
        {
            // ARRANGE
            MockMediator
                .Setup(m => m.Send(It.IsAny<CrearUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string)null);

            // ACT
            var result = await Controller.CrearUsuario(ValidDto, MockImageFile.Object);

            // ASSERT 
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }
        #endregion

        #region CloudinaryFails_ShouldReturn500InternalServerError()
        [Fact]
        public async Task CrearUsuario_CloudinaryFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var cloudinaryException = new Exception("Error simulado al subir la imagen a Cloudinary.");

            MockCloudinaryService
                .Setup(s => s.SubirImagen(It.IsAny<Stream>(), It.IsAny<string>()))
                .ThrowsAsync(cloudinaryException);

            // ACT
            var result = await Controller.CrearUsuario(ValidDto, MockImageFile.Object);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
        #endregion

        #region MediatorFails_ShouldReturn500InternalServerError()
        [Fact]
        public async Task CrearUsuario_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var mediatorException = new Exception("Error de validación o DB en el Handler.");

            MockMediator
                .Setup(m => m.Send(It.IsAny<CrearUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(mediatorException);

            // ACT
            var result = await Controller.CrearUsuario(ValidDto, MockImageFile.Object);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
        #endregion
    }
}
