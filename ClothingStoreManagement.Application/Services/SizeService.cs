using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services
{
    public class SizeService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;

        public SizeService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Result<SizeListDTO>> CreateSizeAsync(CreateUpdateSizeDto model)
        {
            var exist = await _db.Sizes.ExistsAsync(s => s.Name.ToLower() == model.Name.Trim().ToLower());
            if (exist)
                return Result<SizeListDTO>.Failure("هذا المقاس موجود بالفعل", ErrorType.conflict);

            var size = new Size(model.Name.Trim(), model.Code.Trim());
            await _db.Sizes.CreateAsync(size);
            await _db.Save();
            return Result<SizeListDTO>.Success(_mapper.Map<SizeListDTO>(size));
        }

        public async Task<Result<IEnumerable<SizeListDTO>>> GetSizesAsync(string? searchTerm = null)
        {
            var query = _db.Sizes.GetAll();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(s => s.Name.Contains(searchTerm) || s.Code.Contains(searchTerm));

            var sizes = await query.ProjectTo<SizeListDTO>(_mapper.ConfigurationProvider).ToListAsync();
            return Result<IEnumerable<SizeListDTO>>.Success(sizes);
        }

        public async Task<Result<string>> UpdateSizeAsync(int id, CreateUpdateSizeDto model)
        {
            var size = await _db.Sizes.FirstOrDefaultAsync(s => s.Id == id, true);
            if (size == null) return Result<string>.Failure("المقاس غير موجود", ErrorType.notFound);

            var exist = await _db.Sizes.ExistsAsync(s => s.Name.ToLower() == model.Name.Trim().ToLower() && s.Id != id);
            if (exist) return Result<string>.Failure("هذا المقاس موجود بالفعل", ErrorType.conflict);

            size.Update(model.Name.Trim(), model.Code.Trim());
            await _db.Save();
            return Result<string>.Success("تم التعديل بنجاح");
        }
    }
}
