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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AniRay.Services.Services
{
    public class UserFavoritesService :
        BaseCRUDService<UserFavoritesUM, UserFavoritesEM, UserFavoritesUSO, UserFavoritesESO, UserFavorites, UserFavoritesUIR, UserFavoritesEIR, UserFavoritesUUR, UserFavoritesEUR>,
        IUserFavoritesService
    {
        private readonly ICurrentUserService _currentUser;
        public UserFavoritesService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        //Doesn't Exist
        #endregion

        #region Get By Id - For Employees
        //Doesn't Exist
        #endregion

        #region Get Paged - For Users
        public override bool IsGetPagedForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<UserFavorites> AddGetPagedFiltersForUsers(UserFavoritesUSO search, IQueryable<UserFavorites> query)
        {
            query = query.Where(uf => uf.UserId == _currentUser.UserId);
            query = query.Include(uf => uf.Movie);
            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<UserFavorites> AddGetPagedFiltersForEmployees(UserFavoritesESO search, IQueryable<UserFavorites> query)
        {
            query = query.Where(uf => uf.UserId == search.UserId);
            query = query.Include(uf => uf.Movie);
            return query;
        }
        #endregion

        #region Insert - For Users
        public override bool IsInsertForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(UserFavoritesUIR request, UserFavorites entity, CancellationToken cancellationToken)
        {
            var AllEntities = await Context.Set<UserFavorites>().Where(uf => uf.UserId == _currentUser.UserId).ToListAsync(cancellationToken);
            if (AllEntities.Count >= 10)
                return ServiceResult<bool>.Fail("User can only have 10 favorite movies");

            bool alreadyExists = AllEntities.Any(uf => uf.MovieId == request.MovieId);
            if (alreadyExists)
                return ServiceResult<bool>.Fail("This movie is already in the user's favorites");

            var validationCheck = await ValidateForeignKeys(request, cancellationToken);
            if (!validationCheck.Success)
                return validationCheck;

            entity.UserId = (int)_currentUser.UserId;
            var movie = await Context.Set<Movie>().FirstOrDefaultAsync(m => m.Id == request.MovieId, cancellationToken);
            if (movie != null)
                movie.Favorites += 1;

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> ValidateForeignKeys(UserFavoritesUIR request, CancellationToken cancellationToken)
        {
            bool userExists = await Context.Set<User>().AnyAsync(x => x.Id == _currentUser.UserId, cancellationToken);
            if (!userExists)
                return ServiceResult<bool>.Fail("User does not exist.");

            bool movieExists = await Context.Set<Movie>().AnyAsync(x => x.Id == request.MovieId, cancellationToken);
            if (!movieExists)
                return ServiceResult<bool>.Fail("Movie does not exist.");

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
        public override async Task<UserFavorites?> EntityGetTriggerForUpdate(int? id, UserFavoritesUUR? request, CancellationToken cancellationToken)
        {
            return await Context.Set<UserFavorites>().FirstOrDefaultAsync(x => x.UserId == _currentUser.UserId, cancellationToken);
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForUsers(UserFavoritesUUR request, UserFavorites entity, CancellationToken cancellationToken)
        {
            var requestedIds = request.MovieId?.Distinct().ToList() ?? new List<int>();

            if (requestedIds.Count > 10)
                return ServiceResult<bool>.Fail("Maximum number of favorites is 10.");

            var validationCheck = await ValidateForeignKeysForUpdate(requestedIds, cancellationToken);
            if (!validationCheck.Success)
                return validationCheck;

            var userId = _currentUser.UserId.Value;

            var existingFavorites = await Context.Set<UserFavorites>().Where(x => x.UserId == userId).ToListAsync(cancellationToken);

            var currentMovieIds = existingFavorites.Select(x => x.MovieId).ToHashSet();
            var requestedSet = requestedIds.ToHashSet();

            var toRemove = existingFavorites.Where(x => !requestedSet.Contains(x.MovieId)).ToList();

            if (toRemove.Any())
            {
                var removedMovieIds = toRemove.Select(x => x.MovieId).ToList();

                var moviesToDecrement = await Context.Set<Movie>()
                    .Where(m => removedMovieIds.Contains(m.Id))
                    .ToListAsync(cancellationToken);

                foreach (var movie in moviesToDecrement)
                    movie.Favorites = Math.Max(0, movie.Favorites - 1);

                Context.Set<UserFavorites>().RemoveRange(toRemove);
            }

            var toAdd = requestedIds.Where(id => !currentMovieIds.Contains(id))
                .Select(id => new UserFavorites
                {
                    UserId = userId,
                    MovieId = id
                }).ToList();

            if (toAdd.Any())
            {
                await Context.Set<UserFavorites>()
                    .AddRangeAsync(toAdd, cancellationToken);

                var addedMovieIds = toAdd.Select(x => x.MovieId).ToList();

                var moviesToIncrement = await Context.Set<Movie>()
                    .Where(m => addedMovieIds.Contains(m.Id))
                    .ToListAsync(cancellationToken);

                foreach (var movie in moviesToIncrement)
                    movie.Favorites += 1;
            }

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> ValidateForeignKeysForUpdate(List<int> requestedIds, CancellationToken cancellationToken)
        {
            var userExists = await Context.Set<User>().AnyAsync(x => x.Id == _currentUser.UserId, cancellationToken);

            if (!userExists)
                return ServiceResult<bool>.Fail("User does not exist.");

            if (!requestedIds.Any())
                return ServiceResult<bool>.Ok(true);

            var existingMovieIds = await Context.Set<Movie>().Where(x => requestedIds.Contains(x.Id)).Select(x => x.Id).ToListAsync(cancellationToken);

            if (existingMovieIds.Count != requestedIds.Count)
                return ServiceResult<bool>.Fail("Some movies do not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update - For Employees
        //Doesn't Exist
        #endregion

        #region SoftDelete
        //Doesn't Exist
        #endregion

    }
}
