using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services
{
    public class ColorService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public ColorService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<ColorListDTO>> CreateColorAsync(CreateUpdateColorDto model)
        {
            var name = model.Name.Trim();
            var code = model.Code.Trim().ToUpper();

            var nameExists = await _db.Colors.ExistsAsync(
                c => c.Name == name);

            if (nameExists)
                return Result<ColorListDTO>.Failure(
                    "هذا اللون موجود بالفعل",
                    ErrorType.conflict);

            var codeExists = await _db.Colors.ExistsAsync(
                c => c.Code == code);

            if (codeExists)
                return Result<ColorListDTO>.Failure(
                    "كود اللون مستخدم بالفعل",
                    ErrorType.conflict);

            var color = new Color(
                name,
                code,
                model.HexCode);

            await _db.Colors.CreateAsync(color);
            await _db.Save();

            return Result<ColorListDTO>.Success(
                _mapper.Map<ColorListDTO>(color));
        }
        public async Task<Result<ColorListDTO>> GetByIdAsync(int id)
        {
            var color = await _db.Colors.FirstOrDefaultAsync((c) => c.Id == id);
            if (color == null)
                return Result<ColorListDTO>.Failure("هذا اللون غير موجود ", ErrorType.notFound);
            return Result<ColorListDTO>.Success(_mapper.Map<ColorListDTO>(color));
        }

        public async Task<Result<IEnumerable<ColorListDTO>>>
            GetColorsAsync(string? searchTerm = null)
        {
            var ColorsQuery = _db.Colors.GetAll();

            if (searchTerm != null)
                ColorsQuery = ColorsQuery.Where(c => c.Name.Contains(searchTerm));
            var ColorsDtoQuery = ColorsQuery.ProjectTo<ColorListDTO>(_mapper.ConfigurationProvider);
            var Colors = await ColorsDtoQuery.ToListAsync();
            return Result<IEnumerable<ColorListDTO>>.Success(Colors);
        }
        public async Task<Result<string>> UpdateColorAsync(
        int id,
        CreateUpdateColorDto model)
        {
            var color = await _db.Colors
                .FirstOrDefaultAsync(c => c.Id == id, true);

            if (color == null)
                return Result<string>.Failure(
                    "هذا اللون غير موجود",
                    ErrorType.notFound);

            var name = model.Name.Trim();
            var code = model.Code.Trim().ToUpper();
            var hexCode = model.HexCode.Trim().ToUpper();

            var nameChanged = !color.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase);

            var codeChanged = !color.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase);

            var hexCodeChanged = !color.HexCode.Equals(
                hexCode,
                StringComparison.OrdinalIgnoreCase);

            if (!nameChanged && !codeChanged && !hexCodeChanged)
                return Result<string>.Success("لم يتم إجراء أي تغييرات");

            if (nameChanged)
            {
                var nameExists = await _db.Colors.ExistsAsync(
                    c => c.Name == name && c.Id != id);

                if (nameExists)
                    return Result<string>.Failure(
                        "هذا اللون موجود بالفعل",
                        ErrorType.conflict);
            }

            if (codeChanged)
            {
                var codeExists = await _db.Colors.ExistsAsync(
                    c => c.Code == code && c.Id != id);

                if (codeExists)
                    return Result<string>.Failure(
                        "كود اللون مستخدم بالفعل",
                        ErrorType.conflict);
            }

            color.Update(name, code, hexCode);

            await _db.Save();

            return Result<string>.Success("تم تعديل اللون بنجاح");
        }
    }
}
