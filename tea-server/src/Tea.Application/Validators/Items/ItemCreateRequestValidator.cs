using System.Text.Json;
using FluentValidation;
using Tea.Application.DTOs.Items;
using Tea.Application.DTOs.Sizes;

namespace Tea.Application.Validators.Items
{
    public class ItemCreateRequestValidator : AbstractValidator<ItemCreateRequest>
    {
        public ItemCreateRequestValidator()
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

            RuleFor(x => x.SizeCreateRequestJson)
                .Must(ValidateJson)
                .WithMessage("SizeCreateRequestJson must be a valid JSON.")
                .Must(SizeCreateRequestJsonValidate)
                .WithMessage("Invalid size data in SizeCreateRequestJson.");

            RuleFor(x => x.CategoryIdList)
                .NotEmpty()
                .WithMessage("Category is required.");
        }

        #region Helper
        private bool ValidateJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool SizeCreateRequestJsonValidate(string json)
        {
            try
            {
                var sizes = JsonSerializer.Deserialize<List<SizeCreateRequest>>(json);
                if (sizes == null || sizes.Count == 0) return false;
                
                foreach(var size in sizes)
                {
                    if (string.IsNullOrEmpty(size.Name) || size.Name.Length > 100) return false;

                    if (size.Price <= 0) return false;

                    if (size.NewPrice.HasValue && size.NewPrice >= size.Price) return false;
                }
                return true;
            } 
            catch { return false; }
        }

        #endregion

    }
}
