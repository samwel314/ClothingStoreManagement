using AutoMapper;
using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Application.ResultHelpers;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Application.Services
{
    public class EmployeeService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public EmployeeService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<string>> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,    
                BaseSalary = dto.BaseSalary,    
                Phone = dto.Phone,
                Type = dto.Type!.Value , 
            }; 
    
           await  _db.Employees.CreateAsync   (employee);
           await _db.Save();
            _db.Clear();    
            return Result<string>.Success(" تم إنشاء المستخدم بنجاح");
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _db.Employees.GetAll().Select(u => new EmployeeDto
            {
                Id = u.Id,
                Name = u.Name,
                Type = u.Type,
                IsActive = u.IsActive,
                Phone = u.Phone,    
                BaseSalary  =u.BaseSalary,
            }).ToListAsync();
        }

        public async Task<Result<string>> ToggleStatusAsync(int id)
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(p => p.Id == id, true);
            if (employee == null)
                return Result<string>.Failure("الموظف غير موجود", ErrorType.notFound);

            employee.ToggleStatus();
            await _db.Save();
            _db.Clear();
            return Result<string>.Success("تم تحديث الحالة بنجاح");
        }


        public async Task <Result<string>> UpdateEmployeeAsync (UpdateEmployeeDto dto )
        {
            var employee = await _db.Employees.FirstOrDefaultAsync(p => p.Id == dto.Id, true);
            if (employee == null)
                return Result<string>.Failure("الموظف غير موجود", ErrorType.notFound);

            employee.BaseSalary = dto.BaseSalary;   
            employee.Name  = dto.Name;  
            employee.Phone = dto.Phone;
            employee.Type = dto.Type; 
            await _db.Save();
            _db.Clear();
            return Result<string>.Success("تم تحديث البيانات بنجاح بنجاح");
        }
    }

}
