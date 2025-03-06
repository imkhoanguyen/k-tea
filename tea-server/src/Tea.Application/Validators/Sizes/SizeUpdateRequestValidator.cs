using FluentValidation;
using Tea.Application.DTOs.Sizes;

namespace Tea.Application.Validators.Sizes
{
    public class SizeUpdateRequestValidator : AbstractValidator<SizeUpdateRequest>
    {
        public SizeUpdateRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("SizeId must be greather than 0.");

            RuleFor(x => x.ItemId)
                .GreaterThan(0)
                .WithMessage("ItemId must be greather than 0.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0.");

            RuleFor(x => x.NewPrice)
            .Must((size, newPrice) => !newPrice.HasValue || newPrice < size.Price)
            .WithMessage("NewPrice must be less than Price if provided.");
        }
    }
}
