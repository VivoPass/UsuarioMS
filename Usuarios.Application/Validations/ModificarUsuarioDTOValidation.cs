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

            RuleFor(x => x.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .MaximumLength(15).WithMessage("El teléfono no debe superar los 15 caracteres.");

            RuleFor(x => x.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(100).WithMessage("La dirección no debe superar los 100 caracteres.");

            RuleFor(x => x.FotoPerfil)
                .NotEmpty().WithMessage("La foto de perfil es obligatoria.");
        }
    }
}
