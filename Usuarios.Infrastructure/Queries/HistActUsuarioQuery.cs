using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;

namespace Usuarios.Infrastructure.Queries
{
    public class HistActUsuarioQuery : IRequest<IEnumerable<HistActUsuarioDTO>>
    {
        public string? IdUsuario { get; set; }
        public DateTime Timestamp { get; set; }

        public HistActUsuarioQuery(string idUsuario, DateTime timestamp)
        {
            IdUsuario = idUsuario;
            Timestamp = timestamp;
        }
    }
}
