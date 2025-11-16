using FluentValidation;
using Usuarios.Application.DTOs;

namespace Usuarios.Application.Validations
{
    public class CrearUsuarioDTOValidation : AbstractValidator<CrearUsuarioDTO>
    {
        public CrearUsuarioDTOValidation()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no debe superar los 50 caracteres.");

            RuleFor(x => x.Apellido)
               .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(50).WithMessage("El apellido no debe superar los 50 caracteres.");

            RuleFor(x => x.FechaNacimiento)
                .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
                .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("La fecha de nacimiento debe ser una fecha pasada.");

            RuleFor(x => x.Correo)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("Debe ingresar un email válido.");

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .MaximumLength(15).WithMessage("El teléfono no debe superar los 15 caracteres.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(100).WithMessage("La dirección no debe superar los 100 caracteres.");

            RuleFor(x => x.Rol)
                .NotEmpty().WithMessage("El ID del rol es obligatorio.");
        }
    }
}
