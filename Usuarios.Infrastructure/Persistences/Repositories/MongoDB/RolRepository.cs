using MongoDB.Bson;
using MongoDB.Driver;
using log4net;
using Usuarios.Domain.Entities;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Configurations;

namespace Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    /// <summary>
    /// Proporciona la capa de acceso a datos para la entidad Rol sobre la base de datos MongoDB.
    /// </summary>
    /// <remarks>
    ///Esta clase implementa el patrón Repository para desacoplar la lógica del Dominio 
    /// de la tecnología de persistencia (MongoDB), permitiendo la reconstrucción de la Entidad Rol
    /// a partir de documentos BSON mediante la fábrica de Dominio.
    /// </remarks>
    public class RolRepository: IRolRepository
    {
        private readonly IMongoCollection<BsonDocument> RolCollection;
        private readonly IRolFactory RolFactory;
        private readonly ILog Logger;

        public RolRepository(MongoDBConfig mongoConfig, IRolFactory rolFactory, ILog logger)
        {
            RolCollection = mongoConfig.db.GetCollection<BsonDocument>("roles");
            RolFactory = rolFactory ?? throw new RolFactoryNullException();
            Logger = logger ?? throw new LoggerNullException();
        }

        #region Task<Rol?> GetById(string rolId)
        /// <summary>
        /// Permite obtener un Rol por su identificador único.
        /// </summary>
        /// <param name="rolId">El ID único del rol.</param>
        /// <returns>Retorna la entidad <see cref="Rol"/> si existe, o null.</returns>
        public async Task<Rol?> GetById(string rolId)
        {
            Logger.Debug($"Buscando rol por ID: {rolId} en MongoDB.");
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", rolId);
                var bsonRol = await RolCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonRol == null)
                {
                    Logger.Info($"Búsqueda por ID {rolId} no encontró documento.");
                    return null;
                }

                var idRol = new VORolId(rolId);
                var nombre = new VORolNombre(bsonRol["nombre"].AsString);

                var rol = RolFactory.Load(
                    idRol,
                    nombre
                );

                Logger.Debug($"Rol ID {rolId} encontrado y reconstruido.");
                return rol;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al obtener rol por ID {rolId} de MongoDB.", ex);
                throw new RolRepositoryException(ex);
            }
        }
        #endregion

        #region Task<Rol?> GetByNombre(string nombreRol)
        /// <summary>
        /// Permite obtener un Rol para facilitar la asignación o validación de roles por su nombre.
        /// </summary>
        /// <param name="nombreRol">El nombre del rol.</param>
        /// <returns>Retorna la entidad <see cref="Rol"/> si existe, o null.</returns>
        public async Task<Rol?> GetByNombre(string nombreRol)
        {
            Logger.Debug($"Buscando rol por nombre: {nombreRol} en MongoDB.");
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("nombre", nombreRol);
                var bsonRol = await RolCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonRol == null)
                {
                    Logger.Info($"Búsqueda por nombre {nombreRol} no encontró documento.");
                    return null;
                }

                var idRol = new VORolId(bsonRol["_id"].AsString);
                var nombre = new VORolNombre(nombreRol);

                var rol = RolFactory.Load(
                    idRol,
                    nombre
                );

                Logger.Debug($"Rol con nombre {nombreRol} encontrado y reconstruido.");
                return rol;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al obtener rol por nombre {nombreRol} de MongoDB.", ex);
                throw new RolRepositoryException(ex);
            }
        }
        #endregion

        #region Task<List<Rol>> GetTodos()
        /// <summary>
        /// Permite recuperar la lista completa de todos los roles, mapeándolos de BSON a la entidad de Dominio.
        /// </summary>
        /// <returns>Retorna una lista de todas las entidades <see cref="Rol"/>.</returns>
        public async Task<List<Rol>> GetTodos()
        {
            Logger.Debug("Recuperando todos los documentos de rol de MongoDB.");
            try
            {
                var bsonRoles = await RolCollection.Find(_ => true).ToListAsync();

                if (bsonRoles == null || !bsonRoles.Any())
                {
                    Logger.Info("No se encontraron documentos de rol en MongoDB.");
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

                Logger.Debug($"Recuperados {listaRoles.Count} roles y reconstruidos.");
                return listaRoles;
            }
            catch (Exception ex)
            {
                Logger.Error("Error al obtener todos los roles de MongoDB.", ex);
                throw new RolRepositoryException(ex);
            }
        }
        #endregion
    }
}
