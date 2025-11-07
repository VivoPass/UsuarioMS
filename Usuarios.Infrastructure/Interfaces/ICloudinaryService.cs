
namespace Usuarios.Infrastructure.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> SubirImagen(Stream archivoStream, string nombreArchivo);
    }
}
