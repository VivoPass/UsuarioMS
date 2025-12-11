using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios.Domain.ValueObjects
{
    public class VOPreferencias
    {
        public List<string> Preferencias { get; private set; }

        public VOPreferencias(List<string> preferencias)
        {
            Preferencias = preferencias ?? new List<string>();
        }

        public bool TienePreferencia(string preferencia) => Preferencias.Contains(preferencia);
    }
}