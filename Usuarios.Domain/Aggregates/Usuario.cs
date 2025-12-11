using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Aggregates
{
    public class Usuario
    {
        public VOId Id { get; private set; }
        public VONombre Nombre { get; private set; }
        public VOApellido Apellido { get; private set; }
        public VOFechaNacimiento FechaNacimiento { get; private set; }
        public VOCorreo Correo { get; private set; }
        public VOTelefono Telefono { get; private set; }
        public VODireccion Direccion { get; private set; }
        public VOFotoPerfil? FotoPerfil { get; private set; }
        public VORolKeycloakId Rol { get; private set; }
        public VOPreferencias? Preferencias { get; private set; }

        public Usuario(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VOTelefono telefono,
            VODireccion direccion, VORolKeycloakId rol, VOFotoPerfil? fotoPerfil = null, VOPreferencias? preferencias = null)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            FechaNacimiento = fechaNacimiento;
            Correo = correo;
            Telefono = telefono;
            Direccion = direccion;
            FotoPerfil = fotoPerfil;
            Rol = rol;
            Preferencias = preferencias;
        }

        public void Modificar(string? nombre, string? apellido, string? telefono, string? direccion, string? fotoPerfil)
        {
            if (nombre != null)
            {
                this.Nombre = new VONombre(nombre);
            }

            if (apellido != null)
            {
                this.Apellido = new VOApellido(apellido);
            }

            if (telefono != null)
            {
                this.Telefono = new VOTelefono(telefono);
            }

            if (direccion != null)
            {
                this.Direccion = new VODireccion(direccion);
            }

            if (fotoPerfil != null)
            {
                this.FotoPerfil = new VOFotoPerfil(fotoPerfil);
            }
        }

        public void SetPreferencias(List<string> nuevasPreferencias)
        {
            Preferencias = new VOPreferencias(nuevasPreferencias);
        }
    }
}
