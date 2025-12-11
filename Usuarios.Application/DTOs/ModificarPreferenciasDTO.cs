using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios.Application.DTOs
{
    public class ModificarPreferenciasDTO
    {
        public List<string> Preferencias { get; set; } = new List<string>();
    }
}
