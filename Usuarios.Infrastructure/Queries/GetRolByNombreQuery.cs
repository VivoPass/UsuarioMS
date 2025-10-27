using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;

namespace Usuarios.Infrastructure.Queries
{
    public class GetRolByNombreQuery : IRequest<RolDTO>
    {
        public string Nombre { get; set; }

        public GetRolByNombreQuery(string nombre)
        {
            Nombre = nombre;
        }
    }
}
