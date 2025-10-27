using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VOId
    {
        public string Valor { get; private set; }

        public VOId(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new IDUsuarioNullException();

            if (!Guid.TryParse(valor, out _))
                throw new IDUsuarioException();

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
