using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Entities
{
    public class Rol
    {
        public VORolId IdRol { get; private set; }
        public VORolNombre NombreRol { get; private set; }
        public VORolKeycloakId RolKeycloakId { get; private set; }

        public Rol(VORolId idRol, VORolNombre nombreRol, VORolKeycloakId rolKeycloakId)
        {
            IdRol = idRol;
            NombreRol = nombreRol;
            RolKeycloakId = rolKeycloakId;
        }
    }
}
