using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios.Application.Commands
{
    public class ModificarPreferenciasUsuarioCommand : IRequest<bool>
    {
        public string UsuarioId { get; }
        public List<string> Preferencias { get; }

        public ModificarPreferenciasUsuarioCommand(string usuarioId, List<string> preferencias)
        {
            UsuarioId = usuarioId;
            Preferencias = preferencias;
        }
    }
}
