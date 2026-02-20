using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Helpers;
using AniRay.Services.Interfaces;
using AniRay.Services.Services.BaseServices;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static AniRay.Services.Helpers.CoreData;

namespace AniRay.Services.Services
{
    public class UserService : 
        BaseCRUDService<UserUM, UserEM, UserESO, UserESO, User,UserIR, UserIR, UserUUR, UserEUR>, IUserService
    {
        private readonly ICurrentUserService _currentUser;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper)
        {
            _currentUser = currentUser;
        }

        //Add methods for: Add Employee, Add Boss, Update Employee/Boss (Boss only)

        #region Get Filters
        public override IQueryable<User> AddFiltersEmployees(UserESO search, IQueryable<User> query)
        {
            base.AddFiltersEmployees(search, query);

            if (!_currentUser.IsWorker())
                return null;

            if (!string.IsNullOrEmpty(search.UsernameFTS))
                query = query.Where(u => u.Username.Contains(search.UsernameFTS));

            if (!string.IsNullOrEmpty(search?.FullNameFTS))
                query = query.Where(u => u.Name.Contains(search.FullNameFTS) || u.LastName.Contains(search.FullNameFTS));

            if (!string.IsNullOrEmpty(search?.EmailFTS))
                query = query.Where(u => u.Email.Contains(search.EmailFTS));

            if (search?.BirthdayGTE != null)
                query = query.Where(u => u.Birthday >= search.BirthdayGTE);

            if (search?.BirthdayLTE != null)
                query = query.Where(u => u.Birthday <= search.BirthdayLTE);

            if (search?.CreatedAtGTE != null)
                query = query.Where(u => u.CreatedAt >= search.CreatedAtGTE);

            if (search?.CreatedAtLTE != null)
                query = query.Where(u => u.CreatedAt <= search.CreatedAtLTE);

            if (search?.UserRoleId != null)
                query = query.Where(u => u.UserRoleId == search.UserRoleId);

            if (search?.UserStatusId != null)
                query = query.Where(u => u.UserStatusId == search.UserStatusId);

            if (search?.GenderId != null)
                query = query.Where(u => u.GenderId == search.GenderId);

            if (search.OrderBy.HasValue)
            {
                var sort = search.SortType?.ToString() ?? "descending";
                var finalOrderBy = $"{search.OrderBy} {sort}";
                query = query.OrderBy(finalOrderBy);
            }

            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;
        }
        public override IQueryable<User> AddGetByIdFiltersEmployees(IQueryable<User> query)
        {
            base.AddGetByIdFiltersEmployees(query);

            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;
        }
        public override IQueryable<User> AddGetByIdFilters(IQueryable<User> query)
        {
            base.AddGetByIdFilters(query);

            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;
        }
        public override UserUM GetById(int id)
        {
            if (!_currentUser.IsUser() || !_currentUser.IsSelf(id))
                return null;

            return base.GetById(id);
        }
        public override UserEM GetByIdEmployees(int id)
        {
            if (!_currentUser.IsWorker())
                return null;

            return base.GetByIdEmployees(id);
        }
        public override Model.PagedResult<UserEM> GetPagedEmployees(UserESO search)
        {
            if (!_currentUser.IsWorker())
                return null;

            return base.GetPagedEmployees(search);
        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsert(UserIR request, User entity)
        {
            base.BeforeInsert(request, entity);
            
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            var nullCheck = BeforeInsertNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeInsertValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsBirthdayInvalid(request.Birthday))
                return ServiceResult<bool>.Fail("Birthday cannot be in the future or before January 1, 1900.");
            if (IsUsernameTaken(request.Username, null))
                return ServiceResult<bool>.Fail("Username already exists.");
            if (IsEmailTaken(request.Email, null))
                return ServiceResult<bool>.Fail("Email already exists.");

            var fkResult = ValidateForeignKeys(request.GenderId);
            if (!fkResult.Success)
                return fkResult;

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);
            entity.PasswordHash = hash;
            entity.PasswordSalt = salt;

