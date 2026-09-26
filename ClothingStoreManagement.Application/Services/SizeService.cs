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
        public async Task<Result<string>> UpdateSizeAsync(
    int id,
    CreateUpdateSizeDto model)
        {
            var size = await _db.Sizes
                .FirstOrDefaultAsync(s => s.Id == id, true);

            if (size == null)
                return Result<string>.Failure(
                    "المقاس غير موجود",
                    ErrorType.notFound);

            var name = model.Name.Trim();
            var code = model.Code.Trim().ToUpper();

            var nameChanged = !size.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase);

            var codeChanged = !size.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase);

            if (!nameChanged && !codeChanged)
                return Result<string>.Success(
                    "لم يتم إجراء أي تغييرات");

            if (nameChanged)
            {
                var nameExists = await _db.Sizes.ExistsAsync(
                    s => s.Name == name && s.Id != id);

                if (nameExists)
                    return Result<string>.Failure(
                        "هذا المقاس موجود بالفعل",
                        ErrorType.conflict);
            }

            if (codeChanged)
            {
                var codeExists = await _db.Sizes.ExistsAsync(
                    s => s.Code == code && s.Id != id);

                if (codeExists)
                    return Result<string>.Failure(
                        "كود المقاس مستخدم بالفعل",
                        ErrorType.conflict);
            }

            size.Update(name, code);

            await _db.Save();

            return Result<string>.Success("تم التعديل بنجاح");
        }

        public async Task<Result<IEnumerable<SizeListDTO>>> GetSizesAsync(string? searchTerm = null)
        {
            var query = _db.Sizes.GetAll();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(s => s.Name.Contains(searchTerm) || s.Code.Contains(searchTerm));

            var sizes = await query.ProjectTo<SizeListDTO>(_mapper.ConfigurationProvider).ToListAsync();
            return Result<IEnumerable<SizeListDTO>>.Success(sizes);
        }

        public async Task<Result<SizeListDTO>> CreateSizeAsync(CreateUpdateSizeDto model)
        {
            var name = model.Name.Trim();
            var code = model.Code.Trim().ToUpper();

            var nameExists = await _db.Sizes.ExistsAsync(
                s => s.Name == name);

            if (nameExists)
                return Result<SizeListDTO>.Failure(
                    "هذا المقاس موجود بالفعل",
                    ErrorType.conflict);

            var codeExists = await _db.Sizes.ExistsAsync(
                s => s.Code == code);

            if (codeExists)
                return Result<SizeListDTO>.Failure(
                    "كود المقاس مستخدم بالفعل",
                    ErrorType.conflict);

            var size = new Size(name, code);

            await _db.Sizes.CreateAsync(size);
            await _db.Save();

            return Result<SizeListDTO>.Success(
                _mapper.Map<SizeListDTO>(size));
        }
    }
}
