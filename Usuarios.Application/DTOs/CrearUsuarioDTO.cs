
namespace Usuarios.Application.DTOs
{
    public class CrearUsuarioDTO
    {
        public string Nombre { get; init; }
        public string Apellido { get; init; }
        public DateOnly FechaNacimiento { get; init; }
        public string Correo { get; init; }
        public string Rol { get; set; }
    }
}
