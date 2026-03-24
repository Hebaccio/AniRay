using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requestss.MovieRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.OtherHelpers;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace AniRay.Services.EntityServices.MovieService
{
    public class MovieService : BaseCRUDService<MovieMU, MovieME, MovieSOU, MovieSOE, Movie, MovieIRU, MovieIRE, MovieURU, MovieURE>, IMovieService
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
        public override IQueryable<Movie> AddGetPagedFiltersForUsers(MovieSOU search, IQueryable<Movie> query)
        {
            query = query.Where(m => !m.IsDeleted);
            query = SharedAddFiltersAttributes(search, query);
            return query;
        }
        private IQueryable<Movie> SharedAddFiltersAttributes(MovieSOU search, IQueryable<Movie> query)
        {
            if (!string.IsNullOrEmpty(search?.TitleFTS))
                query = query.Where(x => x.Title.Contains(search.TitleFTS));
            if (!string.IsNullOrEmpty(search?.StudioFTS))
                query = query.Where(x => x.Studio.Contains(search.StudioFTS));
            if (!string.IsNullOrEmpty(search?.DirectorFTS))
                query = query.Where(x => x.Director.Contains(search.DirectorFTS));
            
            if (search?.FavoritesGTE != null)
                query = query.Where(x => x.Favorites >= search.FavoritesGTE);
            if (search?.FavoritesLTE != null)
                query = query.Where(x => x.Favorites <= search.FavoritesLTE);
            
            if (search?.ReleaseDateGTE != null)
                query = query.Where(x => x.ReleaseDate >= search.ReleaseDateGTE);
            if (search?.ReleaseDateLTE != null)
                query = query.Where(x => x.ReleaseDate <= search.ReleaseDateLTE);
            
            if (search?.IsGenresIncluded == true)
                query = query.Include(x => x.MovieGenres).ThenInclude(x => x.Genre);
            if (search?.GenreIds != null && search.GenreIds.Any())
                query = query.Where(m =>search.GenreIds.All(gid =>m.MovieGenres.Any(mg => mg.GenreId == gid)));
            
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
        public override IQueryable<Movie> AddGetPagedFiltersForEmployees(MovieSOE search, IQueryable<Movie> query)
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
        public override async Task<ServiceResult<bool>> BeforeInsertForEmployees(MovieIRE request, Movie entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request field cannot be null!");

            var nullCheck = await BeforeInsertChecks(request, entity, cancellationToken);
            if (!nullCheck.Success)
                return nullCheck;

            entity.IsDeleted = false;
            entity.Favorites = 0;

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> BeforeInsertChecks(MovieIRE request, Movie entity, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            //Image
            result = UpsertHelper.ValidateStringLength(request.Image, 1, 300, "Image URL", false);
            if (!result.Success) return result;

            //Title
            result = UpsertHelper.ValidateStringLength(request.Title, 1, 150, nameof(request.Title), false);
            if (!result.Success) return result;

            //Description            
            result = UpsertHelper.ValidateStringLength(request.Description, 1, 1000, nameof(request.Description), false);
            if (!result.Success) return result;

            //Studio
            result = UpsertHelper.ValidateStringLength(request.Studio, 1, 40, nameof(request.Studio), false);
            if (!result.Success) return result;

            //Director        
            result = UpsertHelper.ValidateStringLength(request.Director, 1, 40, nameof(request.Director), false);
            if (!result.Success) return result;

            //Release Date
            var earliestAllowedDate = new DateOnly(1908, 8, 17);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var sixMonthsFromNow = today.AddMonths(6);
            result = UpsertHelper.ValidateDate(request.ReleaseDate, earliestAllowedDate, sixMonthsFromNow, "Movie Release Date", false);
            if (!result.Success) return result;

            //Genres
            if (!await GenreCheck(request, entity, cancellationToken))
                return ServiceResult<bool>.Fail("One or more genres do not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        public async Task<bool> GenreCheck(MovieIRE request, Movie entity, CancellationToken cancellationToken)
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
        public override async Task FinalInsertEmployeeIncludes(Movie entity, MovieIRE request)
        {
            await Context.Entry(entity)
                .Collection(e => e.MovieGenres)
                .Query().Include(mg => mg.Genre).LoadAsync();
        }
        #endregion

        #region Update - For Users
        //Doesn't Exist
        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(MovieURE request, Movie entity, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            //Image
            result = UpsertHelper.ValidateStringLength(request.Image, 1, 300, "Image URL", true);
            if (!result.Success) return result;

            //Title
            result = UpsertHelper.ValidateStringLength(request.Title, 1, 150, nameof(request.Title), true);
            if (!result.Success) return result;

            //Description            
            result = UpsertHelper.ValidateStringLength(request.Description, 1, 1000, nameof(request.Description), true);
            if (!result.Success) return result;

            //Studio
            result = UpsertHelper.ValidateStringLength(request.Studio, 1, 40, nameof(request.Studio), true);
            if (!result.Success) return result;

            //Director        
            result = UpsertHelper.ValidateStringLength(request.Director, 1, 40, nameof(request.Director), true);
            if (!result.Success) return result;

            //Release Date
            var earliestAllowedDate = new DateOnly(1908, 8, 17);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var sixMonthsFromNow = today.AddMonths(6);
            result = UpsertHelper.ValidateDate(request.ReleaseDate, earliestAllowedDate, sixMonthsFromNow, "Movie Release Date", true);
            if (!result.Success) return result;

            //Genres
            if (request.GenreIds != null)
            {
                result = await SyncMovieGenres(entity, request.GenreIds, cancellationToken);
                if (!result.Success) return result;
            }

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> SyncMovieGenres(Movie entity, List<int>? genreIds, CancellationToken cancellationToken)
        {
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
        public override async Task FinalUpdateEmployeeIncludes(Movie entity, MovieURE? request)
        {
            await Context.Entry(entity)
                .Collection(e => e.MovieGenres)
                .Query().Include(mg => mg.Genre).LoadAsync();
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