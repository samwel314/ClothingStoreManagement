using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services
{
    public class PaymentSourceService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;

        public PaymentSourceService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<string>> UpdatePaymentSourceAsync(int id, CreateUpdatePaymentSourceDto model)
        {
            var source = await _db.PaymentSources.FirstOrDefaultAsync((c) => c.Id == id, true);
            if (source == null)
                return Result<string>.Failure("مصدر الدفع غير موجود", ErrorType.notFound);
            model.Name = model.Name.Trim(); 
            if (!source.Name.Equals(model))  //**---
            {
                var exist = await _db.PaymentSources.ExistsAsync((c) => c.Name == model.Name && c.Id != id);
                if (exist)
                    return Result<string>.Failure("مصدر الدفع موجود بالفعل", ErrorType.conflict);
                source.UpdateName(model.Name);
                await _db.Save();
            }
            return Result<string>.Success($"تم تعديل مصدر الدفع  بنجاح ");
        }
        public async Task<Result<PaymentSourceDTO>> CreateSourceAsync(string name)
        {
            name = name.Trim();
            var exist = await _db.PaymentSources.ExistsAsync(p => p.Name == name );
            if (exist)
                return Result<PaymentSourceDTO>.Failure("مصدر الدفع هذا موجود بالفعل", ErrorType.conflict);

            var source = new PaymentSource(name);

            await _db.PaymentSources.CreateAsync(source);
            await _db.Save();

            return Result<PaymentSourceDTO>.Success(_mapper.Map<PaymentSourceDTO>(source));
        }

        public async Task<Result<IEnumerable<PaymentSourceDTO>>> GetActiveSourcesAsync()
        {
            var sources = await _db.PaymentSources.GetAll()
                .ProjectTo<PaymentSourceDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<PaymentSourceDTO>>.Success(sources);
        }

        public async Task<Result<string>> ToggleStatusAsync(int id)
        {
            var source = await _db.PaymentSources.FirstOrDefaultAsync(p => p.Id == id, true);
            if (source == null)
                return Result<string>.Failure("المصدر غير موجود", ErrorType.notFound);

            source.ToggleStatus(); 
            await _db.Save();

            return Result<string>.Success("تم تحديث الحالة بنجاح");
        }
    }
}
