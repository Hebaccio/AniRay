using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.UserRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.OtherHelpers;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using static AniRay.Services.HelperServices.OtherHelpers.CoreData;

namespace AniRay.Services.EntityServices.UserService
{
    public class UserService :
        BaseCRUDService<UserMU, UserME, UserSOU, UserSOE, User, UserIRU, UserIRE, UserURU, UserURE>, IUserService
    {
        private readonly ICurrentUserService _currentUser;

        public UserService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        public override bool IsGetByIdForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<User> AddGetByIdFiltersForUsers(IQueryable<User> query)
        {
            query = query.Where(u => u.UserStatusId == (int)CoreData.CoreUserStatus.Active);
            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;
        }
        public override async Task<User?> EntityGetTrigger(int? id, IQueryable<User> query, CancellationToken cancellationToken)
        {
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == _currentUser.UserId, cancellationToken);
        }
        #endregion

        #region Get By Id - For Employees
        public override IQueryable<User> AddGetByIdFiltersForEmployees(IQueryable<User> query)
        {
            if (!_currentUser.IsBoss())
            {
                query = query.Where(u => u.UserRoleId == (int)CoreData.CoreUserRole.User);
            }
            query = query.Include(u => u.UserRole);
            query = query.Include(u => u.UserStatus);
            query = query.Include(u => u.Gender);

            return query;
        }
        #endregion

        #region Get Paged - For Users
        //Doesn't Exist
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<User> AddGetPagedFiltersForEmployees(UserSOE search, IQueryable<User> query)
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

            if (!_currentUser.IsBoss())
            {
                query = query.Where(u => u.UserRoleId == (int)CoreData.CoreUserRole.User);
            }
            else if(search?.UserRoleId != null && _currentUser.IsBoss())
            {
                query = query.Where(u => u.UserRoleId == search.UserRoleId);
            }

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
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(UserIRU request, User entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            var nullCheck = await BeforeInsertForUsersChecks(request, entity, cancellationToken);
            if (!nullCheck.Success)
                return nullCheck;

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);
            entity.PasswordHash = hash;
            entity.PasswordSalt = salt;

            entity.CreatedAt = DateTime.UtcNow;
            entity.UserStatusId = (int)CoreUserStatus.Active;
            entity.UserRoleId = (int)CoreUserRole.User;

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> BeforeInsertForUsersChecks(UserIRU request, User entity, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            //Image
            result = UpsertHelper.ValidateStringLength(request.Pfp, 1, 300, "Image URL", false);
            if (!result.Success) return result;

            //Username
            result = await UpsertHelper.ValidateUsernameAsync<User>(Context, request.Username, 6, 50, nameof(request.Username), _currentUser.UserId, cancellationToken, false);
            if (!result.Success) return result;

            //Name
            result = UpsertHelper.ValidateStringLength(request.Name, 6, 20, nameof(request.Name), false);
            if (!result.Success) return result;

            //Lastname
            result = UpsertHelper.ValidateStringLength(request.LastName, 6, 30, nameof(request.LastName), false);
            if (!result.Success) return result;

            //Email
            result = await UpsertHelper.ValidateEmailAsync<User>(Context, request.Email, 6, 50, nameof(request.Email), _currentUser.UserId, cancellationToken, false);
            if (!result.Success) return result;

            //Password
            result = UpsertHelper.ValidatePasswordRegex(request.Password, request.Password2, 6, 20, nameof(request.Password), false);
            if (!result.Success) return result;

            //Birthday
            var earliestAllowedDate = new DateOnly(1900, 01, 01);
            var today = DateOnly.FromDateTime(DateTime.Now);
            result = UpsertHelper.ValidateDate(request.Birthday, earliestAllowedDate, today, "Birthday", false);
            if (!result.Success) return result;

            //Gender
            result = await UpsertHelper.ValidateForeignKey<Gender>(Context, request.GenderId, nameof(Gender), cancellationToken, false);
            if (!result.Success) return result;

            return ServiceResult<bool>.Ok(true);
        }
        public override async Task FinalInsertUserIncludes(User entity, UserIRU request)
        {
            await Context.Set<User>().Include(u => u.UserRole).LoadAsync();
            await Context.Set<User>().Include(u => u.UserStatus).LoadAsync();
            await Context.Set<User>().Include(u => u.Gender).LoadAsync();
        }
        #endregion

