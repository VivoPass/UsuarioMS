using Usuarios.Domain.Aggregates;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Interfaces
{
    public interface IUsuarioFactory
    {
        Usuario Crear (string id, string nombre, string apellido, DateOnly fechaNacimiento, string correo, string telefono, string direccion, string rol, string fotoPerfil);
        Usuario Load(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VOTelefono telefono,
            VODireccion direccion, VORolKeycloakId rol, VOFotoPerfil fotoPerfil);
    }
}
