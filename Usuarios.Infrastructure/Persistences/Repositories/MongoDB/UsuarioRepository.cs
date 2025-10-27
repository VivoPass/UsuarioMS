using MongoDB.Bson;
using MongoDB.Driver;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Configurations;

namespace Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<BsonDocument> UsuarioCollection;
        private readonly IUsuarioFactory UsuarioFactory;

        public UsuarioRepository(MongoDBConfig mongoConfig, IUsuarioFactory usuarioFactory)
        {
            UsuarioCollection = mongoConfig.db.GetCollection<BsonDocument>("usuarios");
            UsuarioFactory = usuarioFactory ?? throw new UsuarioFactoryNullException();
        }

        #region CrearUsuario(Usuario usuario)
        public async Task CrearUsuario(Usuario usuario)
        {
            try
            {
                var bsonUser = new BsonDocument
                {
                    { "_id", usuario.Id.Valor },
                    { "nombre", usuario.Nombre.Valor },
                    { "apellido", usuario.Apellido.Valor },
                    { "fechaNacimiento", usuario.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)},
                    { "correo", usuario.Correo.Valor },
                    { "rol", usuario.Rol.Valor },
                    { "createdAt", DateTime.UtcNow },
                    { "updatedAt", DateTime.UtcNow }
                };

                await UsuarioCollection.InsertOneAsync(bsonUser);
            }
            catch (Exception ex)
            {
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region GetByCorreo(string correo)
        public async Task<Usuario?> GetByCorreo(string correo)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("correo", correo);
                var bsonUsuario = await UsuarioCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonUsuario == null)
                {
                    return null;
                }

                var idUsuario = new VOId(bsonUsuario["_id"].AsString);
                var nombre = new VONombre(bsonUsuario["nombre"].AsString);
                var apellido = new VOApellido(bsonUsuario["apellido"].AsString);

                var fechaNacimientoDateTime = bsonUsuario["fechaNacimiento"].ToUniversalTime();
                var fechaNacimientoDateOnly = DateOnly.FromDateTime(fechaNacimientoDateTime);
                var fechaNacimiento = new VOFechaNacimiento(fechaNacimientoDateOnly);

                var correoUsuario = new VOCorreo(correo);
                var rol = new VORolId(bsonUsuario["rol"].AsString);

                var usuario = UsuarioFactory.Load(
                    idUsuario,
                    nombre,
                    apellido,
                    fechaNacimiento,
                    correoUsuario,
                    rol
                );

                return usuario;
            }
            catch (Exception ex)
            {
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region GetById(string id)
        public async Task<Usuario?> GetById(string id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var bsonUsuario = await UsuarioCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonUsuario == null)
                {
                    return null;
                }

                var idUsuario = new VOId(id);
                var nombre = new VONombre(bsonUsuario["nombre"].AsString);
                var apellido = new VOApellido(bsonUsuario["apellido"].AsString);

                var fechaNacimientoDateTime = bsonUsuario["fechaNacimiento"].ToUniversalTime();
                var fechaNacimientoDateOnly = DateOnly.FromDateTime(fechaNacimientoDateTime);
                var fechaNacimiento = new VOFechaNacimiento(fechaNacimientoDateOnly);

                var correo = new VOCorreo(bsonUsuario["correo"].AsString);
                var rol = new VORolId(bsonUsuario["rol"].AsString);

                var usuario = UsuarioFactory.Load(
                    idUsuario,
                    nombre,
                    apellido,
                    fechaNacimiento,
                    correo,
                    rol
                );

                return usuario;
            }
            catch (Exception ex)
            {
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region ModificarUsuario(Usuario usuario)
        public async Task ModificarUsuario(Usuario usuario)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", usuario.Id.Valor);
                var update = Builders<BsonDocument>.Update
                    .Set("nombre", usuario.Nombre.Valor)
                    .Set("apellido", usuario.Apellido.Valor);

                var result = await UsuarioCollection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    return;
                }

            }
            catch (Exception ex)
            {
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region GetTodos()
        public async Task<List<Usuario>> GetTodos()
        {
            try
            {
                var bsonUsuarios = await UsuarioCollection.Find(_ => true).ToListAsync();

                if (bsonUsuarios == null || !bsonUsuarios.Any())
                {
                    return new List<Usuario>();
                }

                var listaUsuarios = bsonUsuarios.Select(bsonUsuario =>
                {
                    var idUsuario = new VOId(bsonUsuario["_id"].AsString);
                    var nombre = new VONombre(bsonUsuario["nombre"].AsString);
                    var apellido = new VOApellido(bsonUsuario["apellido"].AsString);

                    var fechaNacimientoDateTime = bsonUsuario["fechaNacimiento"].ToUniversalTime();
                    var fechaNacimientoDateOnly = DateOnly.FromDateTime(fechaNacimientoDateTime);
                    var fechaNacimiento = new VOFechaNacimiento(fechaNacimientoDateOnly);

                    var correo = new VOCorreo(bsonUsuario["correo"].AsString);
                    var rol = new VORolId(bsonUsuario["rol"].AsString);

                    return UsuarioFactory.Load(
                        idUsuario,
                        nombre,
                        apellido,
                        fechaNacimiento,
                        correo,
                        rol
                    );
                }).ToList();

                return listaUsuarios;
            }
            catch (Exception ex)
            {
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

    } 
}
