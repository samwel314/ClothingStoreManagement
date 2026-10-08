using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services;

public class SupplierService
{
    private readonly ApplicationDbContext _db;

    public SupplierService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<string>> CreateSupplierAsync(
        CreateSupplierDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<string>.Failure(
                "اسم المورد مطلوب",
                ErrorType.validation);
        }

        var supplier = new Supplier
        {
            Name = dto.Name.Trim(),
            Phone = dto.Phone,
            AdditionalPhone = dto.AdditionalPhone,
            Address = dto.Address,
            Notes = dto.Notes
        };

        await _db.Suppliers.AddAsync(supplier);
        await _db.SaveChangesAsync();

        return Result<string>.Success(
            "تم إنشاء المورد بنجاح");
    }

    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        return await _db.Suppliers
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => new SupplierDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                AdditionalPhone = x.AdditionalPhone,
                Address = x.Address,
                Notes = x.Notes,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,

                TotalPurchases = x.Purchases
                    .Sum(p => (decimal?)p.TotalAmount) ?? 0,

                TotalPaid = x.Purchases
                    .Sum(p => (decimal?)p.PaidAmount) ?? 0,

                Remaining =
                    (x.Purchases
                        .Sum(p => (decimal?)p.TotalAmount) ?? 0)
                    -
                    (x.Purchases
                        .Sum(p => (decimal?)p.PaidAmount) ?? 0)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierDto>> GetAllActiveSuppliersAsync()
    {
        return await _db.Suppliers
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new SupplierDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                AdditionalPhone = x.AdditionalPhone,
                Address = x.Address,
                Notes = x.Notes,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,

                TotalPurchases = x.Purchases
                    .Sum(p => (decimal?)p.TotalAmount) ?? 0,

                TotalPaid = x.Purchases
                    .Sum(p => (decimal?)p.PaidAmount) ?? 0,

                Remaining =
                    (x.Purchases
                        .Sum(p => (decimal?)p.TotalAmount) ?? 0)
                    -
                    (x.Purchases
                        .Sum(p => (decimal?)p.PaidAmount) ?? 0)
            })
            .ToListAsync();
    }

    public async Task<Result<SupplierDto>> GetSupplierAsync(int id)
    {
        var supplier = await _db.Suppliers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new SupplierDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                AdditionalPhone = x.AdditionalPhone,
                Address = x.Address,
                Notes = x.Notes,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,

                TotalPurchases = x.Purchases
                    .Sum(p => (decimal?)p.TotalAmount) ?? 0,

                TotalPaid = x.Purchases
                    .Sum(p => (decimal?)p.PaidAmount) ?? 0,

                Remaining =
                    (x.Purchases
                        .Sum(p => (decimal?)p.TotalAmount) ?? 0)
                    -
                    (x.Purchases
                        .Sum(p => (decimal?)p.PaidAmount) ?? 0)
            })
            .FirstOrDefaultAsync();

        if (supplier == null)
        {
            return Result<SupplierDto>.Failure(
                "المورد غير موجود",
                ErrorType.notFound);
        }

        return Result<SupplierDto>.Success(supplier);
    }

    public async Task<Result<string>> UpdateSupplierAsync(
        UpdateSupplierDto dto)
    {
        var supplier = await _db.Suppliers
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (supplier == null)
        {
            return Result<string>.Failure(
                "المورد غير موجود",
                ErrorType.notFound);
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return Result<string>.Failure(
                "اسم المورد مطلوب",
                ErrorType.validation);
        }

        supplier.Name = dto.Name.Trim();
        supplier.Phone = dto.Phone;
        supplier.AdditionalPhone = dto.AdditionalPhone;
        supplier.Address = dto.Address;
        supplier.Notes = dto.Notes;

        await _db.SaveChangesAsync();

        return Result<string>.Success(
            "تم تحديث بيانات المورد بنجاح");
    }

    public async Task<Result<string>> ToggleStatusAsync(int id)
    {
        var supplier = await _db.Suppliers
            .FirstOrDefaultAsync(x => x.Id == id);

        if (supplier == null)
        {
            return Result<string>.Failure(
                "المورد غير موجود",
                ErrorType.notFound);
        }

        supplier.ToggleStatus();

        await _db.SaveChangesAsync();

        return Result<string>.Success(
            "تم تحديث حالة المورد بنجاح");
    }
    public async Task<Result<string>> CreatePurchaseAsync(
    CreateSupplierPurchaseDto dto)
    {
        if (dto.SupplierId <= 0)
            return Result<string>.Failure(
                "المورد مطلوب",
                ErrorType.validation);

        if (dto.TotalAmount <= 0)
            return Result<string>.Failure(
                "قيمة الشراء يجب أن تكون أكبر من صفر",
                ErrorType.validation);

        if (dto.PaidAmount < 0)
            return Result<string>.Failure(
                "المبلغ المدفوع لا يمكن أن يكون سالباً",
                ErrorType.validation);

        if (dto.PaidAmount > dto.TotalAmount)
            return Result<string>.Failure(
                "المبلغ المدفوع لا يمكن أن يكون أكبر من قيمة الشراء",
                ErrorType.validation);

        var supplierExists = await _db.Suppliers
            .FirstOrDefaultAsync(x => x.Id == dto.SupplierId && x.IsActive);

        if (supplierExists == null)
            return Result<string>.Failure(
                "المورد غير موجود أو غير نشط",
                ErrorType.notFound);

        // التأكد من أن الخزنة تكفي للمبلغ المدفوع
        if (dto.PaidAmount > 0)
        {
            var treasuryTotal = await _db.TreasuryTransactions
                .SumAsync(x => x.Amount);

            if (dto.PaidAmount > treasuryTotal)
            {
                return Result<string>.Failure(
                    $"رصيد الخزنة غير كافٍ. الرصيد الحالي {treasuryTotal:N2} ج.م",
                    ErrorType.validation);
            }
        }

        var purchase = new SupplierPurchase
        {
            SupplierId = dto.SupplierId,
            TotalAmount = dto.TotalAmount,
            PaidAmount = dto.PaidAmount,
            Notes = dto.Notes
        };

        await _db.SupplierPurchases.AddAsync(purchase);

        // تسجيل المبلغ المدفوع كحركة مصروف من الخزنة
        if (dto.PaidAmount > 0)
        {
            await _db.TreasuryTransactions.AddAsync(
                new MainTreasuryTransaction(
                    -Math.Abs(dto.PaidAmount ) ,
                    TreasuryTransactionType.SupplierPayment,
                    $"دفع للمورد :  {supplierExists.Name} جزء عمليه شراء  - {dto.PaidAmount:N2} ج.م")
                {
                    SupplierId = dto.SupplierId 
                });
        }

        await _db.SaveChangesAsync();
         _db.ChangeTracker.Clear   (); 
        return Result<string>.Success(
            "تم تسجيل عملية الشراء بنجاح");
    }
    public async Task<Result<string>> CreateSupplierPaymentAsync(
    CreateSupplierPaymentDto dto)
    {
        if (dto.SupplierId <= 0)
            return Result<string>.Failure(
                "المورد مطلوب",
                ErrorType.validation);

        if (dto.Amount <= 0)
            return Result<string>.Failure(
                "مبلغ السداد يجب أن يكون أكبر من صفر",
                ErrorType.validation);

        var supplier = await _db.Suppliers
            .FirstOrDefaultAsync(x => x.Id == dto.SupplierId);

        if (supplier == null)
            return Result<string>.Failure(
                "المورد غير موجود",
                ErrorType.notFound);

        var remaining = await _db.SupplierPurchases
            .Where(x => x.SupplierId == dto.SupplierId)
            .SumAsync(x => x.TotalAmount - x.PaidAmount);

        if (remaining <= 0)
            return Result<string>.Failure(
                "لا يوجد مبلغ مستحق على هذا المورد",
                ErrorType.validation);

        if (dto.Amount > remaining)
            return Result<string>.Failure(
                $"مبلغ السداد أكبر من المبلغ المستحق. المتبقي {remaining:N2} ج.م",
                ErrorType.validation);

        // التأكد من رصيد الخزنة
        var treasuryTotal = await _db.TreasuryTransactions
            .SumAsync(x => x.Amount);

        if (dto.Amount > treasuryTotal)
            return Result<string>.Failure(
                $"رصيد الخزنة غير كافٍ. الرصيد الحالي {treasuryTotal:N2} ج.م",
                ErrorType.validation);

        // نوزع السداد على أقدم المشتريات المستحقة أولاً
        var purchases = await _db.SupplierPurchases
            .Where(x =>
                x.SupplierId == dto.SupplierId &&
                x.TotalAmount > x.PaidAmount)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        var remainingPayment = dto.Amount;

        foreach (var purchase in purchases)
        {
            if (remainingPayment <= 0)
                break;

            var purchaseRemaining =
                purchase.TotalAmount - purchase.PaidAmount;

            var paymentAmount =
                Math.Min(remainingPayment, purchaseRemaining);

            purchase.PaidAmount += paymentAmount;
            remainingPayment -= paymentAmount;
        }

        await _db.TreasuryTransactions.AddAsync(
            new MainTreasuryTransaction(
                -Math.Abs(dto.Amount),
                TreasuryTransactionType.SupplierPayment,
                string.IsNullOrWhiteSpace(dto.Notes)
                    ? $"سداد للمورد: {supplier.Name}"
                    : dto.Notes));

        await _db.SaveChangesAsync();
         _db.ChangeTracker.Clear();   
        return Result<string>.Success(
            "تم سداد المبلغ بنجاح");
    }
    public async Task<IEnumerable<SupplierPurchaseDto>>
        GetSupplierPurchasesAsync(int supplierId)
    {
        return await _db.SupplierPurchases
            .AsNoTracking()
            .Where(x => x.SupplierId == supplierId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SupplierPurchaseDto
            {
                Id = x.Id,
                TotalAmount = x.TotalAmount,
                PaidAmount = x.PaidAmount,
                RemainingAmount =
                    x.TotalAmount - x.PaidAmount,
                CreatedAt = x.CreatedAt,
                Notes = x.Notes
            })
            .ToListAsync();
    }

}