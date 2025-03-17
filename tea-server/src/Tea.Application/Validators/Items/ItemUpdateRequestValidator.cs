using FluentValidation;
using Tea.Application.DTOs.Items;

namespace Tea.Application.Validators.Items
{
    public class ItemUpdateRequestValidator : AbstractValidator<ItemUpdateRequest>
    {
        public ItemUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Slug)
                .NotEmpty()
                .WithMessage("Slug is required.")
                .MaximumLength(100)
                .WithMessage("Slug must not exceed 100 characters.");

        }
    }
}