        #region Insert - For Employees
        public override bool IsInsertForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsBoss();
        }
        public override async Task<ServiceResult<bool>> BeforeInsertForEmployees(UserIRE request, User entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            var nullCheck = await BeforeInsertForEmployeesChecks(request, entity, cancellationToken);
            if (!nullCheck.Success)
                return nullCheck;

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);
            entity.PasswordHash = hash;
            entity.PasswordSalt = salt;

            entity.CreatedAt = DateTime.UtcNow;
            entity.UserStatusId = (int)CoreUserStatus.Active;

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> BeforeInsertForEmployeesChecks(UserIRE request, User entity, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            //Image
            result = UpsertHelper.ValidateStringLength(request.Pfp, 1, 300, "Image URL", false);
            if (!result.Success) return result;

            //Username
            result = await UpsertHelper.ValidateUsernameAsync<User>(Context, request.Username, 6, 50, nameof(request.Username), _currentUser.UserId, cancellationToken, false);
            if (!result.Success) return result;

            //Name
            result = UpsertHelper.ValidateStringLength(request.Name, 6, 20, nameof(request.Name), false);
            if (!result.Success) return result;

            //Lastname
            result = UpsertHelper.ValidateStringLength(request.LastName, 6, 30, nameof(request.LastName), false);
            if (!result.Success) return result;

            //Email
            result = await UpsertHelper.ValidateEmailAsync<User>(Context, request.Email, 6, 50, nameof(request.Email), _currentUser.UserId, cancellationToken, false);
            if (!result.Success) return result;

            //Password
            result = UpsertHelper.ValidatePasswordRegex(request.Password, request.Password2, 6, 20, nameof(request.Password), false);
            if (!result.Success) return result;

            //Birthday
            var earliestAllowedDate = new DateOnly(1900, 01, 01);
            var today = DateOnly.FromDateTime(DateTime.Now);
            result = UpsertHelper.ValidateDate(request.Birthday, earliestAllowedDate, today, "Birthday", false);
            if (!result.Success) return result;

            //Gender
            result = await UpsertHelper.ValidateForeignKey<Gender>(Context, request.GenderId, nameof(Gender), cancellationToken, false);
            if (!result.Success) return result;

            //User Role
            result = await UpsertHelper.ValidateForeignKey<UserRole>(Context, request.UserRoleId, "User Role", cancellationToken, false);
            if (!result.Success) return result;

            return ServiceResult<bool>.Ok(true);
        }
        public override async Task FinalInsertEmployeeIncludes(User entity, UserIRE request)
        {
            await Context.Set<User>().Include(u => u.UserRole).LoadAsync();
            await Context.Set<User>().Include(u => u.UserStatus).LoadAsync();
            await Context.Set<User>().Include(u => u.Gender).LoadAsync();
        }
        #endregion

        #region Update - For Users
        public override bool IsUpdateForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<User?> EntityGetTriggerForUpdate(int? id, UserURU? request, CancellationToken cancellationToken)
        {
            return await Context.Set<User>().FindAsync(_currentUser.UserId, cancellationToken);
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForUsers(UserURU request, User entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            if (entity == null || entity.UserStatusId != (int)CoreData.CoreUserStatus.Active)
                return ServiceResult<bool>.Fail($"Action unavailable because the user is Suspended or Deleted");

            var nullCheck = await BeforeUpdateForUsersChecks(request, entity, cancellationToken);
            if (!nullCheck.Success)
                return nullCheck;

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> BeforeUpdateForUsersChecks(UserURU request, User entity, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            //Image
            result = UpsertHelper.ValidateStringLength(request.Pfp, 1, 300, "Image URL", true);
            if (!result.Success) return result;

            //Username
            result = await UpsertHelper.ValidateUsernameAsync<User>(Context, request.Username, 6, 50, nameof(request.Username), _currentUser.UserId, cancellationToken, true);
            if (!result.Success) return result;

            //Name
            result = UpsertHelper.ValidateStringLength(request.Name, 6, 20, nameof(request.Name), true);
            if (!result.Success) return result;

            //Lastname
            result = UpsertHelper.ValidateStringLength(request.LastName, 6, 30, nameof(request.LastName), true);
            if (!result.Success) return result;

            //Email
            result = await UpsertHelper.ValidateEmailAsync<User>(Context, request.Email, 6, 50, nameof(request.Email), _currentUser.UserId, cancellationToken, true);
            if (!result.Success) return result;

            //Password
            result = UpsertHelper.ValidatePasswordRegex(request.Password, request.Password2, 6, 20, nameof(request.Password), true);
            if (!result.Success) return result;

            //Birthday
            var earliestAllowedDate = new DateOnly(1900, 01, 01);
            var today = DateOnly.FromDateTime(DateTime.Now);
            result = UpsertHelper.ValidateDate(request.Birthday, earliestAllowedDate, today, "Birthday", true);
            if (!result.Success) return result;

            //Gender
            result = await UpsertHelper.ValidateForeignKey<Gender>(Context, request.GenderId, nameof(Gender), cancellationToken, true);
            if (!result.Success) return result;

            return ServiceResult<bool>.Ok(true);
        }
        public override async Task FinalUpdateUserIncludes(User entity, UserURU? request)
        {
            await Context.Set<User>().Include(u => u.UserRole).LoadAsync();
            await Context.Set<User>().Include(u => u.UserStatus).LoadAsync();
            await Context.Set<User>().Include(u => u.Gender).LoadAsync();
        }
        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(UserURE request, User entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            if (_currentUser.IsBoss())
            {
                var nullCheck = await BeforeUpdateForEmployeesChecks(request, entity, cancellationToken);
                if (!nullCheck.Success)
                    return nullCheck;

                return ServiceResult<bool>.Ok(true);
            }
            else if (UserIsNotBoss(entity))
            {
                var nullCheck = await BeforeUpdateForEmployeesChecks(request, entity, cancellationToken);
                if (!nullCheck.Success)
                    return nullCheck;

                return ServiceResult<bool>.Ok(true);
            }

            return ServiceResult<bool>.Fail("Cannot update employee data");
        }
        private bool UserIsNotBoss(User entity)
        {
            return
                !_currentUser.IsBoss() &&
                (entity.UserRoleId == (int)CoreData.CoreUserRole.User) ||
                (entity.Id == _currentUser.UserId &&
                entity.UserStatusId != (int)CoreData.CoreUserStatus.FiredOrQuit);
        }
        private async Task<ServiceResult<bool>> BeforeUpdateForEmployeesChecks(UserURE request, User entity, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            //Image
            result = UpsertHelper.ValidateStringLength(request.Pfp, 1, 300, "Image URL", true);
            if (!result.Success) return result;

            //Username
            result = await UpsertHelper.ValidateUsernameAsync<User>(Context, request.Username, 6, 50, nameof(request.Username), _currentUser.UserId, cancellationToken, true);
            if (!result.Success) return result;

            //Name
            result = UpsertHelper.ValidateStringLength(request.Name, 6, 20, nameof(request.Name), true);
            if (!result.Success) return result;

            //Lastname
            result = UpsertHelper.ValidateStringLength(request.LastName, 6, 30, nameof(request.LastName), true);
            if (!result.Success) return result;

            //Email
            result = await UpsertHelper.ValidateEmailAsync<User>(Context, request.Email, 6, 50, nameof(request.Email), _currentUser.UserId, cancellationToken, true);
            if (!result.Success) return result;

            //Birthday
            var earliestAllowedDate = new DateOnly(1900, 01, 01);
            var today = DateOnly.FromDateTime(DateTime.Now);
            result = UpsertHelper.ValidateDate(request.Birthday, earliestAllowedDate, today, "Birthday", true);
            if (!result.Success) return result;

            //Gender
            result = await UpsertHelper.ValidateForeignKey<Gender>(Context, request.GenderId, nameof(Gender), cancellationToken, true);
            if (!result.Success) return result;

            //UserStatusId
            result = await UpsertHelper.ValidateUserStatusId(Context, request.UserStatusId, entity.UserRoleId, entity.Id, "User Status", cancellationToken, true);
            if (!result.Success) return result;

            return ServiceResult<bool>.Ok(true);
        }
        public override async Task FinalUpdateEmployeeIncludes(User entity, UserURE? request)
        {
            await Context.Set<User>().Include(u => u.UserRole).LoadAsync();
            await Context.Set<User>().Include(u => u.UserStatus).LoadAsync();
            await Context.Set<User>().Include(u => u.Gender).LoadAsync();
        }
        #endregion

        #region SoftDelete
        private bool IsSoftDeleteAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<ActionResult<string>> SoftDelete(int? id, CancellationToken cancellationToken)
        {
            if (!IsSoftDeleteAuthorized())
                return new UnauthorizedResult();

            var entity = await Context.Set<User>().FindAsync(_currentUser.UserId, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "User not found in the database." });
            var token = await Context.Set<RefreshToken>().FirstOrDefaultAsync(x => x.UserId == _currentUser.UserId, cancellationToken);
            if (token == null)
                return new NotFoundObjectResult(new { message = "Token not found in the database." });

            entity.UserStatusId = (int)CoreUserStatus.Deleted;
            token.Revoked = true;
            await Context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new { message = $"User {entity.Name} {entity.LastName} is successfully deleted." });
        }
        #endregion

    }
}
