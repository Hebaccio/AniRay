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
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Services
{
    public class BluRayService : BaseCRUDService<BluRayUM, BluRayEM, BluRayUSO, BluRayESO, BluRay, BluRayIR, BluRayIR, BluRayUR, BluRayUR>, IBluRayService
    {
        private readonly ICurrentUserService _currentUser;

        public BluRayService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }


        #region Get By Id - For Users
        public override IQueryable<BluRay> AddGetByIdFiltersForUsers(IQueryable<BluRay> query)
        {
            query = query.Where(br => !br.IsDeleted && !br.Movie.IsDeleted);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Get Paged - For Users
        public override IQueryable<BluRay> AddGetPagedFiltersForUsers(BluRayUSO search, IQueryable<BluRay> query)
        {
            query = query.Where(br => !br.IsDeleted && br.MovieId == search.MovieId && !br.Movie.IsDeleted).OrderByDescending(br => br.Title);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Get By Id - For Employees
        public override bool IsGetByIdForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override IQueryable<BluRay> AddGetByIdFiltersForEmployees(IQueryable<BluRay> query)
        {
            query = query.Where(br => !br.Movie.IsDeleted);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override bool IsGetPagedForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override IQueryable<BluRay> AddGetPagedFiltersForEmployees(BluRayESO search, IQueryable<BluRay> query)
        {
            query = query.Where(br => br.MovieId == search.MovieId && !br.Movie.IsDeleted).OrderBy(br => br.IsDeleted).ThenByDescending(br => br.Title);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Insert - For Users
        //No implementation as this function will not be added
        #endregion

        #region Update - For Users
        //No implementation as this function will not be added
        #endregion

        #region Insert - For Employees
        public override bool IsInsertForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override async Task<ServiceResult<bool>> BeforeInsertForEmployees(BluRayIR request, BluRay entity, CancellationToken cancellationToken)
        {
            var nullCheck = BeforeInsertNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeInsertValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsReleaseDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than one week in the future or before August 17, 1908");

            var fkResult = await ValidateForeignKeys(request.VideoFormatId, request.AudioFormatId, request.MovieId, cancellationToken);
            if (!fkResult.Success)
                return fkResult;

            entity.SubtitleLanguage = "English";
            entity.IsDeleted = false;

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> BeforeInsertNullCheck(BluRayIR request)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Blu Ray INsert Request cannot be null");
            if (string.IsNullOrEmpty(request?.Image?.Trim()))
                return ServiceResult<bool>.Fail("Image URL cannot be null");
            if (string.IsNullOrEmpty(request?.Title?.Trim()))
                return ServiceResult<bool>.Fail("Title cannot be null");
            if (string.IsNullOrEmpty(request?.Description?.Trim()))
                return ServiceResult<bool>.Fail("Description cannot be null");
            if (request.ReleaseDate == null)
                return ServiceResult<bool>.Fail("Release date cannot be null");
            if (request.VideoFormatId == null)
                return ServiceResult<bool>.Fail("Video format is required");
            if (request.AudioFormatId == null)
                return ServiceResult<bool>.Fail("Audio format is required");
            if (request.MovieId == null)
                return ServiceResult<bool>.Fail("Movie is required");
            if (request.DiscCount == null)
                return ServiceResult<bool>.Fail("Disc count is required");
            if (request.Runtime == null)
                return ServiceResult<bool>.Fail("Runtime is required");
            if (request.InStock == null)
                return ServiceResult<bool>.Fail("Stock is required");
            if (request.Price == null)
                return ServiceResult<bool>.Fail("Price is required");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeInsertValidation(BluRayIR request)
        {
            if (request.Image.Length > 300)
                return ServiceResult<bool>.Fail("Image URL cannot exceed 300 characters");
            if (request.Title.Length > 150)
                return ServiceResult<bool>.Fail("Title cannot exceed 150 characters");
            if (request.Description.Length > 1000)
                return ServiceResult<bool>.Fail("Description cannot exceed 1000 characters");

            if (request.DiscCount < 1 || request.DiscCount > 5)
                return ServiceResult<bool>.Fail("Disc count must be between 1 and 5");
            if (request.Runtime < 1 || request.Runtime > 600)
                return ServiceResult<bool>.Fail("Runtime must be between 1 and 600 minutes");
            if (request.InStock < 0 || request.InStock > 100)
                return ServiceResult<bool>.Fail("Stock must be between 0 and 100");
            if (request.Price <= 0)
                return ServiceResult<bool>.Fail("Price must be greater than 0");

            return ServiceResult<bool>.Ok(true);
        }
        private bool IsReleaseDateInvalid(DateOnly ReleaseDate)
        {

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var oneWeekFromNow = today.AddDays(7);
            var earliestAllowedDate = new DateOnly(1908, 8, 17);

            return ReleaseDate < earliestAllowedDate || ReleaseDate > oneWeekFromNow;
        }
        private async Task<ServiceResult<bool>> ValidateForeignKeys(int? videoFormatId, int? audioFormatId, int? movieId, CancellationToken cancellationToken)
        {
            if (!await Context.Set<VideoFormat>().AnyAsync(v => v.Id == videoFormatId, cancellationToken))
                return ServiceResult<bool>.Fail("Selected video format does not exist.");

            if (!await Context.Set<AudioFormat>().AnyAsync(a => a.Id == audioFormatId, cancellationToken))
                return ServiceResult<bool>.Fail("Selected audio format does not exist.");

            if (! await Context.Set<Movie>().AnyAsync(m => m.Id == movieId, cancellationToken))
                return ServiceResult<bool>.Fail("Selected movie does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update - For Employees
        public override bool IsUpdateForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }

        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(BluRayUR request, BluRay entity, CancellationToken cancellationToken)
        {
            var nullCheck = BeforeUpdateNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeUpdateValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsReleaseDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than one week in the future or before August 17, 1908");

            var fkResult = await ValidateForeignKeys(request.VideoFormatId, request.AudioFormatId, request.MovieId, cancellationToken);
            if (!fkResult.Success)
                return fkResult;

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> BeforeUpdateNullCheck(BluRayUR request)
        {
            if (string.IsNullOrEmpty(request?.Image?.Trim()))
                return ServiceResult<bool>.Fail("Image URL cannot be null");
            if (string.IsNullOrEmpty(request?.Title?.Trim()))
                return ServiceResult<bool>.Fail("Title cannot be null");
            if (string.IsNullOrEmpty(request?.Description?.Trim()))
                return ServiceResult<bool>.Fail("Description cannot be null");
            if (request.ReleaseDate == null)
                return ServiceResult<bool>.Fail("Release date cannot be null");
            if (request.VideoFormatId == null)
                return ServiceResult<bool>.Fail("Video format is required");
            if (request.AudioFormatId == null)
                return ServiceResult<bool>.Fail("Audio format is required");
            if (request.MovieId == null)
                return ServiceResult<bool>.Fail("Movie is required");
            if (request.DiscCount == null)
                return ServiceResult<bool>.Fail("Disc count is required");
            if (request.Runtime == null)
                return ServiceResult<bool>.Fail("Runtime is required");
            if (request.InStock == null)
                return ServiceResult<bool>.Fail("Stock is required");
            if (request.Price == null)
                return ServiceResult<bool>.Fail("Price is required");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeUpdateValidation(BluRayUR request)
        {
            if (request.Image.Length > 300)
                return ServiceResult<bool>.Fail("Image URL cannot exceed 300 characters");
            if (request.Title.Length > 150)
                return ServiceResult<bool>.Fail("Title cannot exceed 150 characters");
            if (request.Description.Length > 1000)
                return ServiceResult<bool>.Fail("Description cannot exceed 1000 characters");

            if (request.DiscCount < 1 || request.DiscCount > 5)
                return ServiceResult<bool>.Fail("Disc count must be between 1 and 5");
            if (request.Runtime < 1 || request.Runtime > 600)
                return ServiceResult<bool>.Fail("Runtime must be between 1 and 600 minutes");
            if (request.InStock < 0 || request.InStock > 100)
                return ServiceResult<bool>.Fail("Stock must be between 0 and 100");
            if (request.Price <= 0)
                return ServiceResult<bool>.Fail("Price must be greater than 0");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Soft Delete
        public override bool IsSoftDeleteAuthorized(int id)
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            if (!IsSoftDeleteAuthorized(id))
                return new UnauthorizedResult();

            var entity = await Context.Set<BluRay>().FindAsync(id, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "BluRay not found in the database." });

            entity.IsDeleted = true;
            await Context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new { message = $"BluRay {entity.Title} is successfully deleted." });
        }
        #endregion

    }
}
