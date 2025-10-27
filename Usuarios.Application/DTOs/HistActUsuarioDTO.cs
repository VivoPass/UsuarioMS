
namespace Usuarios.Application.DTOs
{
    public class HistActUsuarioDTO
    {
        public string Id { get; set; } = default!;
        public required string IdUsuario { get; set; } = default!;
        public required string Accion { get; set; } = default!;
        public DateTime Timestamp { get; set; }
    }
}
