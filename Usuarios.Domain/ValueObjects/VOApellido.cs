using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VOApellido
    {
        public string Valor { get; private set; }

        public VOApellido(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ApellidoUsuarioException();

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
