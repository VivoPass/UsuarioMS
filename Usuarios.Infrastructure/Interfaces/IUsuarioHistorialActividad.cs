using MongoDB.Bson;

namespace Usuarios.Infrastructure.Interfaces
{
    public interface IUsuarioHistorialActividad
    {
        Task AgregarHistAct(BsonDocument actUsuario);
        Task<List<BsonDocument>> GetTodosHistAct();
        Task<List<BsonDocument>> GetByIdUsuarioHistAct(string idUsuario, DateTime timestamp);
    }
}
