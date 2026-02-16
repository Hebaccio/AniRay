using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services.BaseServices;
using Azure.Core;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Text;

namespace AniRay.Services.Services
{
    public class MoviesService : BaseCRUDService<MovieModel, MovieSearchObject, Movie, MovieInsertRequest, MovieUpdateRequest>, IMoviesService
    {
        public MoviesService(AniRayDbContext context, IMapper mapper) : base(context, mapper)
        { }

        #region Get Filters
        public override IQueryable<Movie> AddFilters(MovieSearchObject search, IQueryable<Movie> query)
        {
            query = base.AddFilters(search, query);
            query = query.Where(m => !m.IsDeleted);

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
                query = query.Include(x => x.MovieGenres).ThenInclude(x=> x.Genre);
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
        public override IQueryable<Movie> AddGetByIdFilters(IQueryable<Movie> query)
        {
            query = query.Where(m => !m.IsDeleted).Include(x => x.MovieGenres).ThenInclude(x => x.Genre);
            return query;
        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsert(MovieInsertRequest request, Movie entity)
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

            if (IsDateInvalid(request))
                return ServiceResult<bool>.Fail("Release date cannot be before August 17, 1908 or more than one week in the future.");

            if (!GenreCheck(request, entity))
                return ServiceResult<bool>.Fail("One or more genres do not exist.");

            entity.Favorites = 0;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdate(MovieUpdateRequest request, Movie entity)
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

            if (IsDateInvalid(request))
                return ServiceResult<bool>.Fail("Release date cannot be before August 17, 1908 or more than one week in the future.");

            var genreResult = SyncMovieGenres(entity, request.GenreIds);
            if (!genreResult.Success)
                return genreResult;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Insert/Update Helpers
        public bool GenreCheck(MovieInsertRequest request, Movie entity)
        {
            if (request.GenreIds == null || !request.GenreIds.Any())
                return true;

            var distinctGenreIds = request.GenreIds.Distinct().ToList();

            var existingGenreIds = Context.Set<Genre>()
                .Where(g => distinctGenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToList();

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
        public bool IsDateInvalid(MovieInsertRequest request)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var oneWeekFromNow = today.AddDays(7);
            var earliestAllowedDate = new DateOnly(1908, 8, 17);

            return request.ReleaseDate > oneWeekFromNow || request.ReleaseDate < earliestAllowedDate;
        }
        private bool IsDateInvalid(MovieUpdateRequest request)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var oneWeekFromNow = today.AddDays(7);
            var earliestAllowedDate = new DateOnly(1908, 8, 17);

            return request.ReleaseDate > oneWeekFromNow || request.ReleaseDate < earliestAllowedDate;
        }
        private ServiceResult<bool> SyncMovieGenres(Movie entity, IEnumerable<int>? genreIds)
        {
            if (genreIds == null)
                return ServiceResult<bool>.Ok(true);

            Context.Entry(entity)
                .Collection(x => x.MovieGenres)
                .Load();

            var distinctGenreIds = genreIds.Distinct().ToList();

            var existingGenreIds = Context.Set<Genre>()
                .Where(g => distinctGenreIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToList();

            if (existingGenreIds.Count != distinctGenreIds.Count)
                return ServiceResult<bool>.Fail("One or more genres do not exist.");

            var currentGenreIds = entity.MovieGenres.Select(mg => mg.GenreId).ToList();
            var genresToAdd = distinctGenreIds.Except(currentGenreIds);
            foreach (var genreId in genresToAdd)
            {
                entity.MovieGenres.Add(new MovieGenre { GenreId = genreId });
            }

            var genresToRemove = entity.MovieGenres.Where(mg => !distinctGenreIds.Contains(mg.GenreId)).ToList();
            foreach (var mg in genresToRemove)
            {
                entity.MovieGenres.Remove(mg);
            }

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region SoftDelete
        public override ServiceResult<string> SoftDelete(int id)
        {
            var entity = Context.Set<Movie>().Find(id);
            if (entity == null)
                return ServiceResult<string>.Fail($"Movie is not found in the database");

            entity.IsDeleted = true;
            Context.SaveChanges();

            return ServiceResult<string>.Ok($"Movie {entity.Title} is succesfully deleted");
        }
        #endregion
    }
}
