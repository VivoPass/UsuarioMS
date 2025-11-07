using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Factories
{
    public class UsuarioFactory: IUsuarioFactory
    {
        public Usuario Crear (string nombre, string apellido, DateOnly fechaNacimiento, string correo, string telefono, string direccion, string fotoPerfil, string rol)
        {
            VOId id = new VOId(Guid.NewGuid().ToString());
            VONombre Nombre = new VONombre(nombre);
            VOApellido Apellido = new VOApellido(apellido);
            VOFechaNacimiento FechaNacimiento = new VOFechaNacimiento(fechaNacimiento);
            VOCorreo Correo = new VOCorreo(correo);
            VOTelefono Telefono = new VOTelefono(telefono);
            VODireccion Direccion = new VODireccion(direccion);
            VOFotoPerfil FotoPerfil = new VOFotoPerfil(fotoPerfil);
            VORolId Rol = new VORolId(rol);

            var NuevoUsuario = new Usuario(id, Nombre, Apellido, FechaNacimiento, Correo, Telefono, Direccion, FotoPerfil, Rol);

            return NuevoUsuario;
        }

        public Usuario Load(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VOTelefono telefono,
            VODireccion direccion, VOFotoPerfil fotoPerfil, VORolId rol)
        {
            return new Usuario(id, nombre, apellido, fechaNacimiento, correo, telefono, direccion, fotoPerfil, rol);
        }
    }
}
