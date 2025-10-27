using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Usuarios.Application.DTOs;

namespace Usuarios.Infrastructure.Queries
{
    public class GetRolByIdQuery : IRequest<RolDTO>
    {
        public string Id { get; set; }

        public GetRolByIdQuery(string id)
        {
            Id = id;
        }
    }
}
