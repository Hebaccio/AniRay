using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requestss.BluRay;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.BluRayService
{
    public class BluRayService : BaseCRUDService<BluRayMU, BluRayME, BluRaySOU, BluRaySOE, BluRay, BluRayIRU, BluRayIRE, BluRayURU, BluRayURE>, IBluRayService
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

        #region Get By Id - For Employees
        public override IQueryable<BluRay> AddGetByIdFiltersForEmployees(IQueryable<BluRay> query)
        {
            query = query.Where(br => !br.Movie.IsDeleted);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Get Paged - For Users
        public override IQueryable<BluRay> AddGetPagedFiltersForUsers(BluRaySOU search, IQueryable<BluRay> query)
        {
            query = query.Where(br => !br.IsDeleted && br.MovieId == search.MovieId && !br.Movie.IsDeleted).OrderByDescending(br => br.Title);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<BluRay> AddGetPagedFiltersForEmployees(BluRaySOE search, IQueryable<BluRay> query)
        {
            query = query.Where(br => br.MovieId == search.MovieId && !br.Movie.IsDeleted)
                .OrderBy(br => br.IsDeleted)
                .ThenByDescending(br => br.Title);

            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Insert - For Users
        //Doesn't Exist
        #endregion

        #region Insert - For Employees
        public override async Task<ServiceResult<bool>> BeforeInsertForEmployees(BluRayIRE request, BluRay entity, CancellationToken cancellationToken)
        {
            var nullCheck = BeforeInsertNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeInsertValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            if (IsReleaseDateInvalid(request.ReleaseDate, entity.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than 6 months in the future or before movie's release date");

            var fkResult = await ValidateForeignKeys(request.VideoFormatId, request.AudioFormatId, request.MovieId, cancellationToken);
            if (!fkResult.Success)
                return fkResult;

            entity.SubtitleLanguage = "English";
            entity.IsDeleted = false;

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> BeforeInsertNullCheck(BluRayIRE request)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Blu Ray cannot be null");
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
        private ServiceResult<bool> BeforeInsertValidation(BluRayIRE request)
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
        private bool IsReleaseDateInvalid(DateOnly? releaseDate, DateOnly movieReleaseDate)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var sixMonthsFromNow = today.AddMonths(6);

            return releaseDate < movieReleaseDate || releaseDate > sixMonthsFromNow;
        }
        private async Task<ServiceResult<bool>> ValidateForeignKeys(int? videoFormatId, int? audioFormatId, int? movieId, CancellationToken cancellationToken)
        {
            if (videoFormatId != null && !await Context.Set<VideoFormat>().AnyAsync(v => v.Id == videoFormatId, cancellationToken))
                return ServiceResult<bool>.Fail("Selected video format does not exist.");

            if (audioFormatId != null && !await Context.Set<AudioFormat>().AnyAsync(a => a.Id == audioFormatId, cancellationToken))
                return ServiceResult<bool>.Fail("Selected audio format does not exist.");

            if (movieId != null && !await Context.Set<Movie>().AnyAsync(m => m.Id == movieId, cancellationToken))
                return ServiceResult<bool>.Fail("Selected movie does not exist.");

            return ServiceResult<bool>.Ok(true);
        }

        #endregion

        #region Update - For Users
        //Doesn't Exist
        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(BluRayURE request, BluRay entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Blu Ray cannot be null");

            var nullCheck = await BeforeUpdateChecks(request, entity.ReleaseDate, cancellationToken);
            if (!nullCheck.Success)
                return nullCheck;

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> BeforeUpdateChecks(BluRayURE request, DateOnly movieReleaseDate, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            result = ValidateMaxLength(request?.Image, 300, nameof(request.Image));
            if (!result.Success) return result;
            result = ValidateMaxLength(request?.Title, 150, nameof(request.Title));
            if (!result.Success) return result;
            result = ValidateMaxLength(request?.Description, 1000, nameof(request.Description));
            if (!result.Success) return result;

            result = ValidateDate(request.ReleaseDate, movieReleaseDate);
            if (!result.Success) return result;

            result = ValidateAmount(request?.DiscCount, 1, 5, nameof(request.DiscCount));
            if (!result.Success) return result;
            result = ValidateAmount(request?.Runtime, 1, 600, nameof(request.Runtime));
            if (!result.Success) return result;
            result = ValidateAmount(request?.InStock, 1, 100, nameof(request.InStock));
            if (!result.Success) return result;

            result = ValidatePrice(request?.Price, 1, "Price");
            if (!result.Success) return result;

            var fkResult = await ValidateForeignKeys(request.VideoFormatId, request.AudioFormatId, request.MovieId, cancellationToken);
            if (!fkResult.Success) return fkResult;

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> ValidatePrice(decimal? price, decimal minValue, string attributeName)
        {
            if (price != null && price < minValue)
                return ServiceResult<bool>.Fail($"{attributeName} must be higher than {minValue}");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> ValidateAmount(int? attribute, int minValue, decimal maxValue, string attributeName)
        {
            if(attribute != null &&  (attribute < minValue || attribute > maxValue))
                return ServiceResult<bool>.Fail($"{attributeName} must be between {minValue} and {maxValue}");
            
            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> ValidateMaxLength(string? value, int max, string fieldName)
        {
            var v = value?.Trim();
            if (!string.IsNullOrEmpty(v) && v.Length > max)
                return ServiceResult<bool>.Fail($"{fieldName} cannot exceed {max} characters");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> ValidateDate(DateOnly? releaseDate, DateOnly movieReleaseDate)
        {
            if (releaseDate != null && IsReleaseDateInvalid(releaseDate, movieReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than 6 months in the future or before movie's release date");

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
