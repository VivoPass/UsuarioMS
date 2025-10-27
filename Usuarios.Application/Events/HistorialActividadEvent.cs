using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usuarios.Application.Events
{
    public class HistorialActividadEvent : INotification
    {
        public string UsuarioId { get; set; }
        public string Accion { get; set; } //Ejemplo: "USER_UPDATED", "LOGIN", "PASSWORD_CHANGED"
        public DateTime Timestamp { get; set; }

        public HistorialActividadEvent(string usuarioId, string accion, DateTime timestamp)
        {
            UsuarioId = usuarioId;
            Accion = accion;
            Timestamp = timestamp;
        }
    }
}
