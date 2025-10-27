
namespace Usuarios.Application.DTOs
{
    public class UsuarioDTO
    {
        public string Id { get; init; }
        public string Nombre { get; init; }
        public string Apellido { get; init; }
        public DateOnly FechaNacimiento { get; init; }
        public string Correo { get; init; }
        public string Rol { get; set; }

    }
}
