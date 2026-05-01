using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Application.Validation;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
                Product.AddVariant(variant);
            }

            await _db.Products.CreateAsync(Product);
            await _db.Save();
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
                        Size = v.Size.Code,
                        Purchase = v.PurchasePrice , 
                        StockQuantity = v.StockQuantity,
                        Price = v.SellingPrice,
                        Sku = v.VariantSKU
                    })
                }).FirstOrDefaultAsync((p) => p.Id == Id);
            if (product == null)
                return Result<ProductListDto>.Failure("هذا المنتج غير موجود", ErrorType.notFound);
            return Result<ProductListDto>.Success(product);
        }

        public async Task<Result<string>> UpdateProductBasicAsync(UpdateProductBasicDto dto )
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
            return Result<string>.Success();
        }
        public async Task<Result<string>> UpdateProductSkuAsync(UpdateProductSkuDto dto)
        {
            var product = await _db.Products.FirstOrDefaultAsync
                ((p) => p.Id == dto.ProductId, true);
            if (product == null)
                return Result<string>.Failure("هذا المنتج غير موجود", ErrorType.notFound);
            dto.SKU = dto.SKU.Trim();

            if (product.SKU != dto.SKU )
            {
                var sameSDKExist = await _db.Products.ExistsAsync((p) => p.SKU == dto.SKU);
                if (sameSDKExist)
                    return Result<string>.Failure("هذا الباركود موجود بالفعل ", ErrorType.conflict);
                product.UpdateSKU(dto.SKU);
            }
            
            await _db.Save();
            return Result<string>.Success();
        }


    }

}
