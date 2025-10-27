
namespace Usuarios.Application.DTOs
{
    public class CrearActUsuarioDTO
    {
        public required string IdUsuario { get; set; } = default!;
        public required string Accion { get; set; } = default!;
    }
}
