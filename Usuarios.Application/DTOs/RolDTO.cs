
namespace Usuarios.Application.DTOs
{
    /// <summary>
    /// DTO que representa la información de un Rol dentro del sistema.
    /// </summary>
    public class RolDTO
    {
        /// <summary>
        /// ID único interno del Rol en la base de datos del microservicio de Usuarios.
        /// </summary>
        public string IdRol { get; set; }

        /// <summary>
        /// Nombre descriptivo del Rol (ej. "Administrador", "Cliente").
        /// </summary>
        public string NombreRol { get; set; }

        /// <summary>
        /// ID del Rol asociado en el sistema de gestión de identidades externo (Keycloak).
        /// </summary>
        public string IdRolKeycloak { get; set; }
    }
}
