using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VORolKeycloakId
    {
        public string Valor { get; private set; }

        public VORolKeycloakId(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new IDRolNullException();

            /*if (valor != "administrador" || valor != "usuario_final" || valor != "soporte_tecnico" || valor != "organizador")
                throw new RolKeycloakInvalido();*/

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
