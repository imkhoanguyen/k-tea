using FluentValidation;
using Tea.Application.DTOs.Roles;

namespace Tea.Application.Validators.Roles
{
    public class RoleCreateRequestValidator : AbstractValidator<RoleCreateRequest>
    {
        public RoleCreateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Vui lòng điền tên quyền.")
                .MaximumLength(100)
                .WithMessage("Tên quyền có độ dài tối đa là 100 ký tự.");
        }
    }
}
