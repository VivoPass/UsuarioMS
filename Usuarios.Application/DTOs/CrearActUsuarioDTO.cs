
namespace Usuarios.Application.DTOs
{
    /// <summary>
    /// DTO utilizado para la publicación de una nueva actividad en el historial de un usuario.
    /// Este DTO se consume en el endpoint 'publishActivity'.
    /// </summary>
    public class CrearActUsuarioDTO
    {
        /// <summary>
        /// ID único del usuario al que se asocia la actividad (ej. ID de Keycloak).
        /// </summary>
        public required string IdUsuario { get; set; } = default!;

        /// <summary>
        /// Descripción de la acción realizada por el usuario (ej. "Log in", "Datos del perfil modificados").
        /// </summary>
        public required string Accion { get; set; } = default!;
    }
}
