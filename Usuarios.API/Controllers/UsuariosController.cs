using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Usuarios.Application.Commands;
using Usuarios.Application.DTOs;
using Usuarios.Application.Events;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Queries;

namespace Usuarios.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator Mediator;
        private readonly IPublishEndpoint PublishEndpoint;

        public UsuariosController(IMediator mediator, IPublishEndpoint publishEndpoint)
        {
            Mediator = mediator ?? throw new MediatorNullException();
            PublishEndpoint = publishEndpoint ?? throw new PublishEndpointNullException();
        }

        #region CrearUsuario([FromBody] CrearUsuarioDTO usuarioDto)
        [HttpPost("crearUsuario")]
        public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO usuarioDto)
        {
            try
            {
                var userId = await Mediator.Send(new CrearUsuarioCommand(usuarioDto));
                if (userId == null)
                {
                    return BadRequest("No se pudo crear el usuario.");
                }

                return CreatedAtAction(nameof(CrearUsuario), new { id = userId }, new
                {
                    id = userId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region ModificarUsuarioById([FromQuery] string id, [FromBody] ModificarUsuarioDTO usuarioDto)
        [HttpPatch("modificarUsuario")]
        public async Task<IActionResult> ModificarUsuarioById([FromQuery] string id, [FromBody] ModificarUsuarioDTO usuarioDto)
        {
            try
            {
                var result = await Mediator.Send(new ModificarUsuarioCommand(usuarioDto, id));
                if (!result)
                {
                    return NotFound("El usuario no pudo ser actualizado.");
                }

                return Ok("Usuario actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetUsuarioByCorreo ([FromQuery] string correo)
        [HttpGet("getUsuarioByCorreo")]
        public async Task<IActionResult> GetUsuarioByCorreo ([FromQuery] string correo)
        {
            try
            {
                var query = new GetUsuarioByCorreoQuery(correo);
                var userDto = await Mediator.Send(query);
                if (userDto == null)
                {
                    return NotFound($"No se encontró un usuario con el email {correo}");
                }

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetUsuarioById([FromQuery] string id)
        [HttpGet("getUsuarioById")]
        public async Task<IActionResult> GetUsuarioById([FromQuery] string id)
        {
            try
            {
                var query = new GetUsuarioByIdQuery(id);
                var userDto = await Mediator.Send(query);

                if (userDto == null)
                {
                    return NotFound($"No se encontró un usuario con el id {id}");
                }

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetTodosUsuarios()
        [HttpGet("getTodosUsuarios")]
        public async Task<IActionResult> GetTodosUsuarios()
        {
            try
            {
                var query = new GetTodosUsuariosQuery();
                var users = await Mediator.Send(query);
                if (users == null || !users.Any())
                {
                    return NotFound("No se encontraron usuarios.");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetRolByNombre([FromQuery] string nombre)
        [HttpGet("getRolByNombre")]
        public async Task<IActionResult> GetRolByNombre([FromQuery] string nombre)
        {
            try
            {
                var query = new GetRolByNombreQuery(nombre);
                var rolDto = await Mediator.Send(query);
                if (rolDto == null)
                {
                    return NotFound($"No se encontró un rol con el nombre {nombre}");
                }

                return Ok(rolDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetRolById([FromQuery] string id)
        [HttpGet("getRolById")]
        public async Task<IActionResult> GetRolById([FromQuery] string id)
        {
            try
            {
                var query = new GetRolByIdQuery(id);
                var rolDto = await Mediator.Send(query);

                if (rolDto == null)
                {
                    return NotFound($"No se encontró un rol con el id {id}");
                }

                return Ok(rolDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetTodosRoles()
        [HttpGet("getTodosRoles")]
        public async Task<IActionResult> GetTodosRoles()
        {
            try
            {
                var query = new GetTodosRolesQuery();
                var roles = await Mediator.Send(query);
                if (roles == null || !roles.Any())
                {
                    return NotFound("No se encontraron roles.");
                }

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region GetHistActByUsuarioId([FromQuery] string id)
        [HttpGet("activity")]
        public async Task<IActionResult> GetHistActByUsuarioId([FromQuery] string id)
        {
            try
            {
                var activities = await Mediator.Send(new HistActUsuarioQuery(id, DateTime.MaxValue));
                if (activities == null || !activities.Any())
                {
                    return NotFound($"No se encontraron actividades para el usuario con ID {id}");
                }

                return Ok(activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region AgregarAct([FromBody] CrearActUsuarioDTO createUserActivityDto)
        [HttpPost("publishActivity")]
        public async Task<IActionResult> AgregarAct([FromBody] CrearActUsuarioDTO createUserActivityDto)
        {
            try
            {
                await PublishEndpoint.Publish(new HistorialActividadEvent(
                    createUserActivityDto.IdUsuario,
                    createUserActivityDto.Accion,
                    DateTime.UtcNow
                ));

                return Ok("Evento de actividad publicado correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

    }
}