using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagement.Application.Services
{
    public class CategoryService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<CategoryDTO>> CreateCategoryAsync(CreateUpdateCategoryDto model)
        {
            var exist = await _db.Categories.ExistsAsync((c) => c.Name == model.Name.Trim());
            if (exist)
                return Result<CategoryDTO>.Failure("هذا الفئة موجودة بالفعل " , ErrorType.conflict);
            var category = new Category(model.Name.ToLower().Trim());
            await _db.Categories.CreateAsync(category );
            await _db.Save();
            return Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category));
        }
        public async Task<Result<CategoryDTO>> GetByIdAsync(int id )
        {
            var category = await _db.Categories.FirstOrDefaultAsync( (c) => c.Id == id );
            if (category == null)
                return Result<CategoryDTO>.Failure("هذا الفئة غير موجودة" , ErrorType.notFound);
            return  Result<CategoryDTO>.Success(_mapper.Map<CategoryDTO>(category)); 
        }
        public async Task<Result<IEnumerable<CategoryDTO>>> GetCategoriesLookUpAsync()
        {
            var Categories  = await _db.Categories.GetAll().Where(c=> c.IsActive)
                . ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider).ToListAsync();
            return Result <IEnumerable<CategoryDTO>>.Success(Categories);
        }
        public async Task<Result<IEnumerable<CategoryListDTO>>>
            GetCategoriesAsync(string ? searchTerm = null , bool ? active = null )
        {
            var CategoriesQuery =  _db.Categories.GetAll();
            if (active != null)
                CategoriesQuery = CategoriesQuery.Where(c => c.IsActive == active);
            if (searchTerm != null)
                CategoriesQuery = CategoriesQuery.Where(c => c.Name.Contains(searchTerm));
           var CategoriesDtoQuery =  CategoriesQuery
               .Select(c => new CategoryListDTO
               {
                   Id = c.Id,
                   Name = c.Name,
                   UpdatedAt = c.UpdatedAt,
                   CreatedAt = c.CreatedAt,
                   IsActive = c.IsActive,
                   ProductsCount = c.Products.Count(),
               });
          
            var Categories = await CategoriesDtoQuery.ToListAsync();
            return Result<IEnumerable<CategoryListDTO>>.Success(Categories);
        }
        public async Task <Result <string>> UpdateCategoryAsync (int id ,  CreateUpdateCategoryDto model )
        {
            var category = await _db.Categories.FirstOrDefaultAsync((c) => c.Id == id , true);
            if (category == null)
                return Result<string>.Failure("هذه الفئة غير موجودة" , ErrorType.notFound);
            if (!category.Name.Equals(model.Name.Trim()))  //**---
            {
                var exist =  await _db.Categories.ExistsAsync( (c) => c.Name == model.Name.Trim() );
                if (exist)
                    return Result<string>.Failure("هذه الفئة موجودة بالفعل" , ErrorType.conflict);
                category.UpdateName(model.Name); 
                await _db.Save();     
            }
            return Result<string>.Success($"تم تحديث الفئة بنجاح"); 
        }
        public async Task<Result<string>> ActiveCategoryAsync(int Id  )
        {
            var category = await _db.Categories.FirstOrDefaultAsync((c) => c.Id == Id, true);
            if (category == null)
                return Result<string>.Failure("هذه الفئة غير موجودة", ErrorType.notFound);
            category.Activate();    
            await _db.Save();
            return Result<string>.Success("تم تفعيل الفئة بنجاح");
        }
        public async Task<Result<string>> DeActiveCategoryAsync(int Id )
        {
            var category = await _db.Categories.FirstOrDefaultAsync((c) => c.Id == Id, true);
            if (category == null)
                return Result<string>.Failure("هذه الفئة غير موجودة", ErrorType.notFound);
            category.Deactivate();
            await _db.Save();
            return Result<string>.Success("تم تعطيل الفئة بنجاح");
        }
    }
}


