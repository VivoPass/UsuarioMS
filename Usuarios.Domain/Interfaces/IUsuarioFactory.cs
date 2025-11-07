using Usuarios.Domain.Aggregates;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Interfaces
{
    public interface IUsuarioFactory
    {
        Usuario Crear (string nombre, string apellido, DateOnly fechaNacimiento, string correo, string telefono, string direccion, string fotoPerfil, string rol);
        Usuario Load(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VOTelefono telefono,
            VODireccion direccion, VOFotoPerfil fotoPerfil, VORolId rol);
    }
}
