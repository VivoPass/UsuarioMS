using MongoDB.Bson;
using MongoDB.Driver;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Factories;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Configurations;

namespace Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    public class RolRepository: IRolRepository
    {
        private readonly IMongoCollection<BsonDocument> RolCollection;
        private readonly IRolFactory RolFactory;

        public RolRepository(MongoDBConfig mongoConfig, IRolFactory rolFactory)
        {
            RolCollection = mongoConfig.db.GetCollection<BsonDocument>("roles");
            RolFactory = rolFactory ?? throw new RolFactoryNullException();
        }

        #region Task<Rol?> GetById(string rolId)
        public async Task<Rol?> GetById(string rolId)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", rolId);
                var bsonRol = await RolCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonRol == null)
                {
                    return null;
                }

                var idRol = new VORolId(rolId);
                var nombre = new VORolNombre(bsonRol["nombre"].AsString);

                var rol = RolFactory.Load(
                    idRol,
                    nombre
                );

                return rol;
            }
            catch (Exception ex)
            {
                throw new RolRepositoryException(ex);
            }
        }
        #endregion

        #region Task<Rol?> GetByNombre(string nombreRol)
        public async Task<Rol?> GetByNombre(string nombreRol)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("nombre", nombreRol);
                var bsonRol = await RolCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonRol == null)
                {
                    return null;
                }

                var idRol = new VORolId(bsonRol["_id"].AsString);
                var nombre = new VORolNombre(nombreRol);

                var rol = RolFactory.Load(
                    idRol,
                    nombre
                );

                return rol;
            }
            catch (Exception ex)
            {
                throw new RolRepositoryException(ex);
            }
        }
        #endregion

        #region Task<List<Rol>> GetTodos()
        public async Task<List<Rol>> GetTodos()
        {
            try
            {
                var bsonRoles = await RolCollection.Find(_ => true).ToListAsync();

                if (bsonRoles == null || !bsonRoles.Any())
                {
                    return new List<Rol>();
                }

                var listaRoles = bsonRoles.Select(bsonRol =>
                {
                    var idRol = new VORolId(bsonRol["_id"].AsString);
                    var nombre = new VORolNombre(bsonRol["nombre"].AsString);

                    return RolFactory.Load(
                        idRol,
                        nombre
                    );
                }).ToList();

                return listaRoles;
            }
            catch (Exception ex)
            {
                throw new RolRepositoryException(ex);
            }
        }
        #endregion
    }
}
