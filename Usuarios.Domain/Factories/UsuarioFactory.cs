using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Factories
{
    public class UsuarioFactory: IUsuarioFactory
    {
        public Usuario Crear (string nombre, string apellido, DateOnly fechaNacimiento, string correo, string rol)
        {
            VOId id = new VOId(Guid.NewGuid().ToString());
            VONombre Nombre = new VONombre(nombre);
            VOApellido Apellido = new VOApellido(apellido);
            VOFechaNacimiento FechaNacimiento = new VOFechaNacimiento(fechaNacimiento);
            VOCorreo Correo = new VOCorreo(correo);
            VORolId Rol = new VORolId(rol);

            var NuevoUsuario = new Usuario(id, Nombre, Apellido, FechaNacimiento, Correo, Rol);

            return NuevoUsuario;
        }

        public Usuario Load(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VORolId rol)
        {
            return new Usuario(id, nombre, apellido, fechaNacimiento, correo, rol);
        }
    }
}
