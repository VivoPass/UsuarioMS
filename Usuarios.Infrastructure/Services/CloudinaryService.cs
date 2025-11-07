using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using log4net;
using Microsoft.Extensions.Configuration;
using Usuarios.Infrastructure.Interfaces;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Infrastructure.Services
{
    /// <summary>
    /// Proporciona un mecanismo para subir archivos multimedia de forma externa.
    /// </summary>
    /// <remarks>
    /// Este servicio implementa el patrón External Service Access (acceso a servicio externo) 
    /// para desacoplar la aplicación de la lógica específica de almacenamiento en la nube (Cloudinary), 
    /// permitiendo el escalado de archivos multimedia sin sobrecargar el servidor principal y la base de datos.
    /// </remarks>
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary Cloudinary;
        private readonly ILog Logger;

        public CloudinaryService(IConfiguration configuration, ILog logger)
        {
            var account = new Account(
                Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY"),
                Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
            );

            Cloudinary = new Cloudinary(account);
            Logger = logger ?? throw new LoggerNullException();
        }

        #region SubirImagen(Stream archivoStream, string nombreArchivo)
        /// <summary>
        /// Permite subir un archivo de imagen al servicio de Cloudinary y obtener la URL segura.
        /// </summary>
        /// <param name="archivoStream">El flujo de datos (Stream) del archivo a subir.</param>
        /// <param name="nombreArchivo">El nombre base del archivo para usar en la subida.</param>
        /// <returns>Retorna la URL pública y segura de la imagen subida.</returns>
        public async Task<string> SubirImagen(Stream archivoStream, string nombreArchivo)
        {
            Logger.Info($"Iniciando subida de archivo: {nombreArchivo} a la carpeta 'usuarios'.");
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(nombreArchivo, archivoStream),
                    Folder = "usuarios"
                };

                var resultado = await Cloudinary.UploadAsync(uploadParams);

                if (resultado.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Logger.Warn($"Subida a Cloudinary falló. Status: {resultado.StatusCode}. Error: {resultado.Error?.Message}");
                    throw new SubirImagenCloudinaryException(new InvalidOperationException
                        ($"Cloudinary no devolvió 200 OK. Error: {resultado.Error?.Message}"));
                }

                Logger.Info($"Imagen subida exitosamente. URL: {resultado.SecureUrl}");
                return resultado.SecureUrl.ToString();
            }
            catch (SubirImagenCloudinaryException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error crítico al subir imagen {nombreArchivo} a Cloudinary.", ex);
                throw new SubirImagenCloudinaryException(ex); ;
            }
        }
        #endregion
    }
}
