using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VORolNombre
    {
        public string Valor { get; private set; }

        public VORolNombre(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new NombreRolException();

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
