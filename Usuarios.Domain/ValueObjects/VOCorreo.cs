using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VOCorreo
    {
        public string Valor { get; private set; }

        public VOCorreo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor) || !valor.Contains("@"))
                throw new CorreoUsuarioException();

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
