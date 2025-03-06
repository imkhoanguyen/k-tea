using FluentValidation;
using Tea.Application.DTOs.Categories;

namespace Tea.Application.Validators.Categories
{
    public class CategoryCreateChildrenRequestValidator : AbstractValidator<CategoryCreateChildrenRequest>
    {
        public CategoryCreateChildrenRequestValidator()
        {
            RuleFor(x => x.ParentId)
                .GreaterThan(0)
                .WithMessage("ParentId must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 10000 characters.");

            RuleFor(x => x.Slug)
                .NotEmpty()
                .WithMessage("Slug is required.")
                .MaximumLength(100)
                .WithMessage("Slug must not exceed 100 characters.");
        }
    }
}
