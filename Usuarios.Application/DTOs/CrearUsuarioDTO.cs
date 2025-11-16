
namespace Usuarios.Application.DTOs
{
    public class CrearUsuarioDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string? FotoPerfil { get; set; }
        public string Rol { get; set; }
    }
}
