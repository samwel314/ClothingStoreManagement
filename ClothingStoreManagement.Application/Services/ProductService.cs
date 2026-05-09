using AutoMapper;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services
{
    public class ProductService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<string>> CreateProductAsync(CreateProductWithVariantsDto dto)
        {
            var categoryExist = await _db.Categories.ExistsAsync((c) => c.Id == dto.CategoryId);
            if (!categoryExist)
                return Result<string>.Failure("هذه الفئة غير موجودة", ErrorType.notFound);

            var sameNameExist = await _db.Products.ExistsAsync((p) => p.Name == dto.Name.Trim() && p.CategoryId == dto.CategoryId);
            if (sameNameExist)
                return Result<string>.Failure("هذا الاسم موجود بالفعل في هذه الفئة", ErrorType.conflict);

            var sameSDKExist = await _db.Products.ExistsAsync((p) => p.SKU == dto.SKU.Trim());
            if (sameSDKExist)
                return Result<string>.Failure("هذا الباركود موجود بالفعل ", ErrorType.conflict);

            var Product = new Product(
                dto.Name!.Trim(),
                dto.SKU!.Trim(),
                dto.CategoryId);

            var colors = await _db.Colors.GetAll().AsNoTracking().ToDictionaryAsync(c => c.Id, c => c);
            var sizes = await _db.Sizes.GetAll().AsNoTracking().ToDictionaryAsync(c => c.Id, c => c);
            foreach (var variantDto in dto.Variants)
            {
                if (!colors.ContainsKey(variantDto.ColorId))
                    return Result<string>.Failure($"لون بالمعرف {variantDto.ColorId} غير موجود", ErrorType.notFound);

                if (!sizes.ContainsKey(variantDto.SizeId))
                    return Result<string>.Failure($"حجم بالمعرف {variantDto.SizeId} غير موجود", ErrorType.notFound);

                var variant = new ProductVariant(
                    Product.Id, Product.SKU,
                    variantDto.SizeId,
                    sizes[variantDto.SizeId].Code,
                    variantDto.ColorId,
                    colors[variantDto.ColorId].Code,
                    variantDto.StockQuantity,
                    variantDto.SellingPrice,
                    variantDto.PurchasePrice);
              await  _db.Movements.CreateAsync(new StockMovement
                {
                    ProductVariant = variant,   
                    QuantityChange = variant.StockQuantity,
                    StockAfter = variant.StockQuantity, 
                    Type = MovementType.Restock,
                    CreatedAt = DateTime.UtcNow
                });
                Product.AddVariant(variant);
            }

            await _db.Products.CreateAsync(Product);
            await _db.Save();
            _db.Clear();
            return Result<string>.Success("تم إضافة المنتج بنجاح ");
        }

        public async Task<Result<Pagination<ProductListDto>>>
            GetAllProductsAsync(int page, int pageSize = 5, bool? active = null, int? categoryId = null,
            string? searchTerm = null)
        {
            var baseQuery = _db.Products.GetAll();

            if (categoryId != null)
                baseQuery = baseQuery.Where(p => p.CategoryId == categoryId);

            if (active != null)
                baseQuery = baseQuery.Where(p => p.IsActive == active);
            if (searchTerm != null)
                baseQuery = baseQuery.Where(p => p.Name.Contains(searchTerm)
                          || p.SKU.Contains(searchTerm));
            var count = await baseQuery.CountAsync();
            var pagination = new Pagination<ProductListDto>(count, pageSize, page);

            var products = await baseQuery.OrderBy(p => p.Name)
                .Skip((pagination.pageNumber - 1) * pagination.pageSize).Take(pagination.pageSize).
                Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    SKU = p.SKU,
                    IsActive = p.IsActive,
                    LastUpdate = p.UpdatedAt == null ? p.CreatedAt : p.UpdatedAt.Value,
                    Variants = p.Variants.Select(v => new ProductVariantListDto
                    {
                        Id = v.Id,
                        Color = v.Color.Code,
                        Size = v.Size.Code,
                        StockQuantity = v.StockQuantity,
                        Price = v.SellingPrice,
                        Sku = v.VariantSKU
                    })
                }).ToListAsync();

            pagination.Items = products;
            return Result<Pagination<ProductListDto>>.Success(pagination);
        }
        public async Task<Result<string>> ActiveProductAsync(Guid Id)
        {
            var product = await _db.Products.FirstOrDefaultAsync((p) => p.Id == Id, true);
            if (product == null)
                return Result<string>.Failure("هذا المنتج غير موجود", ErrorType.notFound);
            product.Activate();
            await _db.Save();
            return Result<string>.Success();
        }
        public async Task<Result<string>> DeActiveProductAsync(Guid Id)
        {

            var product = await _db.Products.FirstOrDefaultAsync((p) => p.Id == Id, true);
            if (product == null)
                return Result<string>.Failure("هذا المنتج غير موجود", ErrorType.notFound);
            product.Deactivate();
            await _db.Save();
            return Result<string>.Success();
        }

        // 
        public async Task<Result<ProductListDto>>
    GetProductAsync(Guid Id)
        {
            var product = await _db.Products.GetAll().
                Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category.Name,
                    CategoryId = p.Category.Id,
                    SKU = p.SKU,
                    IsActive = p.IsActive,
                    LastUpdate = p.UpdatedAt == null ? p.CreatedAt : p.UpdatedAt.Value,
                    Variants = p.Variants.Select(v => new ProductVariantListDto
                    {
                        Id = v.Id,
                        Color = v.Color.Code,
                        ColorId = v.Color.Id,
                        Size = v.Size.Code,
                        SizeId = v.Size.Id,
                        Purchase = v.PurchasePrice,
                        StockQuantity = v.StockQuantity,
                        Price = v.SellingPrice,
                        Sku = v.VariantSKU
                    }) , 
                    TotalSalesCount = _db.Invoices.GetAll().
                    Where(i => i.Status == InvoiceStatus.completed)
                    .SelectMany(i => i.Items).
                    Where(ii => ii.ProductVariant.ProductId == p.Id).Count(),   
                    TotalSalesAmount = _db.Invoices.GetAll()
    .Where(i => i.Status == InvoiceStatus.completed)
    .SelectMany(i => i.Items)
    .Where(ii => ii.ProductVariant.ProductId == p.Id)
    .Sum(ii => ii.Quantity * ii.SellingPrice) ,
                    NetProfit = _db.Invoices.GetAll()
    .Where(i => i.Status == InvoiceStatus.completed)
    .SelectMany(i => i.Items)
    .Where(ii => ii.ProductVariant.ProductId == p.Id)
    .Sum(ii => (ii.SellingPrice - ii.PurchasePrice) * ii.Quantity) 
                }).FirstOrDefaultAsync((p) => p.Id == Id);
            if (product == null)
                return Result<ProductListDto>.Failure("هذا المنتج غير موجود", ErrorType.notFound);

              product.TopVariant = await  
                _db.Invoices.GetAll().Where(i => i.Status == InvoiceStatus.completed)
                .SelectMany(i => i.Items)!
                .Where(ii => ii.ProductVariant.ProductId == product.Id).
                GroupBy(ii => ii.ProductVariantId)!
                .Select(g => new TopVariantDTO
                {
                    Total = g.Sum(x => x.Quantity * (x.SellingPrice - x.PurchasePrice)),
                    Id = g.Key , 
                    Size = g.First().ProductVariant.Size.Code,
                    Code = g.First().ProductVariant.Color.Code

                }).OrderByDescending(x => x.Total).FirstOrDefaultAsync()!;

            
            return Result<ProductListDto>.Success(product);
        }

        public async Task<Result<string>> UpdateProductBasicAsync(UpdateProductBasicDto dto)
        {
            var product = await _db.Products.FirstOrDefaultAsync
                ((p) => p.Id == dto.ProductId, true);
            if (product == null)
                return Result<string>.Failure("هذا المنتج غير موجود", ErrorType.notFound);
            if (product.CategoryId != dto.CategoryId)
                product.ChangeCategory(dto.CategoryId);
            dto.Name = dto.Name.Trim();

            bool nameChanged = product.Name != dto.Name;
            bool categoryChanged = product.CategoryId != dto.CategoryId;

            if (nameChanged || categoryChanged)
            {
                var sameNameExist = await _db.Products.ExistsAsync(p =>
                    p.Name == dto.Name &&
                    p.CategoryId == dto.CategoryId &&
                    p.Id != dto.ProductId);

                if (sameNameExist)
                    return Result<string>.Failure("هذا الاسم موجود بالفعل في هذه الفئة", ErrorType.conflict);
            }

            if (categoryChanged)
                product.ChangeCategory(dto.CategoryId);

            if (nameChanged)
                product.UpdateName(dto.Name);
            await _db.Save();
            _db.Clear();
            return Result<string>.Success();
        }
        public async Task<Result<string>> UpdateProductSkuAsync(UpdateProductSkuDto dto)
        {
            var product = await _db.Products.FirstOrDefaultAsync
                ((p) => p.Id == dto.ProductId, true);
            if (product == null)
                return Result<string>.Failure("هذا المنتج غير موجود", ErrorType.notFound);
            dto.SKU = dto.SKU.Trim();

            if (product.SKU != dto.SKU)
            {
                var sameSDKExist = await _db.Products.ExistsAsync((p) => p.SKU == dto.SKU);
                if (sameSDKExist)
                    return Result<string>.Failure("هذا الباركود موجود بالفعل ", ErrorType.conflict);
                product.UpdateSKU(dto.SKU);
                var variants =
                      await _db.ProductVariants.GetAll(true).Include(pr => pr.Size)
                     .Include(pr => pr.Color).Where((pv) => pv.ProductId == dto.ProductId).ToListAsync();
                foreach (var variant in variants)
                {
                    variant.UpdateVariant(variant.SizeId, variant.Size.Code,
                        variant.ColorId, variant.Color.Code, product.SKU);
                }
            }

            await _db.Save();
            _db.Clear();
            return Result<string>.Success();
        }


        public async Task<Result<string>> CreateVariantAsync(CreateProductVariantDto dto)
        {
            var product = await _db.Products.FirstOrDefaultAsync
                                          ((p) => p.Id == dto.ProductId, true);
            if (product == null)
                return Result<string>.Failure("هذا المنتج غير موجود", ErrorType.notFound);

            var color = await _db.Colors.FirstOrDefaultAsync
                              ((c) => c.Id == dto.ColorId, true);
            if (color == null)
                return Result<string>.Failure("هذا اللون غير موجود", ErrorType.notFound);

            var size = await _db.Sizes.FirstOrDefaultAsync
                              ((s) => s.Id == dto.SizeId, true);
            if (size == null)
                return Result<string>.Failure("هذا المقاس  غير موجود", ErrorType.notFound);

            var isVariantExist = await _db.ProductVariants.ExistsAsync(pv => product.Id == pv.ProductId && pv.ColorId == dto.ColorId && pv.SizeId == dto.SizeId);
            if (isVariantExist)
                return Result<string>.Failure("هذا التنوع موجود بالفعل ", ErrorType.conflict);
            var variant = new ProductVariant
                (product.Id, product.SKU, dto.SizeId,
                size.Code, dto.ColorId, color.Code, dto.StockQuantity, dto.SellingPrice, dto.PurchasePrice);

            await _db.ProductVariants.CreateAsync(variant);
            await _db.Movements.CreateAsync(new StockMovement
            {
                ProductVariant = variant,
                QuantityChange = variant.StockQuantity,
                StockAfter = variant.StockQuantity,
                Type = MovementType.Restock,
                CreatedAt = DateTime.UtcNow , 
            });
            product.UpdateChanges();
            await _db.Save();
            _db.Clear();
            return Result<string>.Success("تم إضافة التنوع بنجاح ");
        }

        public async Task<Result<string>> UpdateVariantAsync(CreateProductVariantDto dto)
        {
            var variant = await _db.ProductVariants.FirstOrDefaultAsync(p => p.Id == dto.Id, true);
            if (variant == null) return Result<string>.Failure("هذا التنوع غير موجود", ErrorType.notFound);

            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId, true);
            if (product == null) return Result<string>.Failure("هذا المنتج غير موجود", ErrorType.notFound);

            bool isChanged = false;

            if (variant.PurchasePrice != dto.PurchasePrice)
            {
                variant.UpdatePurchasePrice(dto.PurchasePrice);
                isChanged = true;
            }
            if (variant.SellingPrice != dto.SellingPrice)
            {
                variant.UpdateSellingPrice(dto.SellingPrice);
                isChanged = true;
            }
            if (variant.StockQuantity != dto.StockQuantity)
            {
                var temp = dto.StockQuantity - variant.StockQuantity;
                variant.UpdateStockQuantity(dto.StockQuantity);
                await _db.Movements.CreateAsync(new StockMovement
                {
                    ProductVariant = variant,
                    QuantityChange = temp,
                    StockAfter = variant.StockQuantity,
                    Type = MovementType.ManualEdit,
                    CreatedAt = DateTime.UtcNow
                });

                isChanged = true;
            }

            if (variant.ColorId != dto.ColorId || variant.SizeId != dto.SizeId)
            {
                var isVariantExist = await _db.ProductVariants.ExistsAsync(pv =>
                    product.Id == pv.ProductId && pv.ColorId == dto.ColorId &&
                    pv.SizeId == dto.SizeId && pv.Id != variant.Id);

                if (isVariantExist)
                    return Result<string>.Failure("هذا التنوع موجود بالفعل", ErrorType.conflict);

                var color = await _db.Colors.FirstOrDefaultAsync(c => c.Id == dto.ColorId, true);
                var size = await _db.Sizes.FirstOrDefaultAsync(s => s.Id == dto.SizeId, true);

                if (color != null && size != null)
                {
                    variant.UpdateVariant(dto.SizeId, size.Code, dto.ColorId, color.Code, product.SKU);
                    isChanged = true;
                }
            }

            if (isChanged)
            {
                product.UpdateChanges();
                await _db.Save();
            }
            _db.Clear();
            return Result<string>.Success("تم تحديث التنوع بنجاح");
        }
        public async Task<Result<string>> DeleteVariant(Guid Id)
        {

            var variant = await _db.ProductVariants.FirstOrDefaultAsync((p) => p.Id == Id);
            if (variant == null)
                return Result<string>.Failure("هذا التنوع غير موجود", ErrorType.notFound);
            _db.ProductVariants.Delete(variant);
            await _db.Save();
            _db.Clear();
            return Result<string>.Success();
        }

        public async Task<Result< IEnumerable <VariantMovementsDTO>>> VariantMovementsAsync(Guid Id)
        {
            var variant = await _db.ProductVariants.FirstOrDefaultAsync((p) => p.Id == Id);
            if (variant == null)
                return Result<IEnumerable<VariantMovementsDTO>>.Failure("هذا التنوع غير موجود", ErrorType.notFound);
            var movements = await _db.Movements.GetAll().Where(m => m.ProductVariantId == Id)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new VariantMovementsDTO
                {
                    Id = m.Id,
                    QuantityChange = m.QuantityChange,
                    StockAfter = m.StockAfter,
                    Type = m.Type,
                    ReferenceId = m.ReferenceId,
                    CreatedAt = m.CreatedAt,
                    CreatedBy = m.CreatedBy
                }).ToListAsync();
            return Result<IEnumerable<VariantMovementsDTO>>.Success(movements);
        }
    }

}