            entity.CreatedAt = DateTime.UtcNow;
            entity.UserStatusId = (int)CoreUserStatus.Active;
            entity.UserRoleId = (int)CoreUserRole.User;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdate(UserUUR request, User entity)
        {
            base.BeforeUpdate(request, entity);

            if (!_currentUser.IsUser() || !_currentUser.IsSelf(entity.Id))
                return ServiceResult<bool>.Fail("Unauthorized action");

            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            if (entity == null || entity.UserStatusId != (int)CoreData.CoreUserStatus.Active)
                return ServiceResult<bool>.Fail($"Action unavailable because the user is Suspended or Deleted");

            var nullCheck = BeforeUpdateNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeUpdateValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsUsernameTaken(request.Username, entity.Id))
                return ServiceResult<bool>.Fail("Username already exists.");
            if (IsEmailTaken(request.Email, entity.Id))
                return ServiceResult<bool>.Fail("Email already exists.");
            if (IsBirthdayInvalid(request.Birthday))
                return ServiceResult<bool>.Fail("Birthday cannot be in the future or before January 1, 1900.");

            var fkResult = ValidateForeignKeys(request.GenderId);
            if (!fkResult.Success)
                return fkResult;

            return ServiceResult<bool>.Ok(true);
        }
        public override ServiceResult<bool> BeforeUpdateEmployee(UserEUR request, User entity)
        {
            base.BeforeUpdateEmployee(request, entity);

            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            Context.Entry(entity).Reference(u => u.UserRole).Load();
            Context.Entry(entity).Reference(u => u.UserStatus).Load();

            if (!_currentUser.IsEmployee() || entity.UserRole.Name != CoreUserRole.User.ToString())
                return ServiceResult<bool>.Fail("Unauthorized action");

            var nullCheck = BeforeUpdateEmployeesNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeUpdateEmployeesValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsUsernameTaken(request.Username, entity.Id))
                return ServiceResult<bool>.Fail("Username already exists.");
            if (IsEmailTaken(request.Email, entity.Id))
                return ServiceResult<bool>.Fail("Email already exists.");
            if (IsBirthdayInvalid(request.Birthday))
                return ServiceResult<bool>.Fail("Birthday cannot be in the future or before January 1, 1900.");
            var fkResult = ValidateForeignKeysEmployee(request.UserStatusId);
            if (!fkResult.Success)
                return fkResult;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Insert/Update Filters
        private ServiceResult<bool> BeforeInsertNullCheck(UserIR request)
        {
            if (string.IsNullOrEmpty(request?.Pfp?.Trim()))
                return ServiceResult<bool>.Fail("Profile picture cannot be null.");
            if (string.IsNullOrEmpty(request?.Username?.Trim()))
                return ServiceResult<bool>.Fail("Username cannot be null.");
            if (string.IsNullOrEmpty(request?.Name?.Trim()))
                return ServiceResult<bool>.Fail("First name cannot be null.");
            if (string.IsNullOrEmpty(request?.LastName?.Trim()))
                return ServiceResult<bool>.Fail("Last name cannot be null.");
            if (string.IsNullOrEmpty(request?.Email?.Trim()))
                return ServiceResult<bool>.Fail("Email cannot be null.");
            if (string.IsNullOrEmpty(request?.Password?.Trim()))
                return ServiceResult<bool>.Fail("Password cannot be null.");
            if (request.Birthday == null)
                return ServiceResult<bool>.Fail("Birthday cannot be null.");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeInsertValidation(UserIR request)
        {
            if (request.Pfp.Length > 300)
                return ServiceResult<bool>.Fail("Profile picture URL cannot exceed 300 characters.");
            if (request.Username.Length > 15)
                return ServiceResult<bool>.Fail("Username cannot exceed 15 characters.");
            if (request.Name.Length > 20)
                return ServiceResult<bool>.Fail("First name cannot exceed 20 characters.");
            if (request.LastName.Length > 30)
                return ServiceResult<bool>.Fail("Last name cannot exceed 30 characters.");
            if (request.Email.Length > 50)
                return ServiceResult<bool>.Fail("Email cannot exceed 50 characters.");
            if (request.Password.Length < 6 || request.Password.Length > 15)
                return ServiceResult<bool>.Fail("Password must be between 6 and 15 characters.");

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> BeforeUpdateNullCheck(UserUUR request)
        {
            if (string.IsNullOrEmpty(request?.Pfp?.Trim()))
                return ServiceResult<bool>.Fail("Profile picture cannot be null.");
            if (string.IsNullOrEmpty(request?.Username?.Trim()))
                return ServiceResult<bool>.Fail("Username cannot be null.");
            if (string.IsNullOrEmpty(request?.Name?.Trim()))
                return ServiceResult<bool>.Fail("First name cannot be null.");
            if (string.IsNullOrEmpty(request?.LastName?.Trim()))
                return ServiceResult<bool>.Fail("Last name cannot be null.");
            if (string.IsNullOrEmpty(request?.Email?.Trim()))
                return ServiceResult<bool>.Fail("Email cannot be null.");
            if (request.Birthday == null)
                return ServiceResult<bool>.Fail("Birthday cannot be null.");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeUpdateValidation(UserUUR request)
        {
            if (request.Pfp.Length > 300)
                return ServiceResult<bool>.Fail("Profile picture URL cannot exceed 300 characters.");
            if (request.Username.Length > 15)
                return ServiceResult<bool>.Fail("Username cannot exceed 15 characters.");
            if (request.Name.Length > 20)
                return ServiceResult<bool>.Fail("First name cannot exceed 20 characters.");
            if (request.LastName.Length > 30)
                return ServiceResult<bool>.Fail("Last name cannot exceed 30 characters.");
            if (request.Email.Length > 50)
                return ServiceResult<bool>.Fail("Email cannot exceed 50 characters.");

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> BeforeUpdateEmployeesNullCheck(UserEUR request)
        {
            if (string.IsNullOrEmpty(request?.Pfp?.Trim()))
                return ServiceResult<bool>.Fail("Profile picture cannot be null.");
            if (string.IsNullOrEmpty(request?.Username?.Trim()))
                return ServiceResult<bool>.Fail("Username cannot be null.");
            if (string.IsNullOrEmpty(request?.Name?.Trim()))
                return ServiceResult<bool>.Fail("First name cannot be null.");
            if (string.IsNullOrEmpty(request?.LastName?.Trim()))
                return ServiceResult<bool>.Fail("Last name cannot be null.");
            if (string.IsNullOrEmpty(request?.Email?.Trim()))
                return ServiceResult<bool>.Fail("Email cannot be null.");
            if (request.Birthday == null)
                return ServiceResult<bool>.Fail("Birthday cannot be null.");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeUpdateEmployeesValidation(UserEUR request)
        {
            if (request.Pfp.Length > 300)
                return ServiceResult<bool>.Fail("Profile picture URL cannot exceed 300 characters.");
            if (request.Username.Length > 15)
                return ServiceResult<bool>.Fail("Username cannot exceed 15 characters.");
            if (request.Name.Length > 20)
                return ServiceResult<bool>.Fail("First name cannot exceed 20 characters.");
            if (request.LastName.Length > 30)
                return ServiceResult<bool>.Fail("Last name cannot exceed 30 characters.");
            if (request.Email.Length > 50)
                return ServiceResult<bool>.Fail("Email cannot exceed 50 characters.");

            return ServiceResult<bool>.Ok(true);
        }

        private bool IsUsernameTaken(string username, int? currentUserId)
        {
            if (currentUserId == null)
                return Context.Set<User>().Any(u => u.Username == username);

            return Context.Set<User>().Any(u => u.Username == username && u.Id != currentUserId);
        }
        private bool IsEmailTaken(string email, int? currentUserId)
        {
            if (currentUserId == null)
                return Context.Set<User>().Any(u => u.Email == email);

            return Context.Set<User>().Any(u => u.Email == email && u.Id != currentUserId);
        }
        private bool IsBirthdayInvalid(DateOnly? birthday)
        {
            if (!birthday.HasValue)
                return true;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var earliestAllowed = new DateOnly(1900, 1, 1);

            return birthday.Value > today || birthday.Value < earliestAllowed;
        }
        private ServiceResult<bool> ValidateForeignKeys(int genderId)
        {
            if (!Context.Set<Gender>().Any(x => x.Id == genderId))
                return ServiceResult<bool>.Fail("Selected gender does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> ValidateForeignKeysEmployee(int userStatusId)
        {
            if (!Context.Set<UserStatus>().Any(x => x.Id == userStatusId))
                return ServiceResult<bool>.Fail("Selected User status does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Soft Delete
        public override ServiceResult<string> SoftDelete(int id)
        {
            if (!_currentUser.IsAuthenticated || !_currentUser.IsUser() || !_currentUser.IsSelf(id))
                return ServiceResult<string>.Fail("Unauthorized action");

            var entity = Context.Set<User>().Find(id);
            if (entity == null)
                return ServiceResult<string>.Fail("User not found in the database.");

            entity.UserStatusId = (int)CoreUserStatus.Deleted;

            Context.SaveChanges();

            return ServiceResult<string>.Ok($"User {entity.Name} {entity.LastName} is successfully deleted.");
        }
        #endregion
    }
}

