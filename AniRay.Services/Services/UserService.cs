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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using static AniRay.Services.Helpers.CoreData;

namespace AniRay.Services.Services
{
    public class UserService :
        BaseCRUDService<UserUM, UserEM, UserESO, UserESO, User, UserIR, UserIR, UserUUR, UserEUR>, IUserService
    {
        private readonly ICurrentUserService _currentUser;

        public UserService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        public override bool IsGetByIdForUsersAuthorized(int? id)
        {
            return _currentUser.IsAuthenticated && (_currentUser.IsUser() && _currentUser.IsSelf(id.Value));
        }
        public override IQueryable<User> AddGetByIdFiltersForUsers(IQueryable<User> query)
        {
            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;
        }
        #endregion

        #region Get Paged - For Users
        //Has no implementation for User class/table
        #endregion

        #region Get By Id - For Employees
        public override bool IsGetByIdForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override IQueryable<User> AddGetByIdFiltersForEmployees(IQueryable<User> query)
        {
            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override bool IsGetPagedForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override IQueryable<User> AddGetPagedFiltersForEmployees(UserESO search, IQueryable<User> query)
        {
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
        #endregion

        #region Insert - For Users
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(UserIR request, User entity, CancellationToken cancellationToken)
        {
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
            if (await IsUsernameTaken(request.Username, null, cancellationToken))
                return ServiceResult<bool>.Fail("Username already exists.");
            if (await IsEmailTaken(request.Email, null, cancellationToken))
                return ServiceResult<bool>.Fail("Email already exists.");

            var fkResult = await ValidateForeignKeys(request.GenderId, cancellationToken);
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
            if (request?.Birthday == null)
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


        private async Task<bool> IsUsernameTaken(string username, int? currentUserId, CancellationToken cancellationToken = default)
        {
            if (currentUserId == null)
                return await Context.Set<User>().AnyAsync(u => u.Username == username, cancellationToken);

            return await Context.Set<User>().AnyAsync(u => u.Username == username && u.Id != currentUserId, cancellationToken);
        }
        private async Task<bool> IsEmailTaken(string email, int? currentUserId, CancellationToken cancellationToken = default)
        {
            if (currentUserId == null)
                return await Context.Set<User>().AnyAsync(u => u.Email == email, cancellationToken);

            return await Context.Set<User>().AnyAsync(u => u.Email == email && u.Id != currentUserId, cancellationToken);
        }
        private bool IsBirthdayInvalid(DateOnly? birthday)
        {
            if (!birthday.HasValue)
                return true;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var earliestAllowed = new DateOnly(1900, 1, 1);

            return birthday.Value > today || birthday.Value < earliestAllowed;
        }
        private async Task<ServiceResult<bool>> ValidateForeignKeys(int genderId, CancellationToken cancellationToken = default)
        {
            bool exists = await Context.Set<Gender>().AnyAsync(x => x.Id == genderId, cancellationToken);

            if (!exists)
                return ServiceResult<bool>.Fail("Selected gender does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update - For Users
        public override bool IsUpdateForUsersAuthorized(int? id)
        {
            return _currentUser.IsAuthenticated && (_currentUser.IsUser() && _currentUser.IsSelf(id.Value));
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForUsers(UserUUR request, User entity, CancellationToken cancellationToken)
        {
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

            if (await IsUsernameTaken(request.Username, entity.Id))
                return ServiceResult<bool>.Fail("Username already exists.");
            if (await IsEmailTaken(request.Email, entity.Id))
                return ServiceResult<bool>.Fail("Email already exists.");
            if (IsBirthdayInvalid(request.Birthday))
                return ServiceResult<bool>.Fail("Birthday cannot be in the future or before January 1, 1900.");

            var fkResult = await ValidateForeignKeys(request.GenderId);
            if (!fkResult.Success)
                return fkResult;

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

        #endregion

        #region Insert - For Employees
        //Has no implementation for User class/table
        #endregion

        #region Update - For Employees
        public override bool IsUpdateForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(UserEUR request, User entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            await Context.Entry(entity).Reference(u => u.UserRole).LoadAsync();
            await Context.Entry(entity).Reference(u => u.UserStatus).LoadAsync();

            if (entity.UserRole.Name != CoreUserRole.User.ToString())
                return ServiceResult<bool>.Fail("Unauthorized action");

            var nullCheck = BeforeUpdateEmployeesNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeUpdateEmployeesValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (await IsUsernameTaken(request.Username, entity.Id, cancellationToken))
                return ServiceResult<bool>.Fail("Username already exists.");
            if (await IsEmailTaken(request.Email, entity.Id, cancellationToken))
                return ServiceResult<bool>.Fail("Email already exists.");
            if (IsBirthdayInvalid(request.Birthday))
                return ServiceResult<bool>.Fail("Birthday cannot be in the future or before January 1, 1900.");
            var fkResult = await ValidateForeignKeysEmployee(request.UserStatusId, cancellationToken);
            if (!fkResult.Success)
                return fkResult;

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
        private async Task<ServiceResult<bool>> ValidateForeignKeysEmployee(int userStatusId, CancellationToken cancellationToken)
        {
            var success = await Context.Set<UserStatus>().AnyAsync(x => x.Id == userStatusId, cancellationToken);
            if (!success)
                return ServiceResult<bool>.Fail("Selected User status does not exist.");

            return ServiceResult<bool>.Ok(true);
        }

        #endregion

        #region Soft Delete
        public override bool IsSoftDeleteAuthorized(int id)
        {
            return _currentUser.IsAuthenticated && (_currentUser.IsUser() && _currentUser.IsSelf(id));
        }
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            if (!IsSoftDeleteAuthorized(id))
                return new UnauthorizedResult();

            var entity = await Context.Set<User>().FindAsync(id, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "User not found in the database." });

            entity.UserStatusId = (int)CoreUserStatus.Deleted;
            await Context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new { message = $"User {entity.Name} {entity.LastName} is successfully deleted." });
        }
        #endregion

    }
}
