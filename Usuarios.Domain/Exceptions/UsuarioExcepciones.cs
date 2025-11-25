using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios.Domain.Exceptions
{
    /*/////////////////PRESENTACION/////////////////*/
    /*public class ControllerException : Exception
    {
        public int StatusCode { get; }

        public ControllerException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class UsuarioNoEncontradoException : ControllerException
    {
        public UsuarioNoEncontradoException(string id)
            // Se define el StatusCode (404 Not Found) y el mensaje de error
            : base($"El usuario con ID '{id}' no fue encontrado.", 404)
        {
        }
    }*/

    /*/////////////////APLICACION/////////////////*/
    #region COMMANDS EXCEPTIONS
    //CrearUsuarioCommandHandler
    public class CrearUsuarioCommandHandlerException : Exception
    {
        public CrearUsuarioCommandHandlerException(Exception inner) : base("No fue posible agregar el usuario al dominio. " +
            "El comando no cumplió con las reglas de negocio definidas.", inner)
        { }
    }
    //ModificarUsuarioCommandHandler
    public class ModificarUsuarioCommandHandlerException : Exception
    {
        public ModificarUsuarioCommandHandlerException(Exception inner) : base("No fue posible modificar el usuario al dominio. " +
            "El comando no cumplió con las reglas de negocio definidas.", inner)
        { }
    }
    #endregion

    #region EVENTS EXCEPTIONS
    //HistorialActividadEventHandler
    public class HistorialActividadEventHandlerException : Exception
    {
        public HistorialActividadEventHandlerException(Exception inner)
            : base("El evento de Actividad del Usuario agregada no pudo ser aplicado al estado del dominio.", inner)
        { }
    }
    public class HistActUsuarioQueryHandlerInvalidIdException : Exception
    {
        public HistActUsuarioQueryHandlerInvalidIdException() : base("El ID de usuario no puede ser nulo o vacío.")
        { }
    }
    #endregion

    /*/////////////////DOMINIO/////////////////*/
    #region VALUE OBJECTS EXCEPTIONS
    public class ApellidoUsuarioException : Exception
    {
        public ApellidoUsuarioException() : base("El apellido no puede estar vacío.") { }
    }
    public class CorreoUsuarioException : Exception
    {
        public CorreoUsuarioException() : base("El correo es inválido.") { }
    }
    public class FechaNacimientoUsuarioException : Exception
    {
        public FechaNacimientoUsuarioException() : base("La fecha de nacimiento no puede ser futura.") { }
    }
    public class IDUsuarioNullException : Exception
    {
        public IDUsuarioNullException() : base("El ID de usuario no puede estar vacío.") { }
    }
    public class IDUsuarioException : Exception
    {
        public IDUsuarioException() : base("El ID de usuario debe ser un GUID válido.") { }
    }
    public class NombreUsuarioException : Exception
    {
        public NombreUsuarioException() : base("El nombre no puede estar vacío.") { }
    }
    public class IDRolNullException : Exception
    {
        public IDRolNullException() : base("El ID de rol no puede estar vacío.") { }
    }
    public class IDRolException : Exception
    {
        public IDRolException() : base("El ID de rol debe ser un GUID válido.") { }
    }
    public class NombreRolException : Exception
    {
        public NombreRolException() : base("El nombre del rol no puede estar vacío.") { }
    }
    public class TelefonoUsuarioException : Exception
    {
        public TelefonoUsuarioException() : base("El telefono no puede estar vacío.") { }
    }
    public class DireccionUsuarioException : Exception
    {
        public DireccionUsuarioException() : base("La direccion no puede estar vacía.") { }
    }
    public class FotoPerfilUsuarioException : Exception
    {
        public FotoPerfilUsuarioException() : base("La foto de perfil no puede estar vacía.") { }
    }
    #endregion

    #region FACTORIES EXCEPTIONS
    //USUARIO FACTORY NULL EXCEPTION
    public class UsuarioFactoryNullException : Exception
    {
        public UsuarioFactoryNullException() : base("El componente IUsuarioFactory no fue inicializado correctamente.")
        { }
    }
    //ROL FACTORY NULL EXCEPTION
    public class RolFactoryNullException : Exception
    {
        public RolFactoryNullException() : base("El componente IRolFactory no fue inicializado correctamente.")
        { }
    }
    #endregion

    /*/////////////////INFRAESTRUCTURA/////////////////*/
    #region MONGODBCONFIG EXCEPTIONS
    public class ConexionBdInvalida : Exception
    {
        public ConexionBdInvalida() : base("La cadena de conexión de MongoDB no está definida.") { }
    }
    public class NombreBdInvalido : Exception
    {
        public NombreBdInvalido() : base("El nombre de la base de datos de MongoDB no está definido.") { }
    }
    public class MongoDBConnectionException : Exception
    {
        public MongoDBConnectionException(Exception inner) : base("Error al conectar con la base de datos de MongoDB.", inner) { }
    }
    public class MongoDBUnnexpectedException : Exception
    {
        public MongoDBUnnexpectedException(Exception inner) : base("Error inesperado con la base de datos de MongoDB.", inner) { }
    }
    #endregion

    #region CONSUMERS EXCEPTIONS
    //HistActConsumer
    public class HistActConsumerException : Exception
    {
        public HistActConsumerException(Exception inner)
            : base("Ocurrió un error al consumir el mensaje HistorialActividad desde la cola de eventos.", inner)
        { }
    }
    #endregion

    #region REPOSITORIES EXCEPTIONS
    //Usuario Repository Exception
    public class UsuarioRepositoryException : Exception
    {
        public UsuarioRepositoryException(Exception inner) : base("Fallo en UsuarioRepository. " +
            "No se pudo completar la operación.", inner)
        { }
    }
    public class UsuarioRepositoryNullException : Exception
    {
        public UsuarioRepositoryNullException() : base("El componente IUsuarioRepository no fue inicializado correctamente.")
        { }
    }
    //Rol Repository Exception
    public class RolRepositoryException : Exception
    {
        public RolRepositoryException(Exception inner) : base("Fallo en RolRepository. " +
            "No se pudo completar la operación.", inner)
        { }
    }
    public class RolRepositoryNullException : Exception
    {
        public RolRepositoryNullException() : base("El componente IRolRepository no fue inicializado correctamente.")
        { }
    }
    //Hist Act Repository Exception
    public class HistActRepositoryException : Exception
    {
        public HistActRepositoryException(Exception inner) : base("Fallo en UsuarioHistorialActividadRepository. " +
            "No se pudo completar la operación.", inner)
        { }
    }
    public class HistActRepositoryNullException : Exception
    {
        public HistActRepositoryNullException() : base("El componente IUsuarioHistorialActividad no fue inicializado correctamente.")
        { }
    }
    //Auditoria Repository Exception
    public class AuditoriaRepositoryException : Exception
    {
        public AuditoriaRepositoryException(Exception inner) : base("Fallo en AuditoriaRepository. " +
                                                                    "No se pudo completar la operación.", inner)
        { }
    }
    public class AuditoriaRepositoryNullException : Exception
    {
        public AuditoriaRepositoryNullException() : base("El componente IAuditoriaRepository no fue inicializado correctamente.")
        { }
    }
    #endregion

    #region QUERIES EXCEPTIONS
    //GetRolByIdQueryHandler
    public class GetRolByIdQueryHandlerException : Exception
    {
        public GetRolByIdQueryHandlerException(Exception inner)
            : base("Hubo un error al obtener el Rol por ID del repositorio.", inner)
        { }
    }
    //GetRolByNombreQueryHandler
    public class GetRolByNombreQueryHandlerException : Exception
    {
        public GetRolByNombreQueryHandlerException(Exception inner)
            : base("Hubo un error al obtener el Rol por Nombre del repositorio.", inner)
        { }
    }
    //GetTodosRolesQueryHandler
    public class GetTodosRolesQueryHandlerException : Exception
    {
        public GetTodosRolesQueryHandlerException(Exception inner)
            : base("Hubo un error al obtener todos los Roles del repositorio.", inner)
        { }
    }
    //GetTodosUsuariosQueryHandler
    public class GetTodosUsuariosQueryHandlerException : Exception
    {
        public GetTodosUsuariosQueryHandlerException(Exception inner)
            : base("Hubo un error al obtener todos los Usuarios del repositorio.", inner)
        { }
    }
    //GetUsuarioByCorreoQueryHandler
    public class GetUsuarioByCorreoQueryHandlerException : Exception
    {
        public GetUsuarioByCorreoQueryHandlerException(Exception inner)
            : base("Hubo un error al obtener el Usuario por Correo del repositorio.", inner)
        { }
    }
    //GetUsuarioByIdQueryHandler
    public class GetUsuarioByIdQueryHandlerException : Exception
    {
        public GetUsuarioByIdQueryHandlerException(Exception inner)
            : base("Hubo un error al obtener el Usuario por ID del repositorio.", inner)
        { }
    }
    //HistActUsuarioQueryHandler
    public class HistActUsuarioQueryHandlerException : Exception
    {
        public HistActUsuarioQueryHandlerException(Exception inner)
            : base("Hubo un error al obtener el Historial de Actividad de un Usuario del repositorio.", inner)
        { }
    }
    #endregion

    #region SERVICES EXCEPTIONS
    //Cloudinary Service Exception
    public class SubirImagenCloudinaryException : Exception
    {
        public SubirImagenCloudinaryException(Exception inner) : base("Error al subir la imagen a Cloudinary.", inner) { }
    }
    #endregion

    /*/////////////////OTROS/////////////////*/
    #region MEDIATR EXCEPTIONS
    //Mediatr Null Exception
    public class MediatorNullException : Exception
    {
        public MediatorNullException() : base("El componente IMediator no fue inicializado correctamente.")
        { }
    }
    #endregion

    #region SENDENDPOINTPROVIDER EXCEPTIONS
    //SendEndpointProvider Null Exception
    public class SendEndpointProviderNullException : Exception
    {
        public SendEndpointProviderNullException() : base("No se pudo acceder al publicador de eventos (ISendEndpointProvider).")
        { }
    }
    #endregion

    #region PUBLISHENDPOINT EXCEPTIONS
    //PublishEndpoint Null Exception
    public class PublishEndpointNullException : Exception
    {
        public PublishEndpointNullException() : base("No se pudo acceder al publicador de eventos (IPublishEndpoint). " +
            "El servicio de mensajería no está disponible o no fue configurado.")
        { }
    }
    #endregion

    #region CLOUDINARY EXCEPTIONS
    public class CloudinaryServiceNullException : Exception
    {
        public CloudinaryServiceNullException() : base("No se pudo acceder al proveedor de imágenes Cloudinary. " +
                                                "El servicio no está inicializado o no fue configurado correctamente.")
        { }
    }
    #endregion

    #region LOGGER EXCEPTIONS
    public class LoggerNullException : Exception
    {
        public LoggerNullException() : base("El servicio de logging (ILogger o similar) es obligatorio y no puede ser nulo. " + 
                                            "Asegúrese de que ILogger esté correctamente inyectado en el constructor del componente.")
        { }
    }
    #endregion

    #region VALIDATION EXCEPTIONS
        //Rol Exceptions
        public class IDRolNotFoundException : Exception
    {
        public IDRolNotFoundException() : base("El ID del rol especificado no existe.")
        { }
    }
    public class NombreRolNotFoundException : Exception
    {
        public NombreRolNotFoundException() : base("El nombre del rol especificado no existe.")
        { }
    }
    //Usuario Exceptions
    public class IDUsuarioNotFoundException : Exception
    {
        public IDUsuarioNotFoundException() : base("El usuario especificado no existe.")
        { }
    }
    public class CorreoUsuarioNotFoundException : Exception
    {
        public CorreoUsuarioNotFoundException() : base("El usuario especificado no existe.")
        { }
    }
    public class CorreoUsuarioExistsException : Exception
    {
        public CorreoUsuarioExistsException() : base("El correo electrónico ya está registrado.")
        { }
    }
    #endregion

}
