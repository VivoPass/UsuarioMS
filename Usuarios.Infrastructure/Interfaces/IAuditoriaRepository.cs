using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios.Infrastructure.Interfaces
{
    public interface IAuditoriaRepository
    {
        Task InsertarAuditoriaUsuario(string idUsuario, string level, string tipo, string mensaje);
        Task InsertarAuditoriaHistAct(string idUsuario, string level, string tipo, string mensaje);
    }
}
