using Usuarios.Domain.Entities;
using Usuarios.Domain.Interfaces;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Factories
{
    public class RolFactory: IRolFactory
    {
        public Rol Load(VORolId id, VORolNombre nombre)
        {
            return new Rol(id, nombre);
        }
    }
}
