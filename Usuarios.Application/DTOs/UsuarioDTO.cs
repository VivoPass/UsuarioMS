
namespace Usuarios.Application.DTOs
{
    /// <summary>
    /// DTO que representa la información detallada de un Usuario.
    /// Se utiliza para la respuesta de las consultas (Queries) de obtención de usuario.
    /// </summary>
    public class UsuarioDTO
    {
        /// <summary>
        /// ID único del usuario (obtenido de Keycloak).
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; init; }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string Apellido { get; init; }

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        public DateOnly FechaNacimiento { get; init; }

        /// <summary>
        /// Correo electrónico del usuario (utilizado para el inicio de sesión).
        /// </summary>
        public string Correo { get; init; }

        /// <summary>
        /// Número de teléfono del usuario.
        /// </summary>
        public string Telefono { get; init; }

        /// <summary>
        /// Dirección física o de residencia del usuario.
        /// </summary>
        public string Direccion { get; init; }

        /// <summary>
        /// URL de la imagen de perfil del usuario, alojada en Cloudinary.
        /// </summary>
        public string FotoPerfil { get; init; }

        /// <summary>
        /// Identificador del rol del usuario (obtenido de Keycloak).
        /// </summary>
        public string Rol { get; set; }

    }
}
