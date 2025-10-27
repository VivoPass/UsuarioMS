using FluentValidation;
using Usuarios.Application.DTOs;

namespace Usuarios.Application.Validations
{
    public class ModificarUsuarioDTOValidation : AbstractValidator<ModificarUsuarioDTO>
    {
        public ModificarUsuarioDTOValidation()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no debe superar los 50 caracteres.");

            RuleFor(x => x.Apellido)
                    .NotEmpty().WithMessage("El apellido es obligatorio.")
                    .MaximumLength(50).WithMessage("El apellido no debe superar los 50 caracteres.");
        }
    }
}
