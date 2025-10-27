using Usuarios.Domain.Entities;

namespace Usuarios.Domain.Interfaces
{
    public interface IRolRepository
    {
        Task<Rol?> GetByNombre(string nombre);
        Task<Rol?> GetById(string id);
        Task<List<Rol>> GetTodos();
    }
}
