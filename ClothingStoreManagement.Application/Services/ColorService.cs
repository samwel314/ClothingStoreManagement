using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
            var exist = await _db.Colors.ExistsAsync((c) =>  c.Name ==  model.Name.Trim());
            if (exist)
                return Result<ColorListDTO>.Failure("هذا اللون موجود بالفعل ", ErrorType.conflict);
            var color = new Color(model.Name , model.Code);
            await _db.Colors.CreateAsync(color);
            await _db.Save();
            return Result<ColorListDTO>.Success(_mapper.Map<ColorListDTO>(color));
        }
        public async Task<Result<ColorListDTO>> GetByIdAsync(int id)
        {
            var color = await _db.Colors.FirstOrDefaultAsync((c) => c.Id  == id);
            if (color == null)
                return Result<ColorListDTO>.Failure("هذا اللون غير موجود ", ErrorType.notFound);
            return Result<ColorListDTO>.Success(_mapper.Map<ColorListDTO>(color));
        }
       
        public async Task<Result<IEnumerable<ColorListDTO>>>
            GetColorsAsync(string? searchTerm = null, CancellationToken ct = default)
        {
            var ColorsQuery = _db.Colors.GetAll();
            
            if (searchTerm != null)
                ColorsQuery = ColorsQuery.Where(c => c.Name.Contains(searchTerm));
            var ColorsDtoQuery = ColorsQuery.ProjectTo<ColorListDTO>(_mapper.ConfigurationProvider);
            var Colors = await ColorsDtoQuery.ToListAsync();
            return Result<IEnumerable<ColorListDTO>>.Success(Colors);
        }
        public async Task<Result<string>> UpdateColorAsync(int id, CreateUpdateColorDto model )
        {
            var color = await _db.Colors.FirstOrDefaultAsync( (c) => c.Id == id  , true);
            if (color == null)
                return Result<string>.Failure("هذا اللون غير موجود", ErrorType.notFound);
            if (!color.Name.Equals(model.Name.Trim()) || !color.Code.Equals(model.Code.Trim()))  //**---
            {
                var exist = await _db.Colors.ExistsAsync((c) => c.Name ==  model.Name.Trim() && c.Id != id);
                if (exist)
                    return Result<string>.Failure("هذا اللون موجود بالفعل", ErrorType.conflict);
                color.Update(model.Name, model.Code);
                await _db.Save();
            }
            return Result<string>.Success($"تم تعديل اللون بنجاح ");
        }
    }
}
