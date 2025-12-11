using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VOFechaNacimiento
    {
        public DateOnly Valor { get; private set; }

        public VOFechaNacimiento(DateOnly valor)
        {
            if (valor > DateOnly.FromDateTime(DateTime.Today))
                throw new FechaNacimientoUsuarioException();

            if (valor > DateOnly.FromDateTime(DateTime.Today).AddYears(-18))
                throw new FechaNacimientoUsuarioMenorException();

            Valor = valor;
        }
    }
}
