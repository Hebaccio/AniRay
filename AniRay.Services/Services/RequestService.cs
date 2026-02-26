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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Services
{
    public class RequestService : BaseCRUDService<RequestUM, RequestUM, RequestUSO, RequestESO, Request, RequestUIR, RequestUIR, RequestUUR, RequestUUR>, IRequestService
    {
        private readonly ICurrentUserService _currentUser;
        public RequestService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser) 
        {
            _currentUser = currentUser;
        }

        #region Get By Id - User
        public override async Task<ActionResult<RequestUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            if (!IsGetByIdForUsersAuthorized(_currentUser.UserId))
                return new UnauthorizedResult();
            IQueryable<Request> query = Context.Set<Request>().AsQueryable();
            query = AddGetByIdFiltersForUsers(query);

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);

            if (entity == null)
                return new NotFoundObjectResult(new { message = $"Entity with ID {id} not found." });

            var mapped = Mapper.Map<RequestUM>(entity);

            return new OkObjectResult(mapped);
        }
        public override bool IsGetByIdForUsersAuthorized(int? id)
        {
            return _currentUser.IsAuthenticated && (_currentUser.IsUser() && _currentUser.IsSelf(id.Value));
        }
        public override IQueryable<Request> AddGetByIdFiltersForUsers(IQueryable<Request> query)
        {
            query = query.Where(r=> r.UserId == _currentUser.UserId);
            query = query.Include(r => r.User);
            return query;
        }
        #endregion

        #region Get Paged - User
        public override bool IsGetPagedForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<Request> AddGetPagedFiltersForUsers(RequestUSO search, IQueryable<Request> query)
        {
            query = query.Where(r=> r.UserId == search.UserId);
            query = query.Include(r => r.User);
            return query;
        }
        #endregion

        #region Get By Id - Employee
        public override bool IsGetByIdForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override IQueryable<Request> AddGetByIdFiltersForEmployees(IQueryable<Request> query)
        {
            query = query.Include(r => r.User);
            return query;
        }
        #endregion

        #region Get Paged - Employee
        public override bool IsGetPagedForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override IQueryable<Request> AddGetPagedFiltersForEmployees(RequestESO search, IQueryable<Request> query)
        {
            query = query.Include(r => r.User);

            if (!string.IsNullOrEmpty(search.TitleFTS))
                query = query.Where(r=> r.Title.Contains(search.TitleFTS));
            if (!string.IsNullOrEmpty(search.UserFullNameFTS))
                query = query.Where(r => r.User.Name.Contains(search.UserFullNameFTS) || r.User.LastName.Contains(search.UserFullNameFTS));
            if (!string.IsNullOrEmpty(search.UserMailFTS))
                query = query.Where(r=> r.User.Email.Contains(search.UserMailFTS));
            if(search.DateTimeGTE != null)
                query = query.Where(r=> r.DateTime >= search.DateTimeGTE);
            if (search.DateTimeLTE != null)
                query = query.Where(r => r.DateTime <= search.DateTimeLTE);

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

            bool exists = await Context.Set<User>().AnyAsync(x => x.Id == _currentUser.UserId, cancellationToken);
            if (!exists)
                return ServiceResult<bool>.Fail("Selected User does not exist.");

            entity.UserId = (int)_currentUser.UserId;
            entity.DateTime = DateTime.UtcNow;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

    }
}
