using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VONombre
    {
        public string Valor { get; private set; }

        public VONombre(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new NombreUsuarioException();

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
