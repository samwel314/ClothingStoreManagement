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
        public async Task<IEnumerable<EmployeeDto>> GetAllActiveEmployeesAsync()
        {
            return await _db.Employees.GetAll().Where(e => e.IsActive).Select(u => new EmployeeDto
            {
                Id = u.Id,
                Name = u.Name,
                Type = u.Type,
                IsActive = u.IsActive,
                Phone = u.Phone,
                BaseSalary = u.BaseSalary,
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
        public async Task<Result<EmployeeDto>> GetEmployeeAsync(int id )
        {
            var employee =
                await _db.Employees.GetAll().Where(e => e.Id == id)
                .Select(u => new EmployeeDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Type = u.Type,
                    IsActive = u.IsActive,
                    Phone = u.Phone,
                    BaseSalary = u.BaseSalary,
                }).FirstOrDefaultAsync();
            if (employee == null)
                return Result<EmployeeDto>.Failure("هذا الموظف غير موجود ", ErrorType.notFound);
            return Result<EmployeeDto>.Success(employee);
        }
        public async Task AddEmployeeTransactionAsync(EmployeeTransaction dto)
        {
            var transaction = new EmployeeTransaction
            {
                EmployeeId = dto.EmployeeId,
                Amount = dto.Amount,
                Type = dto.Type,
                Notes = dto.Notes,
                DaysCount = dto.DaysCount,
                CreatedById = dto.CreatedById,
            };
            await _db.EmployeeTransaction.CreateAsync(transaction);
            await _db.Save();
            _db.Clear();    
        }   
        public async Task <EmployeeDetails> EmployeeDetailsAsync (int id , DateTime date)
        {
            DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);

            DateTime endOfMonth = startOfMonth.AddMonths(1).AddSeconds(-1);

            var thisMonthTransactions = await _db.EmployeeTransaction.GetAll()
                .Where(t => t.EmployeeId == id &&
                            t.CreatedAt >= startOfMonth &&
                            t.CreatedAt <= endOfMonth).Select(e => new EmployeeTransactionsDto
                            {
                                Amount = e.Amount,  
                                CreatedAt =   e.CreatedAt,    
                                Description = e.Notes , 
                                Type = e.Type ,
                                Days = e.DaysCount ?? 0,
                                CreatedBy = e.CreatedBy.UserName
                            })
                .ToListAsync();

            return new EmployeeDetails
            {
                EmployeeTransactions = thisMonthTransactions,   
                Borrow = thisMonthTransactions.Where(et=>et.Type == EmployeeTransactionType.Borrow).Sum(et=> et.Amount),
                Absence = thisMonthTransactions.Where(et => et.Type == EmployeeTransactionType.Absence).Sum(et => et.Amount),
                Bonus = thisMonthTransactions.Where(et => et.Type == EmployeeTransactionType.Bonus ).Sum(et => et.Amount),
                Penalty = thisMonthTransactions.Where(et => et.Type == EmployeeTransactionType.Penalty).Sum(et => et.Amount),
                AbsenceDays = thisMonthTransactions.Where(et => et.Type == EmployeeTransactionType.Absence).Select(et=>et.Days).Sum(),
            }; 
        }
    }

}
