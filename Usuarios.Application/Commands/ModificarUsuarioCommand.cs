using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;

namespace Usuarios.Application.Commands
{
    public class ModificarUsuarioCommand : IRequest<bool>
    {
        public ModificarUsuarioDTO UsuarioDto { get; }
        public string id { get; }

        public ModificarUsuarioCommand(string id, ModificarUsuarioDTO usuarioDto)
        {
            UsuarioDto = usuarioDto;
            this.id = id;
        }
    }
}
