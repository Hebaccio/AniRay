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
using System.Linq.Dynamic.Core;
using System.Threading;

namespace AniRay.Services.Services
{
    public class MovieService : BaseCRUDService<MovieUM, MovieEM, MovieUSO, MovieESO, Movie, MovieUIR, MovieEIR, MovieUUR, MovieEUR>, IMovieService
    {
        private readonly ICurrentUserService _currentUser;

        public MovieService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        public override IQueryable<Movie> AddGetByIdFiltersForUsers(IQueryable<Movie> query)
        {
            query = query.Where(m => !m.IsDeleted).Include(x => x.MovieGenres).ThenInclude(x => x.Genre);
            return query;
        }
        #endregion

        #region Get By Id - For Employees
        public override IQueryable<Movie> AddGetByIdFiltersForEmployees(IQueryable<Movie> query)
        {
            query = query.Include(x => x.MovieGenres).ThenInclude(x => x.Genre);
            return query;
        }
        #endregion

        #region Get Paged - For Users
        public override IQueryable<Movie> AddGetPagedFiltersForUsers(MovieUSO search, IQueryable<Movie> query)
        {
            query = query.Where(m => !m.IsDeleted);
            query = SharedAddFiltersAttributes(search, query);
            return query;
        }
        private IQueryable<Movie> SharedAddFiltersAttributes(MovieUSO search, IQueryable<Movie> query)
        {
            if (!string.IsNullOrEmpty(search?.TitleFTS))
            {
                query = query.Where(x => x.Title.Contains(search.TitleFTS));
            }
            if (!string.IsNullOrEmpty(search?.StudioFTS))
            {
                query = query.Where(x => x.Studio.Contains(search.StudioFTS));
            }
            if (!string.IsNullOrEmpty(search?.DirectorFTS))
            {
                query = query.Where(x => x.Director.Contains(search.DirectorFTS));
            }
            if (search?.FavoritesGTE != null)
            {
                query = query.Where(x => x.Favorites >= search.FavoritesGTE);
            }
            if (search?.FavoritesLTE != null)
            {
                query = query.Where(x => x.Favorites <= search.FavoritesLTE);
            }
            if (search?.ReleaseDateGTE != null)
            {
                query = query.Where(x => x.ReleaseDate >= search.ReleaseDateGTE);
            }
            if (search?.ReleaseDateLTE != null)
            {
                query = query.Where(x => x.ReleaseDate <= search.ReleaseDateLTE);
            }
            if (search?.IsGenresIncluded == true)
            {
                query = query.Include(x => x.MovieGenres).ThenInclude(x => x.Genre);
            }
            if (search?.GenreIds != null && search.GenreIds.Any())
            {
                query = query.Where(m =>
                    search.GenreIds.All(gid =>
                        m.MovieGenres.Any(mg => mg.GenreId == gid)
                    )
                );
            }
            if (search.OrderBy.HasValue)
            {
                var sort = search.SortType?.ToString() ?? "descending";
                var finalOrderBy = $"{search.OrderBy} {sort}";
                query = query.OrderBy(finalOrderBy);
            }

            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<Movie> AddGetPagedFiltersForEmployees(MovieESO search, IQueryable<Movie> query)
        {
            if (search.IsDeleted != null)
                query = query.Where(m => m.IsDeleted == search.IsDeleted);

            query = SharedAddFiltersAttributes(search, query);
            return query;
        }
        #endregion

        #region Insert - For Users
        //Doesn't Exist
        #endregion

        #region Insert - For Employees
        public override async Task<ServiceResult<bool>> BeforeInsertForEmployees(MovieEIR request, Movie entity, CancellationToken cancellationToken)
        {
            var nullCheck = BeforeInsertNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeInsertValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be before August 17, 1908 or more than one week in the future.");

            if (!await GenreCheck(request, entity, cancellationToken))
                return ServiceResult<bool>.Fail("One or more genres do not exist.");

            entity.IsDeleted = false;
            entity.Favorites = 0;

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> BeforeInsertNullCheck(MovieEIR request)
        {
            if (string.IsNullOrEmpty(request?.Image?.Trim()))
                return ServiceResult<bool>.Fail("Image URL cannot be null");
            if (string.IsNullOrEmpty(request?.Title?.Trim()))
                return ServiceResult<bool>.Fail("Movie Title cannot be null");
            if (string.IsNullOrEmpty(request?.Description?.Trim()))
                return ServiceResult<bool>.Fail("Movie Description cannot be null");
            if (request?.ReleaseDate == null)
                return ServiceResult<bool>.Fail("Release Date cannot be null");
            if (string.IsNullOrEmpty(request?.Studio?.Trim()))
                return ServiceResult<bool>.Fail("Studio Name cannot be null");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeInsertValidation(MovieEIR request)
        {
            if (request.Image.Length > 500)
                return ServiceResult<bool>.Fail("Image URL cannot be longer than 500 characters");
            if (request.Title.Length > 200)
                return ServiceResult<bool>.Fail("Title cannot be longer than 200 characters");
            if (request.Description.Length > 1000)
                return ServiceResult<bool>.Fail("Description cannot be longer than 1000 characters");
            if (request.Studio.Length > 100)
                return ServiceResult<bool>.Fail("Studio name cannot be longer than 100 characters");
            if (!string.IsNullOrEmpty(request?.Director?.Trim()) && request?.Director.Length > 100)
                return ServiceResult<bool>.Fail("Director name cannot be longer than 100 characters");

            return ServiceResult<bool>.Ok(true);
        }
        public async Task<bool> GenreCheck(MovieEIR request, Movie entity, CancellationToken cancellationToken)
        {
            if (request.GenreIds == null || !request.GenreIds.Any())
                return true;

            var distinctGenreIds = request.GenreIds.Distinct().ToHashSet();

            var existingGenreIds = (await Context.Set<Genre>()
                .Where(g => distinctGenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToListAsync(cancellationToken))
                .ToHashSet();

            if (existingGenreIds.Count != distinctGenreIds.Count)
                return false;

            foreach (var genreId in distinctGenreIds)
            {
                entity.MovieGenres.Add(new MovieGenre
                {
                    GenreId = genreId
                });
            }

            return true;
        }
        private bool IsDateInvalid(DateOnly? ReleaseDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var oneWeekFromNow = today.AddDays(7);
            var earliestAllowedDate = new DateOnly(1908, 8, 17);

            return ReleaseDate > oneWeekFromNow || ReleaseDate < earliestAllowedDate;
        }
        #endregion

        #region Update - For Users
        //Doesn't Exist
        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(MovieEUR request, Movie entity, CancellationToken cancellationToken)
        {
            var nullCheck = BeforeUpdateNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeUpdateValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be before August 17, 1908 or more than one week in the future.");

            var genreResult = await SyncMovieGenres(entity, request.GenreIds, cancellationToken);
            if (!genreResult.Success)
                return genreResult;

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> BeforeUpdateNullCheck(MovieEUR request)
        {
            if (string.IsNullOrEmpty(request?.Image?.Trim()))
                return ServiceResult<bool>.Fail("Image URL cannot be null");
            if (string.IsNullOrEmpty(request?.Title?.Trim()))
                return ServiceResult<bool>.Fail("Movie Title cannot be null");
            if (string.IsNullOrEmpty(request?.Description?.Trim()))
                return ServiceResult<bool>.Fail("Movie Description cannot be null");
            if (request?.ReleaseDate == null)
                return ServiceResult<bool>.Fail("Release Date cannot be null");
            if (string.IsNullOrEmpty(request?.Studio?.Trim()))
                return ServiceResult<bool>.Fail("Studio Name cannot be null");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeUpdateValidation(MovieEUR request)
        {
            if (request.Image.Length > 300)
                return ServiceResult<bool>.Fail("Image URL cannot be longer than 300 characters");
            if (request.Title.Length > 150)
                return ServiceResult<bool>.Fail("Title cannot be longer than 150 characters");
            if (request.Description.Length > 1000)
                return ServiceResult<bool>.Fail("Description cannot be longer than 1000 characters");
            if (request.Studio.Length > 100)
                return ServiceResult<bool>.Fail("Studio name cannot be longer than 100 characters");
            if (!string.IsNullOrEmpty(request?.Director?.Trim()) && request?.Director.Length > 100)
                return ServiceResult<bool>.Fail("Director name cannot be longer than 100 characters");

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> SyncMovieGenres(Movie entity, IEnumerable<int>? genreIds, CancellationToken cancellationToken)
        {
            if (genreIds == null)
                return ServiceResult<bool>.Ok(true);

            await Context.Entry(entity).Collection(x => x.MovieGenres).LoadAsync(cancellationToken);

            var distinctGenreIds = genreIds.Distinct().ToHashSet();
            var existingCount = await Context.Set<Genre>().CountAsync(g => distinctGenreIds.Contains(g.Id), cancellationToken);
            if (existingCount != distinctGenreIds.Count)
                return ServiceResult<bool>.Fail("One or more genres do not exist.");

            var currentGenreIds = entity.MovieGenres.Select(mg => mg.GenreId).ToHashSet();

            var genresToAdd = distinctGenreIds.Except(currentGenreIds);
            foreach (var genreId in genresToAdd)
                entity.MovieGenres.Add(new MovieGenre { GenreId = genreId });

            var genresToRemove = entity.MovieGenres.Where(mg => !distinctGenreIds.Contains(mg.GenreId)).ToList();
            foreach (var mg in genresToRemove)
                entity.MovieGenres.Remove(mg);

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region SoftDelete
        public bool IsSoftDeleteAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override async Task<ActionResult<string>> SoftDelete(int? id, CancellationToken cancellationToken)
        {
            if (!IsSoftDeleteAuthorized())
                return new UnauthorizedResult();

            var entity = await Context.Set<Movie>().FindAsync(id, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "Movie not found in the database." });

            entity.IsDeleted = true;
            await Context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new { message = $"Movie {entity.Title} is successfully deleted." });
        }
        #endregion

    }
}