using ClothingStoreManagement.Application.DTO;
using FluentValidation;

namespace ClothingStoreManagement.Application.Validation
{
    public class CreateProductValidator : AbstractValidator<CreateProductWithVariantsDto>
    {
        public CreateProductValidator()
        {
            // قواعد فحص اسم المنتج
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم المنتج مطلوب")
                .Length(2, 100).WithMessage("اسم المنتج يجب أن يكون بين 2 و 100 حرف");

            // قواعد فحص الكود (SKU)
            RuleFor(x => x.SKU)
    .NotEmpty()
    .WithMessage("كود المنتج (SKU) مطلوب")
    .Length(1, 3)
    .WithMessage("كود المنتج يجب أن يكون بين 1 و 3 حروف أو أرقام")
    .Matches("^[A-Za-z0-9]+$")
    .WithMessage("كود المنتج يجب أن يحتوي على حروف وأرقام فقط");
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("يجب اختيار القسم بشكل صحيح");

            RuleFor(x => x.Variants)
                .NotEmpty().WithMessage("يجب إضافة تنوع واحد على الأقل (لون ومقاس) للمنتج");

            RuleForEach(x => x.Variants)
                .SetValidator(new CreateProductVariantValidator());
        }
    }
}
