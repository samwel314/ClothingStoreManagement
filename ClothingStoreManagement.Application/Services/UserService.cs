using AutoMapper;
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
    public class UserService
    {
        private readonly IUnitOfWork _db;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Result<string>> Register(CreateUserDto dto)
        {
            dto.UserName = dto.UserName.Trim().ToLower();
            var userExist = await _db.Users.ExistsAsync(u => u.UserName == dto.UserName.Trim().ToLower());
            if (userExist)
                return Result<string>.Failure("اسم المستخدم موجود بالفعل", ErrorType.conflict);
            var user = new User(dto.UserName, BCrypt.Net.BCrypt.HashPassword(dto.Password), dto.Role);

            await _db.Users.CreateAsync(user);
            await _db.Save();
            return Result<string>.Success(" تم إنشاء المستخدم بنجاح");
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _db.Users.GetAll().Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                RoleName = u.Role,
                IsActive = u.IsActive,
            }).ToListAsync();
        }

        public async Task<Result<string>> ActiveUserAsync(int Id)
        {
            var user = await _db.Users.FirstOrDefaultAsync((u) => u.Id == Id, true);
            if (user == null)
                return Result<string>.Failure("هذا المستخدم غير موجود", ErrorType.notFound);
            user.Activate();
            await _db.Save();
            return Result<string>.Success("تم تفعيل الحساب بنجاح");
        }
        public async Task<Result<string>> DeActiveUserAsync(int Id)
        {
            var user = await _db.Users.FirstOrDefaultAsync((u) => u.Id == Id, true);
            if (user == null)
                return Result<string>.Failure("هذا المستخدم غير موجود", ErrorType.notFound);
            user.Deactivate();
            await _db.Save();
            return Result<string>.Success("تم تعطيل الحساب   بنجاح");
        }

        public async Task<Result<string>> UpdateUserData(UpdateUserDto dto)
        {
            dto.UserName = dto.UserName.Trim().ToLower();
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == dto.Id, true);
            if (user == null)
                return Result<string>.Failure("المستخدم غير موجود", ErrorType.notFound);
            if (user.UserName != dto.UserName)
            {
                var userExist = await _db.Users.ExistsAsync(u => u.UserName == dto.UserName);
                if (userExist)
                    return Result<string>.Failure("اسم المستخدم موجود بالفعل", ErrorType.conflict);
                user.UpdateUserName(dto.UserName);
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(dto.Password));
            }
            if (user.Role != dto.Role)
            {
                user.UpdateRole(dto.Role);
            }   

            await _db.Save();
            return Result<string>.Success("تم تحديث بيانات المستخدم بنجاح");
        }
        public async Task<Result<UserSessionDto>> LoginAsync(LoginRequestDto loginDto)
        {
            try
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

                if (user == null)
                    return Result<UserSessionDto>.Failure("اسم المستخدم أو كلمة المرور غير صحيحة" , ErrorType.notFound);

                if (!user.IsActive)
                    return Result<UserSessionDto>.Failure("هذا الحساب معطل، يرجى مراجعة الإدارة", ErrorType.validation);

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

                if (!isPasswordValid)
                    return Result<UserSessionDto>.Failure("اسم المستخدم أو كلمة المرور غير صحيحة", ErrorType.validation);

                var session = new UserSessionDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = user.Role
                };

                return Result<UserSessionDto>.Success(session);
            }
            catch (Exception ex)
            {
                return Result<UserSessionDto>.Failure("حدث خطأ غير متوقع: " + ex.Message , ErrorType.validation);
            }
        }
    }
}
