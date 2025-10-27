using Usuarios.Domain.Aggregates;
using MongoDB.Bson;

namespace Usuarios.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task CrearUsuario(Usuario usuario);
        Task ModificarUsuario(Usuario usuario);
        Task<Usuario?> GetByCorreo(string correo);
        Task<Usuario?> GetById(string id);
        Task<List<Usuario>> GetTodos();

    }
}
