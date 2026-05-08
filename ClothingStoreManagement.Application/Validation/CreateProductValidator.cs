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
                .NotEmpty().WithMessage("كود المنتج (SKU) مطلوب")
                .Length(4, 100).WithMessage("كود المنتج يجب أن يكون بين 4 و 100 حرف");

            // قواعد فحص القسم
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("يجب اختيار القسم بشكل صحيح");

            // قواعد فحص قائمة التنوعات
            RuleFor(x => x.Variants)
                .NotEmpty().WithMessage("يجب إضافة تنوع واحد على الأقل (لون ومقاس) للمنتج");

            // هذا السطر يخبر FluentValidation بالدخول داخل كل عنصر في القائمة وفحصه باستخدام الـ Validator الخاص به
            RuleForEach(x => x.Variants)
                .SetValidator(new CreateProductVariantValidator());
        }
    }
}
