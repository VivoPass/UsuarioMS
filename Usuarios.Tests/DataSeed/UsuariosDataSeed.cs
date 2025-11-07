using Usuarios.Application.Commands;
using Usuarios.Application.DTOs;
using Usuarios.Domain.Aggregates;
using Usuarios.Domain.Entities;
using Usuarios.Domain.ValueObjects;

namespace Usuarios.Tests.DataSeed
{
    public class UsuariosDataSeed
    {
        /*
        // --- DATOS ---
        private readonly VORolId mockRolId;
        private readonly Rol rolExistente;
        private readonly string expectedId;
        private readonly CrearUsuarioCommand crearCommand;
        private readonly ModificarUsuarioCommand modificarCommand;
        private readonly Usuario factoryOutput;

        public UsuariosDataSeed()
        {
            // --- DATOS ---
            mockRolId = new VORolId(Guid.NewGuid().ToString());
            var mockRolNombre = new VORolNombre("Cliente_Simulado");
            rolExistente = new Rol(mockRolId, mockRolNombre);

            expectedId = Guid.NewGuid().ToString();

            crearCommand = new CrearUsuarioCommand(new CrearUsuarioDTO
            {
                Nombre = "Test",
                Apellido = "User",
                Correo = "valid@test.com",
                FechaNacimiento = new DateOnly(2002, 10, 14),
                Rol = Guid.NewGuid().ToString() // ID de Rol simulado
            });

            modificarCommand = new ModificarUsuarioCommand(expectedId, new ModificarUsuarioDTO
            {
                Nombre = "UpdatedTest",
                Apellido = "UpdatedUser"
            });

            factoryOutput = new Usuario(
                id: new VOId(expectedId),
                nombre: new VONombre(crearCommand.UsuarioDto.Nombre),
                apellido: new VOApellido(crearCommand.UsuarioDto.Apellido),
                fechaNacimiento: new VOFechaNacimiento(crearCommand.UsuarioDto.FechaNacimiento),
                correo: new VOCorreo(crearCommand.UsuarioDto.Correo),
                rol: new VORolId(new Guid(crearCommand.UsuarioDto.Rol).ToString())
            );
        }
        */
    }
}
