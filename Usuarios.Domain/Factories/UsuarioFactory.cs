using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Factories
{
    public class UsuarioFactory: IUsuarioFactory
    {
        public Usuario Crear (string id, string nombre, string apellido, DateOnly fechaNacimiento, string correo, string telefono, string direccion, string fotoPerfil, 
            string rol, List<string> preferencias)
        {
            VOId Id = new VOId(id);
            VONombre Nombre = new VONombre(nombre);
            VOApellido Apellido = new VOApellido(apellido);
            VOFechaNacimiento FechaNacimiento = new VOFechaNacimiento(fechaNacimiento);
            VOCorreo Correo = new VOCorreo(correo);
            VOTelefono Telefono = new VOTelefono(telefono);
            VODireccion Direccion = new VODireccion(direccion);
            VORolKeycloakId Rol = new VORolKeycloakId(rol);
            VOFotoPerfil FotoPerfil = new VOFotoPerfil(fotoPerfil);
            VOPreferencias Preferencias = new VOPreferencias(preferencias);

            var NuevoUsuario = new Usuario(Id, Nombre, Apellido, FechaNacimiento, Correo, Telefono, Direccion, Rol, FotoPerfil, Preferencias);

            return NuevoUsuario;
        }

        public Usuario Load(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VOTelefono telefono,
            VODireccion direccion, VORolKeycloakId rol, VOFotoPerfil fotoPerfil, VOPreferencias preferencias)
        {
            return new Usuario(id, nombre, apellido, fechaNacimiento, correo, telefono, direccion, rol, fotoPerfil, preferencias);
        }
    }
}
