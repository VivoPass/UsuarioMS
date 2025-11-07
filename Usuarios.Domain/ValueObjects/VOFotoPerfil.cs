using Usuarios.Domain.Exceptions;

namespace Usuarios.Domain.ValueObjects
{
    public class VOFotoPerfil
    {
        public string Valor { get; private set; }

        public VOFotoPerfil(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new FotoPerfilUsuarioException();

            Valor = valor;
        }

        public override string ToString() => Valor;
    }
}
