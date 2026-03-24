using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requestss.RequestRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.OtherHelpers;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.RequestService
{
    public class RequestService : BaseCRUDService<RequestMU, RequestME, RequestSOU, RequestSOE, Request, RequestIRU, RequestIRE, RequestURU, RequestURE>, IRequestService
    {
        private readonly ICurrentUserService _currentUser;
        public RequestService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser) 
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        public override bool IsGetByIdForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<Request> AddGetByIdFiltersForUsers(IQueryable<Request> query)
        {
            query = query.Where(r => r.UserId == _currentUser.UserId);
            query = query.Include(r => r.User);
            return query;
        }
        #endregion

        #region Get By Id - For Employees
        //Doesn't exist because it's handled in the update
        #endregion

        #region Get Paged - For Users
        public override bool IsGetPagedForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<Request> AddGetPagedFiltersForUsers(RequestSOU search, IQueryable<Request> query)
        {
            query = query.Where(r => r.UserId == _currentUser.UserId);
            query = query.Include(r => r.User);
            return query;
        }
        #endregion

        #region Get By Id - For Employees
        //Doesn't exist because it's handled in the update
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<Request> AddGetPagedFiltersForEmployees(RequestSOE search, IQueryable<Request> query)
        {
            query = query.Include(r => r.User);

            if (!string.IsNullOrEmpty(search.TitleFTS))
                query = query.Where(r => r.Title.Contains(search.TitleFTS));
            if (!string.IsNullOrEmpty(search.UserFullNameFTS))
                query = query.Where(r => r.User.Name.Contains(search.UserFullNameFTS) || r.User.LastName.Contains(search.UserFullNameFTS));
            if (!string.IsNullOrEmpty(search.UserMailFTS))
                query = query.Where(r => r.User.Email.Contains(search.UserMailFTS));
            if (search.DateTimeGTE != null)
                query = query.Where(r => r.DateTime >= search.DateTimeGTE);
            if (search.DateTimeLTE != null)
                query = query.Where(r => r.DateTime <= search.DateTimeLTE);

            if (search.OrderBy.HasValue)
            {
                var sort = search.SortType?.ToString() ?? "descending";
                var finalOrderBy = $"{search.OrderBy} {sort}";
                query = query.OrderBy(finalOrderBy);
            }

            query = query.Include(r => r.User);

            return query;
        }
        #endregion

        #region Insert - For Users
        public override bool IsInsertForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(RequestIRU request, Request entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request field cannot be null!");

            ServiceResult<bool> result;

            //Title
            result = UpsertHelper.ValidateStringLength(request.Title, 20, 200, nameof(request.Title), false);
            if (!result.Success) return result;

            //Text            
            result = UpsertHelper.ValidateStringLength(request.Text, 20, 1000, nameof(request.Text), false);
            if (!result.Success) return result;

            var user = await Context.Set<User>().FirstOrDefaultAsync(x => x.Id == _currentUser.UserId, cancellationToken);
            if (user == null)
                return ServiceResult<bool>.Fail("Selected user does not exist.");

            entity.Response = "";
            entity.ReadByStaff = false;
            entity.ReadByUser = true;
            entity.UserId = user.Id;
            entity.User = user;
            entity.DateTime = DateTime.UtcNow;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Insert - For Employees
        //Doesn't Exist
        #endregion

        #region Update - For Users
        public override bool IsUpdateForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForUsers(RequestURU request, Request entity, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(entity.Response))
                entity.ReadByUser = true;

            return ServiceResult<bool>.Ok(true);
        }
        public override async Task FinalUpdateUserIncludes(Request entity, RequestURU? request)
        {
            await Context.Entry(entity).Reference(e => e.User).LoadAsync();
        }

        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(RequestURE request, Request entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Response))
                return ServiceResult<bool>.Fail("Response cannot be null or empty");
            entity.ReadByStaff = true;
            entity.ReadByUser = false;

            return ServiceResult<bool>.Ok(true);
        }
        public override async Task FinalUpdateEmployeeIncludes(Request entity, RequestURE? request)
        {
            await Context.Entry(entity).Reference(e => e.User).LoadAsync();
        }
        #endregion

        #region SoftDelete
        //Doesn't Exist
        #endregion
    }
}
