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
    public class UsuarioController_ModificarUsuarioById_Tests
    {
        private readonly Mock<IMediator> MockMediator;
        private readonly Mock<IPublishEndpoint> MockPublishEndpoint;
        private readonly Mock<ICloudinaryService> MockCloudinaryService;
        private readonly Mock<ILog> MockLogger;
        private readonly UsuariosController Controller;

        // --- DATOS ---
        private readonly string ExistingUserId = Guid.NewGuid().ToString();
        private readonly ModificarUsuarioDTO ValidDto;
        private readonly Mock<IFormFile> MockImageFile;
        private const string ExpectedImageUrl = "https://cloudinary.com/updated_profile_pic.jpg";

        public UsuarioController_ModificarUsuarioById_Tests()
        {
            MockMediator = new Mock<IMediator>();
            MockPublishEndpoint = new Mock<IPublishEndpoint>();
            MockCloudinaryService = new Mock<ICloudinaryService>();
            MockLogger = new Mock<ILog>();

            Controller = new UsuariosController(
                MockMediator.Object, MockPublishEndpoint.Object, MockCloudinaryService.Object, MockLogger.Object);

            // --- DATOS ---
            ValidDto = new ModificarUsuarioDTO
            {
                Nombre = "Ana Modificada",
                Apellido = "Gomez Modificada",
                Telefono = "123456789",
                Direccion = "New Direccion",
                FotoPerfil = "http://old.url/pic.jpg"
            };

            MockImageFile = new Mock<IFormFile>();
            MockImageFile.SetupGet(f => f.FileName).Returns("new_profile.jpg");
            MockImageFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[] { 1, 2, 3 }));
        }

        #region ExitoConImagen_ShouldReturn200OK()
        [Fact]
        public async Task ModificarUsuarioById_ConNuevaImagen_ShouldReturn200OK()
        {
            // ARRANGE
            MockCloudinaryService
                .Setup(s => s.SubirImagen(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(ExpectedImageUrl);

            MockMediator
                .Setup(m => m.Send(It.IsAny<ModificarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // ACT
            var result = await Controller.ModificarUsuarioById(ExistingUserId, ValidDto, MockImageFile.Object);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }
        #endregion

        #region NoEncontradoConImagen_ShouldReturn404NotFound()
        [Fact]
        public async Task ModificarUsuarioById_ConNuevaImagenFallaMediator_ShouldReturn404NotFound()
        {
            // ARRANGE
            MockCloudinaryService
                .Setup(s => s.SubirImagen(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(ExpectedImageUrl);

            MockMediator
                .Setup(m => m.Send(It.IsAny<ModificarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // ACT
            var result = await Controller.ModificarUsuarioById(ExistingUserId, ValidDto, MockImageFile.Object);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }
        #endregion

        #region CloudinaryFails_ShouldReturn500InternalServerError()
        [Fact]
        public async Task ModificarUsuarioById_CloudinaryFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var cloudinaryException = new Exception("Error al subir la nueva imagen.");

            MockCloudinaryService
                .Setup(s => s.SubirImagen(It.IsAny<Stream>(), It.IsAny<string>()))
                .ThrowsAsync(cloudinaryException);

            // ACT
            var result = await Controller.ModificarUsuarioById(ExistingUserId, ValidDto, MockImageFile.Object);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
        #endregion

        #region MediatorFails_ShouldReturn500InternalServerError()
        [Fact]
        public async Task ModificarUsuarioById_MediatorFails_ShouldReturn500InternalServerError()
        {
            // ARRANGE
            var mediatorException = new Exception("Error de validación o DB en el Handler.");

            MockCloudinaryService
                .Setup(s => s.SubirImagen(It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(ExpectedImageUrl);

            MockMediator
                .Setup(m => m.Send(It.IsAny<ModificarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(mediatorException);

            // ACT
            var result = await Controller.ModificarUsuarioById(ExistingUserId, ValidDto, MockImageFile.Object);

            // ASSERT
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
        #endregion
    }
}
