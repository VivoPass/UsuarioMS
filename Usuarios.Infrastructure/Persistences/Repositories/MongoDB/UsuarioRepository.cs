using log4net;
using log4net.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Exceptions;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    /// <summary>
    /// Proporciona la capa de acceso a datos para la entidad Usuario sobre la base de datos MongoDB.
    /// </summary>
    /// <remarks>
    ///Esta clase implementa el patrón Repository para desacoplar la lógica de negocio 
    /// (Dominio/Application) de la tecnología de persistencia (MongoDB), facilitando el testing, 
    /// el mapeo de documentos BSON a Entidades de Dominio, y la gestión específica de NoSQL.
    /// </remarks>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMongoCollection<BsonDocument> UsuarioCollection;
        private readonly IUsuarioFactory UsuarioFactory;
        private readonly ILog Logger;
        private readonly IAuditoriaRepository AuditoriaRepository;

        public UsuarioRepository(MongoDBConfig mongoConfig, IUsuarioFactory usuarioFactory, ILog logger, IAuditoriaRepository auditoriaRepository)
        {
            UsuarioCollection = mongoConfig.db.GetCollection<BsonDocument>("usuarios");
            UsuarioFactory = usuarioFactory ?? throw new UsuarioFactoryNullException();
            Logger = logger ?? throw new LoggerNullException();
            AuditoriaRepository = auditoriaRepository ?? throw new AuditoriaRepositoryNullException();
        }

        #region CrearUsuario(Usuario usuario)
        /// <summary>
        /// Permite persistir una nueva entidad Usuario como un documento BSON en la colección.
        /// </summary>
        /// <param name="usuario">La entidad Usuario a persistir.</param>
        /// <returns>Tarea completada.</returns>
        public async Task CrearUsuario(Usuario usuario)
        {
            Logger.Info($"Iniciando creación de usuario con ID: {usuario.Id.Valor} en MongoDB.");
            try
            {
                var bsonUser = new BsonDocument
                {
                    { "_id", usuario.Id.Valor },
                    { "nombre", usuario.Nombre.Valor },
                    { "apellido", usuario.Apellido.Valor },
                    { "fechaNacimiento", usuario.FechaNacimiento.Valor.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)},
                    { "correo", usuario.Correo.Valor },
                    {"telefono", usuario.Telefono.Valor},
                    {"direccion", usuario.Direccion.Valor},
                    {"fotoPerfil", usuario.FotoPerfil.Valor ?? ""},
                    { "rol", usuario.Rol.Valor },
                    { "preferencias", BsonArray.Create(usuario.Preferencias.Preferencias) },
                    { "createdAt", DateTime.UtcNow },
                    { "updatedAt", DateTime.UtcNow }
                };

                await UsuarioCollection.InsertOneAsync(bsonUser);
                Logger.Info($"Usuario {usuario.Id.Valor} insertado exitosamente en MongoDB.");
                await AuditoriaRepository.InsertarAuditoriaUsuario(usuario.Id.Valor, "INFO", "USUARIO_REGISTRADO",
                    $"Se registró el usuario con id '{usuario.Id.Valor}' y correo '{usuario.Correo.Valor}'.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al crear usuario {usuario.Id.Valor} en MongoDB.", ex);
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region GetByCorreo(string correo)
        /// <summary>
        /// Permite obtener un Usuario para operaciones de validación o login.
        /// </summary>
        /// <param name="correo">El correo electrónico del usuario.</param>
        /// <returns>Retorna la entidad <see cref="Usuario"/>, reconstruida por la fábrica, o null.</returns>
        public async Task<Usuario?> GetByCorreo(string correo)
        {
            Logger.Debug($"Buscando usuario por correo: {correo} en MongoDB.");
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("correo", correo);
                var bsonUsuario = await UsuarioCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonUsuario == null)
                {
                    Logger.Info($"Búsqueda por correo {correo} no encontró documento.");
                    return null;
                }

                var idUsuario = new VOId(bsonUsuario["_id"].AsString);
                var nombre = new VONombre(bsonUsuario["nombre"].AsString);
                var apellido = new VOApellido(bsonUsuario["apellido"].AsString);

                var fechaNacimientoDateTime = bsonUsuario["fechaNacimiento"].ToUniversalTime();
                var fechaNacimientoDateOnly = DateOnly.FromDateTime(fechaNacimientoDateTime);
                var fechaNacimiento = new VOFechaNacimiento(fechaNacimientoDateOnly);

                var correoUsuario = new VOCorreo(correo);
                var telefono = new VOTelefono(bsonUsuario["telefono"].AsString);
                var direccion = new VODireccion(bsonUsuario["direccion"].AsString);
                var fotoPerfil = new VOFotoPerfil(bsonUsuario["fotoPerfil"].AsString);
                var rol = new VORolKeycloakId(bsonUsuario["rol"].AsString);
                var preferencias = new VOPreferencias(bsonUsuario.GetValue("preferencias", new BsonArray()).AsBsonArray.Select(b => b.AsString).ToList());


                var usuario = UsuarioFactory.Load(
                    idUsuario, nombre, apellido, fechaNacimiento, correoUsuario, telefono, direccion, rol, fotoPerfil, preferencias
                );

                Logger.Debug($"Usuario con correo {correo} encontrado y reconstruido.");
                return usuario;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al obtener usuario por correo {correo} de MongoDB.", ex);
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region GetById(string id)
        /// <summary>
        /// Permite obtener un Usuario por su identificador único.
        /// </summary>
        /// <param name="id">El ID del usuario.</param>
        /// <returns>Retorna la entidad <see cref="Usuario"/> si existe, o null.</returns>
        public async Task<Usuario?> GetById(string id)
        {
            Logger.Debug($"Buscando usuario por ID: {id} en MongoDB.");
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
                var bsonUsuario = await UsuarioCollection.Find(filter).FirstOrDefaultAsync();

                if (bsonUsuario == null)
                {
                    Logger.Info($"Búsqueda por ID {id} no retornó resultados.");
                    return null;
                }

                var idUsuario = new VOId(id);
                var nombre = new VONombre(bsonUsuario["nombre"].AsString);
                var apellido = new VOApellido(bsonUsuario["apellido"].AsString);

                var fechaNacimientoDateTime = bsonUsuario["fechaNacimiento"].ToUniversalTime();
                var fechaNacimientoDateOnly = DateOnly.FromDateTime(fechaNacimientoDateTime);
                var fechaNacimiento = new VOFechaNacimiento(fechaNacimientoDateOnly);

                var correo = new VOCorreo(bsonUsuario["correo"].AsString);
                var telefono = new VOTelefono(bsonUsuario["telefono"].AsString);
                var direccion = new VODireccion(bsonUsuario["direccion"].AsString);
                var fotoPerfil = new VOFotoPerfil(bsonUsuario["fotoPerfil"].AsString);
                var rol = new VORolKeycloakId(bsonUsuario["rol"].AsString);
                var preferencias = new VOPreferencias(bsonUsuario.GetValue("preferencias", new BsonArray()).AsBsonArray.Select(b => b.AsString).ToList());


                var usuario = UsuarioFactory.Load(
                    idUsuario, nombre, apellido, fechaNacimiento, correo, telefono, direccion, rol, fotoPerfil, preferencias
                );

                Logger.Debug($"Usuario ID {id} encontrado y reconstruido.");
                return usuario;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al obtener usuario por ID {id} de MongoDB.", ex);
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region ModificarUsuario(Usuario usuario)
        /// <summary>
        /// Permite actualizar campos no clave de un documento Usuario existente en la colección.
        /// </summary>
        /// <param name="usuario">La entidad Usuario que contiene los valores actualizados.</param>
        /// <returns>Tarea completada.</returns>
        public async Task ModificarUsuario(Usuario usuario)
        {
            Logger.Info($"Iniciando actualización de campos para Usuario ID: {usuario.Id.Valor} en MongoDB.");
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", usuario.Id.Valor);
                var update = Builders<BsonDocument>.Update
                    .Set("nombre", usuario.Nombre.Valor)
                    .Set("apellido", usuario.Apellido.Valor)
                    .Set("telefono", usuario.Telefono.Valor)
                    .Set("direccion", usuario.Direccion.Valor)
                    .Set("fotoPerfil", usuario.FotoPerfil.Valor)
                    .Set("updatedAt", DateTime.UtcNow);

                var result = await UsuarioCollection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    Logger.Warn($"Modificación de Usuario ID: {usuario.Id.Valor} falló. Documento no encontrado o no hubo cambios.");
                    return;
                }

                Logger.Info($"Usuario ID: {usuario.Id.Valor} actualizado. {result.ModifiedCount} documento(s) modificado(s).");

                await AuditoriaRepository.InsertarAuditoriaUsuario(usuario.Id.Valor, "INFO", "USUARIO_MODIFICADO",
                    $"Se modificaron los datos del usuario con id '{usuario.Id.Valor}' y correo '{usuario.Correo.Valor}'.");

            }
            catch (Exception ex)
            {
                Logger.Error($"Error al modificar usuario ID: {usuario.Id.Valor} en MongoDB.", ex);
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region GetTodos()
        /// <summary>
        /// Permite recuperar la lista completa de todos los usuarios, mapeándolos de BSON a la entidad de Dominio.
        /// </summary>
        /// <returns>Retorna una lista de todas las entidades <see cref="Usuario"/>.</returns>
        public async Task<List<Usuario>> GetTodos()
        {
            Logger.Debug("Recuperando todos los documentos de usuario de MongoDB.");
            try
            {
                var bsonUsuarios = await UsuarioCollection.Find(_ => true).ToListAsync();

                if (bsonUsuarios == null || !bsonUsuarios.Any())
                {
                    Logger.Info("No se encontraron documentos de usuario en MongoDB.");
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
                    var telefono = new VOTelefono(bsonUsuario["telefono"].AsString);
                    var direccion = new VODireccion(bsonUsuario["direccion"].AsString);
                    var fotoPerfil = new VOFotoPerfil(bsonUsuario["fotoPerfil"].AsString);
                    var rol = new VORolKeycloakId(bsonUsuario["rol"].AsString);
                    var preferencias = new VOPreferencias(bsonUsuario.GetValue("preferencias", new BsonArray()).AsBsonArray.Select(b => b.AsString).ToList());


                    return UsuarioFactory.Load(
                        idUsuario, nombre, apellido, fechaNacimiento, correo, telefono, direccion, rol, fotoPerfil, preferencias
                    );
                }).ToList();

                Logger.Debug($"Recuperados {listaUsuarios.Count} usuarios y reconstruidos por la fábrica.");
                return listaUsuarios;
            }
            catch (Exception ex)
            {
                Logger.Error("Error al obtener todos los usuarios de MongoDB.", ex);
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

        #region ActualizarPreferencias(Usuario usuario)
        public async Task ActualizarPreferencias(Usuario usuario)
        {
            Logger.Info($"Actualizando preferencias del usuario {usuario.Id.Valor} en MongoDB.");
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", usuario.Id.Valor);

                var update = Builders<BsonDocument>.Update
                    .Set("preferencias", BsonArray.Create(usuario.Preferencias.Preferencias))
                    .Set("updatedAt", DateTime.UtcNow);

                var result = await UsuarioCollection.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                {
                    Logger.Warn($"No se modificaron las preferencias para el usuario {usuario.Id.Valor}.");
                    return;
                }

                await AuditoriaRepository.InsertarAuditoriaUsuario(usuario.Id.Valor, "INFO", "USUARIO_PREFERENCIAS_MODIFICADO",
                    $"Se modificaron las preferencias del usuario con id '{usuario.Id.Valor}' y correo '{usuario.Correo.Valor}'.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al actualizar preferencias para usuario {usuario.Id.Valor} en MongoDB.", ex);
                throw new UsuarioRepositoryException(ex);
            }
        }
        #endregion

    }
}
