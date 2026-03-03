using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services.BaseServices;
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

namespace AniRay.Services.Services
{
    public class RequestService : BaseCRUDService<RequestUM, RequestEM, RequestUSO, RequestESO, Request, RequestUIR, RequestEIR, RequestUUR, RequestEUR>, IRequestService
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
        public override IQueryable<Request> AddGetByIdFiltersForEmployees(IQueryable<Request> query)
        {
            query = query.Include(r => r.User);
            return query;
        }
        #endregion

        #region Get Paged - For Users
        public override bool IsGetPagedForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<Request> AddGetPagedFiltersForUsers(RequestUSO search, IQueryable<Request> query)
        {
            query = query.Where(r => r.UserId == _currentUser.UserId);
            query = query.Include(r => r.User);
            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<Request> AddGetPagedFiltersForEmployees(RequestESO search, IQueryable<Request> query)
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
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(RequestUIR request, Request entity, CancellationToken cancellationToken)
        {
            if (request.Title.Length > 100)
                return ServiceResult<bool>.Fail("Title cannot contain more than 100 characters");
            if (request.Title.Length < 20)
                return ServiceResult<bool>.Fail("Title must be longer than 20 characters");
            if (request.Text.Length > 1000)
                return ServiceResult<bool>.Fail("Request text cannot contain more than 1000 characters");
            if (request.Text.Length < 20)
                return ServiceResult<bool>.Fail("Request text must be longer than 20 characters");

            var user = await Context.Set<User>().FirstOrDefaultAsync(x => x.Id == _currentUser.UserId, cancellationToken);
            if (user == null)
                return ServiceResult<bool>.Fail("Selected user does not exist.");

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
        //Doesn't Exist
        #endregion

        #region Update - For Employees
        //Doesn't Exist
        #endregion

        #region SoftDelete
        //Doesn't Exist
        #endregion

    }
}
