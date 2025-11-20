using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using Usuarios.Domain.Exceptions;
using Usuarios.Infrastructure.Configurations;
using Usuarios.Infrastructure.Interfaces;

namespace Usuarios.Infrastructure.Persistences.Repositories.MongoDB
{
    /// <summary>
    /// Proporciona la capa de persistencia para registrar y consultar las acciones de los usuarios.
    /// </summary>
    /// <remarks>
    /// Esta clase se utiliza para el registro de actividades que realiza el usuario final. 
    /// Utilizar un repositorio separado garantiza que el historial de actividad sea gestionado de manera eficiente 
    /// y que los eventos se almacenen secuencialmente en un formato optimizado para consulta (MongoDB).
    /// </remarks>
    public class UsuarioHistorialActividadRepository : IUsuarioHistorialActividad
    {
        private readonly IMongoCollection<BsonDocument> UsuarioHistActCollection;
        private readonly ILog Logger;

        public UsuarioHistorialActividadRepository(MongoDBConfig mongoConfig, ILog logger)
        {
            UsuarioHistActCollection = mongoConfig.db.GetCollection<BsonDocument>("historial_act_usuarios");
            Logger = logger ?? throw new LoggerNullException();
        }

        #region AgregarHistAct(BsonDocument actUsuario)
        /// <summary>
        /// Permite agregar un nuevo registro de actividad de usuario en el historial.
        /// </summary>
        /// <param name="actUsuario">El documento BSON con los detalles de la actividad a registrar.</param>
        /// <returns>Tarea completada.</returns>
        public async Task AgregarHistAct(BsonDocument actUsuario)
        {
            string userId = actUsuario.Contains("idUsuario") ? actUsuario["idUsuario"].AsString : "Desconocido";
            string accion = actUsuario.Contains("accion") ? actUsuario["accion"].AsString : "Desconocida";

            Logger.Info($"Iniciando registro de actividad para Usuario: {userId}. Acción: {accion}.");
            try
            {
                await UsuarioHistActCollection.InsertOneAsync(actUsuario);
                Logger.Debug($"Actividad registrada con éxito para Usuario: {userId}.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al agregar actividad al historial para Usuario {userId}.", ex);
                throw new HistActRepositoryException(ex);
            }
        }
        #endregion

        #region GetTodosHistAct()
        /// <summary>
        /// Permite obtener el registro completo de todas las actividades de todos los usuarios.
        /// </summary>
        /// <returns>Retorna una lista de <see cref="BsonDocument"/> con el historial completo.</returns>
        public async Task<List<BsonDocument>> GetTodosHistAct()
        {
            Logger.Debug("Recuperando todo el historial de actividad de todos los usuarios.");
            try
            {
                var bsonUserActivity = await UsuarioHistActCollection.Find(new BsonDocument()).ToListAsync();

                if (bsonUserActivity == null || !bsonUserActivity.Any())
                {
                    Logger.Info("No se encontraron registros de historial de actividad.");
                    return new List<BsonDocument>();
                }

                Logger.Debug($"Recuperados {bsonUserActivity.Count} registros de actividad.");
                return bsonUserActivity;
            }
            catch (Exception ex)
            {
                Logger.Error("Error al obtener todo el historial de actividad.", ex);
                throw new HistActRepositoryException(ex);
            }
        }
        #endregion

        #region GetByIdUsuarioHistAct(string idUsuario, DateTime timestamp)
        /// <summary>
        /// Permite obtener el historial de actividad de un usuario específico a partir de un punto en el tiempo.
        /// </summary>
        /// <param name="idUsuario">El ID del usuario cuyo historial se desea consultar.</param>
        /// <param name="timestamp">El punto de inicio en el tiempo para el filtro (solo actividades posteriores).</param>
        /// <returns>Retorna una lista de <see cref="BsonDocument"/> que cumplen con el filtro.</returns>
        public async Task<List<BsonDocument>> GetByIdUsuarioHistAct(string idUsuario, DateTime timestamp)
        {
            Logger.Debug($"Buscando historial de actividad para Usuario ID: {idUsuario} desde {timestamp:yyyy-MM-dd HH:mm:ss}.");
            try
            {
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("_idUsuario", idUsuario)
                    //, Builders<BsonDocument>.Filter.Gte("timestamp", timestamp)
                );

                var bsonUserActivity = await UsuarioHistActCollection.Find(filter).ToListAsync();

                if (bsonUserActivity == null || !bsonUserActivity.Any())
                {
                    Logger.Info($"No se encontraron registros de actividad para Usuario ID: {idUsuario} después de la marca de tiempo.");
                    return new List<BsonDocument>();
                }

                Logger.Debug($"Recuperados {bsonUserActivity.Count} registros de actividad para Usuario ID: {idUsuario}.");
                return bsonUserActivity;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al obtener historial de actividad para Usuario ID: {idUsuario}.", ex);
                throw new HistActRepositoryException(ex);
            }
        }
        #endregion

    }
}
