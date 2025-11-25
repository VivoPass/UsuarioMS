using log4net;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Commands;
using Usuarios.Application.DTOs;
using Usuarios.Application.Events;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Infrastructure.Queries;

namespace Usuarios.API.Controllers
{
    /// <summary>
    /// Controlador API para la gestión de usuarios, roles y actividades relacionadas.
    /// </summary>
    /// <remarks>
    /// Este controlador actúa como la puerta de entrada a las operaciones del microservicio de Usuarios,
    /// utilizando CQRS (Mediator) para separar comandos (escritura) y queries (lectura),
    /// y MassTransit (PublishEndpoint) para la comunicación asíncrona de eventos.
    /// </remarks>
    [ApiController]
    [Route("api/Usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator Mediator;
        private readonly IPublishEndpoint PublishEndpoint;
        private readonly ICloudinaryService CloudinaryService;
        private readonly ILog _logger;

        public UsuariosController(IMediator mediator, IPublishEndpoint publishEndpoint, ICloudinaryService cloudinaryService, ILog logger)
        {
            Mediator = mediator ?? throw new MediatorNullException();
            PublishEndpoint = publishEndpoint ?? throw new PublishEndpointNullException();
            CloudinaryService = cloudinaryService ?? throw new CloudinaryServiceNullException();
            _logger = logger ?? throw new LoggerNullException();
        }

        #region CrearUsuario([FromBody] CrearUsuarioDTO usuarioDto)
        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="usuarioDto">Datos del nuevo usuario (incluye campos de formulario).</param>
        /// <param name="imagen">Archivo de imagen de perfil enviado en el formulario.</param>
        /// <returns>Retorna el ID del usuario creado (HTTP 201 Created).</returns>
        [HttpPost("crearUsuario")]
        public async Task<IActionResult> CrearUsuario([FromForm] CrearUsuarioDTO usuarioDto, IFormFile? imagen)
        {
            try
            {
                if (imagen != null)
                {
                    _logger.Debug($"Subiendo imagen: {imagen.FileName} para el DTO.");
                    using var stream = imagen.OpenReadStream();
                    var url = await CloudinaryService.SubirImagen(stream, imagen.FileName);
                    usuarioDto.FotoPerfil = url;
                    _logger.Debug($"URL de imagen obtenida: {url}. DTO listo para el Command.");
                }

                var userId = await Mediator.Send(new CrearUsuarioCommand(usuarioDto));
                if (userId == null)
                {
                    _logger.Warn("La creación del usuario falló en el Handler (ID nulo).");
                    return BadRequest("No se pudo crear el usuario.");
                }

                _logger.Info($"Usuario creado exitosamente con ID: {userId}.");
                return CreatedAtAction(nameof(CrearUsuario), new { id = userId }, new
                {
                    id = userId
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al crear usuario. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region ModificarUsuarioById([FromQuery] string id, [FromBody] ModificarUsuarioDTO usuarioDto)
        /// <summary>
        /// Modifica un usuario existente.
        /// </summary>
        /// <param name="id">ID del usuario a modificar (FromQuery).</param>
        /// <param name="usuarioDto">Datos del usuario a modificar.</param>
        /// <param name="nuevaImagen">Nueva imagen de perfil opcional.</param>
        /// <returns>HTTP 200 OK si la actualización fue exitosa, 404 Not Found si el usuario no pudo ser actualizado.</returns>
        [HttpPatch("modificarUsuario")]
        public async Task<IActionResult> ModificarUsuarioById([FromQuery] string id, [FromForm] ModificarUsuarioDTO usuarioDto, IFormFile? nuevaImagen)
        {
            try
            {
                if (nuevaImagen != null)
                {
                    _logger.Debug($"Subiendo nueva imagen para usuario ID: {id}.");
                    using var stream = nuevaImagen.OpenReadStream();
                    var url = await CloudinaryService.SubirImagen(stream, nuevaImagen.FileName);
                    usuarioDto.FotoPerfil = url;
                }

                _logger.Debug($"Enviando comando ModificarUsuarioCommand para ID: {id}.");
                var result = await Mediator.Send(new ModificarUsuarioCommand(id, usuarioDto));

                if (!result)
                {
                    _logger.Warn($"Modificación fallida para el usuario ID: {id}. Posiblemente ID no encontrado en el Handler.");
                    return NotFound("El usuario no pudo ser actualizado.");
                }

                _logger.Info($"Usuario ID: {id} actualizado exitosamente.");
                return Ok("Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al modificar usuario ID: {id}. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetUsuarioByCorreo ([FromQuery] string correo)
        /// <summary>
        /// Obtiene los detalles de un usuario a partir de su correo electrónico.
        /// </summary>
        /// <param name="correo">Correo electrónico del usuario.</param>
        /// <returns>Retorna <see cref="UsuarioDTO"/> si es encontrado (HTTP 200 OK), o 404 Not Found.</returns>
        [HttpGet("getUsuarioByCorreo")]
        public async Task<IActionResult> GetUsuarioByCorreo ([FromQuery] string correo)
        {
            try
            {
                var query = new GetUsuarioByCorreoQuery(correo);
                var userDto = await Mediator.Send(query);
                if (userDto == null)
                {
                    _logger.Warn($"Búsqueda fallida: Usuario con correo {correo} no encontrado.");
                    return NotFound($"No se encontró un usuario con el email {correo}");
                }

                _logger.Debug($"Usuario con correo {correo} encontrado.");
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener usuario por correo {correo}. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetUsuarioById([FromQuery] string id)
        /// <summary>
        /// Obtiene los detalles de un usuario a partir de su ID.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Retorna <see cref="UsuarioDTO"/> si es encontrado (HTTP 200 OK), o 404 Not Found.</returns>
        [HttpGet("getUsuarioById")]
        public async Task<IActionResult> GetUsuarioById([FromQuery] string id)
        {
            try
            {
                var query = new GetUsuarioByIdQuery(id);
                var userDto = await Mediator.Send(query);

                if (userDto == null)
                {
                    _logger.Warn($"Búsqueda fallida: Usuario con ID {id} no encontrado.");
                    return NotFound($"No se encontró un usuario con el id {id}");
                }

                _logger.Debug($"Usuario con ID {id} encontrado.");
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener usuario por ID {id}. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetTodosUsuarios()
        /// <summary>
        /// Obtiene la lista completa de todos los usuarios registrados.
        /// </summary>
        /// <returns>Retorna una lista de <see cref="UsuarioDTO"/> (HTTP 200 OK) o 404 Not Found si la lista está vacía.</returns>
        [HttpGet("getTodosUsuarios")]
        public async Task<IActionResult> GetTodosUsuarios()
        {
            try
            {
                var query = new GetTodosUsuariosQuery();
                var users = await Mediator.Send(query);
                if (users == null || !users.Any())
                {
                    _logger.Info("No se encontraron usuarios en el repositorio.");
                    return NotFound("No se encontraron usuarios.");
                }

                _logger.Debug($"Usuarios obtenidos: {users.Count} resultados.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener todos los usuarios. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetRolByNombre([FromQuery] string nombre)
        /// <summary>
        /// Obtiene los detalles de un rol a partir de su nombre.
        /// </summary>
        /// <param name="nombre">Nombre del rol.</param>
        /// <returns>Retorna <see cref="RolDTO"/> si es encontrado (HTTP 200 OK), o 404 Not Found.</returns>
        [HttpGet("getRolByNombre")]
        public async Task<IActionResult> GetRolByNombre([FromQuery] string nombre)
        {
            try
            {
                var query = new GetRolByNombreQuery(nombre);
                var rolDto = await Mediator.Send(query);
                if (rolDto == null)
                {
                    _logger.Warn($"Búsqueda fallida: Rol con nombre {nombre} no encontrado.");
                    return NotFound($"No se encontró un rol con el nombre {nombre}");
                }

                _logger.Debug($"Rol con nombre {nombre} encontrado.");
                return Ok(rolDto);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener rol por nombre {nombre}. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetRolById([FromQuery] string id)
        /// <summary>
        /// Obtiene los detalles de un rol a partir de su ID.
        /// </summary>
        /// <param name="id">ID del rol.</param>
        /// <returns>Retorna <see cref="RolDTO"/> si es encontrado (HTTP 200 OK), o 404 Not Found.</returns>
        [HttpGet("getRolById")]
        public async Task<IActionResult> GetRolById([FromQuery] string id)
        {
            try
            {
                var query = new GetRolByIdQuery(id);
                var rolDto = await Mediator.Send(query);

                if (rolDto == null)
                {
                    _logger.Warn($"Búsqueda fallida: Rol con ID {id} no encontrado.");
                    return NotFound($"No se encontró un rol con el id {id}");
                }

                _logger.Debug($"Rol con ID {id} encontrado.");
                return Ok(rolDto);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener rol por ID {id}. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetTodosRoles()
        /// <summary>
        /// Obtiene la lista completa de todos los roles disponibles.
        /// </summary>
        /// <returns>Retorna una lista de <see cref="RolDTO"/> (HTTP 200 OK) o 404 Not Found si la lista está vacía.</returns>
        [HttpGet("getTodosRoles")]
        public async Task<IActionResult> GetTodosRoles()
        {
            try
            {
                var query = new GetTodosRolesQuery();
                var roles = await Mediator.Send(query);
                if (roles == null || !roles.Any())
                {
                    _logger.Info("No se encontraron roles en el repositorio.");
                    return NotFound("No se encontraron roles.");
                }

                _logger.Debug($"Roles obtenidos: {roles.Count} resultados.");
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener todos los roles. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetHistActByUsuarioId([FromQuery] string id)
        /// <summary>
        /// Obtiene el historial de actividad de un usuario específico.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Retorna una lista de actividades (HTTP 200 OK) o 404 Not Found si no hay actividades.</returns>
        [HttpGet("activity")]
        public async Task<IActionResult> GetHistActByUsuarioId([FromQuery] string id)
        {
            try
            {
                var activities = await Mediator.Send(new HistActUsuarioQuery(id, DateTime.MaxValue));
                if (activities == null || !activities.Any())
                {
                    _logger.Info($"No se encontraron actividades para el usuario ID {id}.");
                    return NotFound($"No se encontraron actividades para el usuario con ID {id}");
                }

                _logger.Debug($"Actividades obtenidas para el usuario ID {id}: {activities} resultados.");
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al obtener historial de actividad para usuario ID {id}. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region AgregarAct([FromBody] CrearActUsuarioDTO createUserActivityDto)
        /// <summary>
        /// Publica un evento de actividad del usuario en el bus de mensajes.
        /// </summary>
        /// <param name="createUserActivityDto">Detalles de la actividad a registrar.</param>
        /// <returns>HTTP 200 OK si el evento fue publicado correctamente.</returns>
        [HttpPost("publishActivity")]
        public async Task<IActionResult> AgregarAct([FromBody] CrearActUsuarioDTO createUserActivityDto)
        {
            try
            {
                _logger.Debug($"Publicando evento de actividad para Usuario: {createUserActivityDto.IdUsuario}. Acción: {createUserActivityDto.Accion}.");
                await PublishEndpoint.Publish(new HistorialActividadEvent(
                    createUserActivityDto.IdUsuario,
                    createUserActivityDto.Accion,
                    DateTime.UtcNow
                ));

                _logger.Info("Evento de actividad publicado correctamente.");
                return Ok("Evento de actividad publicado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al publicar evento de actividad. Mensaje: {ex.Message}", ex);
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

    }
}