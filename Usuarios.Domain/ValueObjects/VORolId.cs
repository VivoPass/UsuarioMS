
using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VORolId
    {
        public string Valor { get; private set; }

        public VORolId(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new IDRolNullException();

            if (!Guid.TryParse(valor, out _))
                throw new IDRolException();

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
