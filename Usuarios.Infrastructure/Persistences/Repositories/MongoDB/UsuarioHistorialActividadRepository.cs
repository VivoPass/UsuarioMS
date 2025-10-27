using MongoDB.Bson;
using MongoDB.Driver;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    public class UsuarioHistorialActividadRepository : IUsuarioHistorialActividad
    {
        private readonly IMongoCollection<BsonDocument> UsuarioHistActCollection;

        public UsuarioHistorialActividadRepository(MongoDBConfig mongoConfig)
        {
            UsuarioHistActCollection = mongoConfig.db.GetCollection<BsonDocument>("historial_act_usuarios");
        }

        #region AgregarHistAct(BsonDocument actUsuario)
        public async Task AgregarHistAct(BsonDocument actUsuario)
        {
            try
            {
                await UsuarioHistActCollection.InsertOneAsync(actUsuario);
            }
            catch (Exception ex)
            {
                throw new HistActRepositoryException(ex);
            }
        }
        #endregion

        #region GetTodosHistAct()
        public async Task<List<BsonDocument>> GetTodosHistAct()
        {
            try
            {
                var bsonUserActivity = await UsuarioHistActCollection.Find(new BsonDocument()).ToListAsync();

                if (bsonUserActivity == null || !bsonUserActivity.Any())
                {
                    return new List<BsonDocument>();
                }

                return bsonUserActivity;
            }
            catch (Exception ex)
            {
                throw new HistActRepositoryException(ex);
            }
        }
        #endregion

        #region GetByIdUsuarioHistAct(string idUsuario, DateTime timestamp)
        public async Task<List<BsonDocument>> GetByIdUsuarioHistAct(string idUsuario, DateTime timestamp)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("_idUsuario", idUsuario),
                    Builders<BsonDocument>.Filter.Gte("timestamp", timestamp)
                );

                var bsonUserActivity = await UsuarioHistActCollection.Find(filter).ToListAsync();

                if (bsonUserActivity == null || !bsonUserActivity.Any())
                {
                    return new List<BsonDocument>();
                }

                return bsonUserActivity;
            }
            catch (Exception ex)
            {
                throw new HistActRepositoryException(ex);
            }
        }
        #endregion

    }
}
