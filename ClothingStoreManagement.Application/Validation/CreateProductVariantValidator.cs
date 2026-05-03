using ClothingStoreManagement.Application.DTO;
using FluentValidation;

namespace ClothingStoreManagement.Application.Validation
{
    public class CreateProductVariantValidator : AbstractValidator<CreateProductVariantDto>
    {
        public CreateProductVariantValidator()
        {
            // قواعد فحص المقاس
            RuleFor(x => x.SizeId)
                .GreaterThan(0).WithMessage("المقاس مطلوب");

            // قواعد فحص اللون
            RuleFor(x => x.ColorId)
                .GreaterThan(0).WithMessage("اللون مطلوب");

            // قواعد فحص كمية المخزن
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(1).WithMessage("الكمية يجب أن تكون 1 على الأقل");

            // قواعد فحص سعر البيع
            RuleFor(x => x.SellingPrice)
                .GreaterThan(0m).WithMessage("سعر البيع يجب أن يكون أكبر من صفر");

            // قواعد فحص سعر الشراء
            RuleFor(x => x.PurchasePrice)
                .GreaterThan(0.0m).WithMessage("سعر الشراء يجب أن يكون أكبر من صفر")
            ;
        }
    }
}
