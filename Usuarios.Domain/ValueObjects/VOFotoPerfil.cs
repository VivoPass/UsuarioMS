using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VOFotoPerfil
    {
        public string Valor { get; private set; }

        public VOFotoPerfil(string valor)
        {
            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
