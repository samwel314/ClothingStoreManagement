using AutoMapper;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace ClothingStoreManagement.Application.Services
{
    public class InvoiceService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public InvoiceService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<InvoiceDTO>> CreateInvoiceAsync()
        {
            var invoice = new Invoice();
            await _db.Invoices.CreateAsync(invoice);
            await _db.Save();
            return Result<InvoiceDTO>.Success(new InvoiceDTO
            {
                Id = invoice.Id,
                Status = invoice.Status,
                SerialNumber = invoice.Serial,
                LastUpdate = invoice.LastUpdatedAt == null ? invoice.CreatedAt : invoice.LastUpdatedAt.Value,
                TotalAmount = invoice.TotalAmount,
           //     TotalAmountWithDiscount = invoice.TotalAmountWithDiscount,
                Items = new List<InvoiceItemDetailsDto>()
            });
        }
        public async Task<Result<InvoiceItemDetailsDto>> GetVariant(string sku)
        {
            var variantDto = _db.ProductVariants.GetAll().Where(pv => pv.VariantSKU == sku.Trim() && pv.StockQuantity > 0)
                .Select(pv => new InvoiceItemDetailsDto
                {
                    Id = pv.Id,
                    ProductName = pv.Product.Name,
                    Color = pv.Color.Code,
                    Size = pv.Size.Code,
                    SKU = pv.VariantSKU,
                    Price = pv.SellingPrice,
                    AvailableQuantity = pv.StockQuantity
                }).FirstOrDefault();

            if (variantDto == null)
                return Result<InvoiceItemDetailsDto>.Failure("هذا المنتج غير متوفر الان ", ErrorType.notFound);
            return Result<InvoiceItemDetailsDto>.Success(variantDto);
        }
        public async Task<Result<string>> PendingInvoiceWithItems(InvoiceDTO dto)
        {
            if (dto.Items.Count() == 0)
                return Result<string>.Failure("لا يكن انشاء او ارشافة  فاتورة فارغة ", ErrorType.notFound);
            var invoice = await _db.Invoices.GetAll(true).Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == dto.Id);
            if (invoice == null)
                return Result<string>.Failure("هذا الفاتورة غير موجودة", ErrorType.notFound);

            var newItems = new List<InvoiceItem>();
            foreach (var item in dto.Items)
            {
                var variant = await _db.ProductVariants.FirstOrDefaultAsync(pv => pv.Id == item.Id);
                if (variant == null)
                    return Result<string>.Failure($" هذا الصنف غير موجود ({item.ProductName})  ", ErrorType.notFound);
                if (!variant.CanWithdraw(item.Quantity))
                    return Result<string>.Failure($"  الكيمة المطلوبة غير متوفرة     ({item.ProductName})  ", ErrorType.notFound);
                newItems.Add(new InvoiceItem(invoice.Id, variant.Id, item.Quantity, variant.SellingPrice, variant.PurchasePrice, item.Discount));
            }
           // invoice.SetTotalWithDiscount(dto.TotalAmountWithDiscount);
            invoice.SetTotal(dto.TotalAmount);
            invoice.Items = newItems;
            await _db.Save();
            return Result<string>.Success(" تم ارشافة الفاتور ");
        }
        public async Task<Result<string>> CompleteInvoiceWithItems(InvoiceDTO dto)
        {
            if (dto.Items.Count() == 0)
                return Result<string>.Failure("لا يكن انشاء او ارشافة  فاتورة فارغة ", ErrorType.notFound);
            var invoice = await _db.Invoices.GetAll(true).Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == dto.Id);
            if (invoice == null)
                return Result<string>.Failure("هذا الفاتورة غير موجودة", ErrorType.notFound);
            if (invoice.Status == InvoiceStatus.completed || invoice.Status == InvoiceStatus.returned  )
            {
                return Result<string>.Failure("لا يمكن تيغر حالة الفاتورة بعد اكمالها و ارتجاعها ", ErrorType.conflict);
            }
            var newItems = new List<InvoiceItem>();
            foreach (var item in dto.Items)
            {
                var variant = await _db.ProductVariants.FirstOrDefaultAsync(pv => pv.Id == item.Id, true);
                if (variant == null)
                    return Result<string>.Failure($" هذا الصنف غير موجود ({item.ProductName})  ", ErrorType.notFound);
                if (!variant.CanWithdraw(item.Quantity))
                    return Result<string>.Failure($"  الكيمة المطلوبة غير متوفرة     ({item.ProductName})  ", ErrorType.notFound);
                variant.Withdraw(item.Quantity);
                newItems.Add(new InvoiceItem(invoice.Id, variant.Id, item.Quantity, variant.SellingPrice, variant.PurchasePrice, item.Discount));
            }
           // invoice.SetTotalWithDiscount(dto.TotalAmountWithDiscount);
            invoice.SetTotal(dto.TotalAmount);
            invoice.UpdateStatus(InvoiceStatus.completed);
            invoice.Items = newItems;
            await _db.Save();
            return Result<string>.Success(" تم حفظ الفاتورة بنجاح ");
        }

    }
}


