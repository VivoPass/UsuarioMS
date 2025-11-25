
namespace Usuarios.Application.DTOs
{
    /// <summary>
    /// DTO utilizado para transferir los datos necesarios para crear un nuevo usuario en el sistema.
    /// Este objeto se recibe generalmente desde un formulario (FromForm) en el endpoint de creación.
    /// </summary>
    public class CrearUsuarioDTO
    {
        /// <summary>
        /// ID del usuario. Se llena después de la creación inicial
        /// del usuario en Keycloak.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre del nuevo usuario.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Apellido del nuevo usuario.
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Fecha de nacimiento del nuevo usuario.
        /// </summary>
        public DateOnly FechaNacimiento { get; set; }

        /// <summary>
        /// Correo electrónico del nuevo usuario. Debe ser único y se usa para el inicio de sesión.
        /// </summary>
        public string Correo { get; set; }

        /// <summary>
        /// Número de teléfono del nuevo usuario.
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Dirección física del nuevo usuario.
        /// </summary>
        public string Direccion { get; set; }

        /// <summary>
        /// URL de la foto de perfil del usuario. Este campo es opcional (`string?`) y
        /// es llenado después de subir la imagen a Cloudinary.
        /// </summary>
        public string? FotoPerfil { get; set; }

        /// <summary>
        /// Rol inicial que se asignará al usuario (ej. "Cliente", "Editor").
        /// </summary>
        public string Rol { get; set; }
    }
}
