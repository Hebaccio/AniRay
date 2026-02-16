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
using Azure;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;

namespace AniRay.Services.Services
{
    public class UserService : BaseCRUDService<UserModel, UserSearchObject, User, UserInsertRequest, UserUpdateRequest>, IUserService
    {
        public UserService(AniRayDbContext context, IMapper mapper) : base(context, mapper) 
        {}

        #region Get Inserts
        public override IQueryable<User> AddFilters(UserSearchObject search, IQueryable<User> query)
        {
            query = base.AddFilters(search, query);
            query = query.Where(u => u.UserRoleId != 1);

            if (!string.IsNullOrEmpty(search.UsernameFTS))
                query = query.Where(u=> u.Username.Contains(search.UsernameFTS));

            if(!string.IsNullOrEmpty(search?.FullNameFTS))
                query = query.Where(u=> u.Name.Contains(search.FullNameFTS) || u.LastName.Contains(search.FullNameFTS));

            if(!string.IsNullOrEmpty(search?.EmailFTS))
                query = query.Where(u=> u.Email.Contains(search.EmailFTS));

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
        public override IQueryable<User> AddGetByIdFilters(IQueryable<User> query)
        {
            base.AddGetByIdFilters(query);

            query = query.Where(u => u.UserRoleId != 1);

            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;

        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsert(UserInsertRequest request, User entity)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

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

            if (IsBirthdayInvalid(request.Birthday))
                return ServiceResult<bool>.Fail("Birthday cannot be in the future or before January 1, 1900.");

            if (IsUsernameTaken(request.Username))
                return ServiceResult<bool>.Fail("Username already exists.");
            if (IsEmailTaken(request.Email))
                return ServiceResult<bool>.Fail("Email already exists.");

            var fkResult = ValidateForeignKeys(request);
            if (!fkResult.Success)
                return fkResult;

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);
            entity.PasswordHash = hash;
            entity.PasswordSalt = salt;

            entity.CreatedAt = DateTime.UtcNow;
            entity.UserStatusId = 1;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdate(UserUpdateRequest request, User entity)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            if (entity == null || entity.UserStatusId != 1)
                return ServiceResult<bool>.Fail("User does not exist.");

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


            if (IsBirthdayInvalid(request.Birthday))
                return ServiceResult<bool>.Fail("Birthday cannot be in the future or before January 1, 1900.");

            if (IsUsernameTaken(request.Username, entity.Id))
                return ServiceResult<bool>.Fail("Username already exists.");

            if (IsEmailTaken(request.Email, entity.Id))
                return ServiceResult<bool>.Fail("Email already exists.");

            if (!UserStatusExists(request.UserStatusId))
                return ServiceResult<bool>.Fail("Selected user status does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Insert/Update Filters
        private bool IsBirthdayInvalid(DateOnly? birthday)
        {
            if (!birthday.HasValue)
                return true;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var earliestAllowed = new DateOnly(1900, 1, 1);

            return birthday.Value > today || birthday.Value < earliestAllowed;
        }
        private bool IsUsernameTaken(string username)
        {
            return Context.Set<User>()
                .Any(u => u.Username == username);
        }
        private bool IsEmailTaken(string email)
        {
            return Context.Set<User>()
                .Any(u => u.Email == email);
        }
        private ServiceResult<bool> ValidateForeignKeys(UserInsertRequest request)
        {
            if (!Context.Set<UserRole>().Any(x => x.Id == request.UserRoleId))
                return ServiceResult<bool>.Fail("Selected user role does not exist.");

            if (!Context.Set<UserStatus>().Any(x => x.Id == request.UserStatusId))
                return ServiceResult<bool>.Fail("Selected user status does not exist.");

            if (!Context.Set<Gender>().Any(x => x.Id == request.GenderId))
                return ServiceResult<bool>.Fail("Selected gender does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        private bool IsUsernameTaken(string username, int currentUserId)
        {
            return Context.Set<User>()
                .Any(u => u.Username == username &&
                          u.Id != currentUserId);
        }
        private bool IsEmailTaken(string email, int currentUserId)
        {
            return Context.Set<User>()
                .Any(u => u.Email == email &&
                          u.Id != currentUserId);
        }
        private bool UserStatusExists(int statusId)
        {
            return Context.Set<UserStatus>()
                .Any(s => s.Id == statusId);
        }
        #endregion

        #region SoftDelete
        public override ServiceResult<string> SoftDelete(int id)
        {
            var entity = Context.Set<User>().Find(id);
            if (entity == null)
                return ServiceResult<string>.Fail($"User is not found in the database");

            entity.UserStatusId = 3;
            Context.SaveChanges();

            return ServiceResult<string>.Ok($"User {entity.Name} {entity.LastName}, is succesfully deleted");
        }
        #endregion
    }
}
