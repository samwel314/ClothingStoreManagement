using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            _db.Invoices.Detach(invoice);
            return Result<InvoiceDTO>.Success(new InvoiceDTO
            {
                Id = invoice.Id,
                Status = invoice.Status,
                SerialNumber = invoice.Serial,
                LastUpdate = invoice.LastUpdatedAt == null ? invoice.CreatedAt : invoice.LastUpdatedAt.Value,
                TotalAmount = invoice.TotalAmount,
                TotalAmountWithDiscount = invoice.TotalAmountWithDiscount,
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
                newItems.Add(new InvoiceItem( item.Quantity, variant.SellingPrice, variant.PurchasePrice, item.Discount)
                {
                    ProductVariant = variant,
                    Invoice = invoice
                });
            }
            invoice.SetTotalWithDiscount(dto.TotalAmountWithDiscount);
            invoice.SetTotal(dto.TotalAmount);
            invoice.Items = newItems;
            await _db.Save();
            return Result<string>.Success(" تم ارشافة الفاتور ");
        }
        public async Task<Result<InvoiceDTO>> CompleteInvoiceWithItems(InvoiceDTO dto)
        {
            if (dto.Items.Count() == 0)
                return Result<InvoiceDTO>.Failure("لا يكن انشاء او ارشافة  فاتورة فارغة ", ErrorType.notFound);
            var invoice = await _db.Invoices.GetAll(true).Include(i => i.Items).FirstOrDefaultAsync(i => i.Id == dto.Id);
            if (invoice == null)
                return Result<InvoiceDTO>.Failure("هذا الفاتورة غير موجودة", ErrorType.notFound);
            if (invoice.Status == InvoiceStatus.completed)
            {
                return Result<InvoiceDTO>.Failure("لا يمكن تيغر حالة الفاتورة بعد اكمالها   ", ErrorType.conflict);
            }

            var newItems = new List<InvoiceItem>();
            Invoice targetInvoice;
            if (invoice.Status == InvoiceStatus.returned)
            {
                Invoice newInvoice = new();
                await _db.Invoices.CreateAsync(newInvoice);
                targetInvoice = newInvoice;
            }
            else
            {
                targetInvoice = invoice;
            }
            foreach (var item in dto.Items)
            {
                var variant = await _db.ProductVariants.FirstOrDefaultAsync(pv => pv.Id == item.Id, true);
                if (variant == null)
                    return Result<InvoiceDTO>.Failure($" هذا الصنف غير موجود ({item.ProductName})  ", ErrorType.notFound);
                if (!variant.CanWithdraw(item.Quantity))
                    return Result<InvoiceDTO>.Failure($"  الكيمة المطلوبة غير متوفرة     ({item.ProductName})  ", ErrorType.notFound);
                variant.Withdraw(item.Quantity);
                newItems.Add(new InvoiceItem(item.Quantity, variant.SellingPrice, variant.PurchasePrice, item.Discount)
                {
                    ProductVariant = variant,   
                    Invoice = targetInvoice 
                });
            }
            targetInvoice.SetTotalWithDiscount(dto.TotalAmountWithDiscount);
            targetInvoice.SetTotal(dto.TotalAmount);
            targetInvoice.UpdateStatus(InvoiceStatus.completed);
            targetInvoice.Items = newItems;
            await _db.Save();
            dto.Id = targetInvoice.Id;
            dto.SerialNumber = targetInvoice.Serial;
            dto.LastUpdate = targetInvoice.LastUpdatedAt ?? targetInvoice.CreatedAt;

            return Result<InvoiceDTO>.Success(dto);
        }
        public async Task<Result<InvoiceDTO>> GetInvoiceById(int id)
        {
            var invoiceDto = await _db.Invoices.GetAll().Where(invoice => invoice.Id == id).Select(invoice => new InvoiceDTO()
            {
                Id = invoice.Id,
                Status = invoice.Status,
                SerialNumber = invoice.Serial,
                LastUpdate = invoice.LastUpdatedAt == null ? invoice.CreatedAt : invoice.LastUpdatedAt.Value,
                Items = new List<InvoiceItemDetailsDto>(invoice.Items.Select(i => new InvoiceItemDetailsDto
                {
                    Id = i.ProductVariantId,
                    ProductName = i.ProductVariant.Product.Name,
                    Color = i.ProductVariant.Color.Code,
                    Size = i.ProductVariant.Size.Code,
                    SKU = i.ProductVariant.VariantSKU,
                    Price = i.SellingPrice,
                    AvailableQuantity = i.ProductVariant.StockQuantity,
                    Quantity = i.Quantity,
                    Discount = i.Discount
                }).ToList()),
            }).FirstOrDefaultAsync(); 
            if (invoiceDto == null)
                return Result<InvoiceDTO>.Failure("هذا الفاتورة غير موجودة", ErrorType.notFound);   

            invoiceDto.UpdateTotalWithDiscount();
            invoiceDto.UpdateTotal();   
            invoiceDto.UpdateTotalQuantity();   
            return Result<InvoiceDTO>.Success(invoiceDto);
        }
        public async Task Remove(int id)
        {
            var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == id);
            if (invoice != null)
            {
                _db.Invoices.Delete(invoice);
                await _db.Save();
            }
        }
        public async Task<Result<IEnumerable<InvoiceLockUpDTO>>>
            GetInvoicesLookUpAsync(string? searchTerm = null, InvoiceStatus? status = null , DateTime ? start = null , DateTime ? end = null)
        {
            var invoices = _db.Invoices.GetAll();
            if (status != null)
            {
                invoices = invoices.Where(i => i.Status == status);
            }
            if (start != null)
            {
                invoices = invoices.Where(i => (i.LastUpdatedAt ?? i.CreatedAt) >= start);
            }
            if (end != null)
            {
                invoices = invoices.Where(i => (i.LastUpdatedAt ?? i.CreatedAt) < end.Value.AddDays(1));
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                invoices = invoices.Where(i => i.Serial.Contains(searchTerm));
            }

            var invoiceLookUpDtos = await invoices.Select(i => new InvoiceLockUpDTO
            {
                Id = i.Id,
                Serial = i.Serial,
                Status = i.Status,
                TotalAmount = i.TotalAmountWithDiscount,
                LastUpdatedAt = i.LastUpdatedAt ?? i.CreatedAt
            }).OrderByDescending(i => i.LastUpdatedAt).ToListAsync();
            return Result<IEnumerable<InvoiceLockUpDTO>>.Success(invoiceLookUpDtos);
        }
         
    
        public async Task<Result<string>> ReturnInvoice (int id)
        {
            var invoice = await _db.Invoices.GetAll(true).Include(i => i.Items)
                .ThenInclude(i => i.ProductVariant).FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return Result<string>.Failure("هذا الفاتورة غير موجودة", ErrorType.notFound);
            foreach (var item in invoice.Items)
            {
                item.ProductVariant.Deposit(item.Quantity);
            }
            invoice.UpdateStatus(InvoiceStatus.returned);   
            await _db.Save(); 
            return Result<string>.Success("تمت إعادة الفاتورة بنجاح");  
        }
    }
}


