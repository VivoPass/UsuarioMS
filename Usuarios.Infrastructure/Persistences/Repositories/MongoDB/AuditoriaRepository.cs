using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    /// <summary>
    /// Repositorio encargado de la gestión de la colección de Auditorías en MongoDB.
    /// </summary>
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly IMongoCollection<BsonDocument> AuditoriaColexion;
        private readonly ILog Log;
        public AuditoriaRepository(AuditoriaDbConfig mongoConfig, ILog log)
        {
            AuditoriaColexion = mongoConfig.db.GetCollection<BsonDocument>("auditoriaUsuarios");
            Log = log ?? throw new LoggerNullException();
        }

        #region InsertarAuditoriaUsuario(string idUsuario, string level, string tipo, string mensaje)
        /// <summary>
        /// Inserta un registro de auditoría en la colección de Auditoría.
        /// Utilizado para eventos relacionados con la gestión de usuarios (Login, Register, etc.).
        /// </summary>
        /// <param name="idUsuario">ID del usuario actor o afectado.</param>
        /// <param name="level">Nivel del log (INFO, WARN, ERROR).</param>
        /// <param name="tipo">Clasificación del evento.</param>
        /// <param name="mensaje">Mensaje descriptivo del evento.</param>
        public async Task InsertarAuditoriaUsuario(string idUsuario, string level, string tipo, string mensaje)
        {
            try
            {
                var documento = new BsonDocument
                {
                    { "_id",  Guid.NewGuid().ToString()},
                    { "idUsuario", idUsuario},
                    { "level", level},
                    { "tipo", tipo},
                    { "mensaje", mensaje},
                    { "timestamp", DateTime.Now}
                };
                await AuditoriaColexion.InsertOneAsync(documento);
                Log.Debug($"Auditoría de Usuario insertada: Tipo='{tipo}', ID='{idUsuario}'.");
            }
            catch (MongoException ex)
            {
                Log.Error($"[FATAL DB ERROR] Fallo al insertar auditoría de usuario (ID: {idUsuario}). Detalles: {ex.Message}", ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Fatal($"[FATAL ERROR] Excepción no controlada al insertar auditoría de usuario (ID: {idUsuario}).", ex);
                throw new AuditoriaRepositoryException(ex);
            }
        }
        #endregion

        #region InsertarAuditoriaHistAct(string idUsuario, string level, string tipo, string mensaje)
        /// <summary>
        /// Inserta un registro de auditoría en la colección de Auditoría.
        /// Utilizado para el historial de actividad (Historial de Compras, Interacciones, etc.).
        /// </summary>
        /// <param name="idUsuario">ID del usuario actor o afectado.</param>
        /// <param name="level">Nivel del log (INFO, WARN, ERROR).</param>
        /// <param name="tipo">Clasificación del evento.</param>
        /// <param name="mensaje">Mensaje descriptivo del evento.</param>
        public async Task InsertarAuditoriaHistAct(string idUsuario, string level, string tipo, string mensaje)
        {
            try
            {
                var documento = new BsonDocument
                {
                    { "_id",  Guid.NewGuid().ToString()},
                    { "idUsuario", idUsuario},
                    { "level", level},
                    { "tipo", tipo},
                    { "mensaje", mensaje},
                    { "timestamp", DateTime.Now}
                };
                await AuditoriaColexion.InsertOneAsync(documento);
                Log.Debug($"Auditoría de Historial de Actividad insertada: Tipo='{tipo}', ID='{idUsuario}'.");
            }
            catch (MongoException ex)
            {
                Log.Error($"[FATAL DB ERROR] Fallo al insertar historial de actividad (ID: {idUsuario}). Detalles: {ex.Message}", ex);
                throw;
            }
            catch (Exception ex)
            {
                Log.Fatal($"[FATAL ERROR] Excepción no controlada al insertar historial de actividad (ID: {idUsuario}).", ex);
                throw new AuditoriaRepositoryException(ex);
            }
        }
        #endregion
    }
}
