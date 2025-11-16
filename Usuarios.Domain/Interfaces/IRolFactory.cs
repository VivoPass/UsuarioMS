using Usuarios.Domain.Entities;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Interfaces
{
    public interface IRolFactory
    {
        Rol Load(VORolId id, VORolNombre nombre, VORolKeycloakId rolKeycloakId);
    }
}
