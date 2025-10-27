using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Interfaces
{
    public interface IUsuarioFactory
    {
        Usuario Crear (string nombre, string apellido, DateOnly fechaNacimiento, string correo, string rol);
        Usuario Load(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VORolId rol);
    }
}
