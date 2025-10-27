using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Domain.Aggregates
{
    public class Usuario
    {
        public VOId Id { get; private set; }
        public VONombre Nombre { get; private set; }
        public VOApellido Apellido { get; private set; }
        public VOFechaNacimiento FechaNacimiento { get; private set; }
        public VOCorreo Correo { get; private set; }
        public VORolId Rol { get; private set; }

        public Usuario(VOId id, VONombre nombre, VOApellido apellido, VOFechaNacimiento fechaNacimiento, VOCorreo correo, VORolId rol)
        {
            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            FechaNacimiento = fechaNacimiento;
            Correo = correo;
            Rol = rol;
        }

        public void Modificar(string? nombre, string? apellido)
        {
            if (nombre != null)
            {
                this.Nombre = new VONombre(nombre);
            }

            if (apellido != null)
            {
                this.Apellido = new VOApellido(apellido);
            }
        }
    }
}
