using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services;
using AniRay.Services.Services.BaseServices;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services
{
    public class BluRayService : BaseCRUDService<BluRayModel, BluRaySearchObject, BluRay, BluRayInsertRequest, BluRayUpdateRequest>, IBluRaysService
    {
        public BluRayService(AniRayDbContext context, IMapper mapper) : base(context, mapper)
        {}

        #region Get Filters
        public override IQueryable<BluRay> AddFilters(BluRaySearchObject search, IQueryable<BluRay> query)
        {
            query = base.AddFilters(search, query);

            query = query.Where(m => !m.IsDeleted && m.MovieId == search.MovieId && !m.Movie.IsDeleted);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br=> br.VideoFormat);

            return query;
        }
        public override IQueryable<BluRay> AddGetByIdFilters(IQueryable<BluRay> query)
        {
            query = base.AddGetByIdFilters(query);

            query = query.Where(br => !br.IsDeleted && !br.Movie.IsDeleted);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsert(BluRayInsertRequest request, BluRay entity)
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
            if (string.IsNullOrEmpty(request?.SubtitleLanguage?.Trim()))
                return ServiceResult<bool>.Fail("Subtitle language is required");
            if (request.Price == null)
                return ServiceResult<bool>.Fail("Price is required");

            if (request.Image.Length > 300)
                return ServiceResult<bool>.Fail("Image URL cannot exceed 300 characters");
            if (request.Title.Length > 150)
                return ServiceResult<bool>.Fail("Title cannot exceed 150 characters");
            if (request.Description.Length > 1000)
                return ServiceResult<bool>.Fail("Description cannot exceed 1000 characters");
            if (request.SubtitleLanguage.Length > 50)
                return ServiceResult<bool>.Fail("Subtitle language cannot exceed 50 characters");

            if (request.DiscCount < 1 || request.DiscCount > 5)
                return ServiceResult<bool>.Fail("Disc count must be between 1 and 5");
            if (request.Runtime < 1 || request.Runtime > 600)
                return ServiceResult<bool>.Fail("Runtime must be between 1 and 600 minutes");
            if (request.InStock < 0 || request.InStock > 100)
                return ServiceResult<bool>.Fail("Stock must be between 0 and 100");
            if (request.Price <= 0)
                return ServiceResult<bool>.Fail("Price must be greater than 0");

            if (IsReleaseDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than one week in the future or before August 17, 1908");

            if (!IsSubtitleLanguageValid(request.SubtitleLanguage))
                return ServiceResult<bool>.Fail("Subtitle language must be English");

            var fkResult = ValidateForeignKeys(request);
            if (!fkResult.Success)
                return fkResult;

            entity.IsDeleted = false;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdate(BluRayUpdateRequest request, BluRay entity)
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
            if (string.IsNullOrEmpty(request?.SubtitleLanguage?.Trim()))
                return ServiceResult<bool>.Fail("Subtitle language is required");
            if (request.Price == null)
                return ServiceResult<bool>.Fail("Price is required");

            if (request.Image.Length > 300)
                return ServiceResult<bool>.Fail("Image URL cannot exceed 300 characters");
            if (request.Title.Length > 150)
                return ServiceResult<bool>.Fail("Title cannot exceed 150 characters");
            if (request.Description.Length > 1000)
                return ServiceResult<bool>.Fail("Description cannot exceed 1000 characters");
            if (request.SubtitleLanguage.Length > 50)
                return ServiceResult<bool>.Fail("Subtitle language cannot exceed 50 characters");

            if (request.DiscCount < 1 || request.DiscCount > 5)
                return ServiceResult<bool>.Fail("Disc count must be between 1 and 5");
            if (request.Runtime < 1 || request.Runtime > 600)
                return ServiceResult<bool>.Fail("Runtime must be between 1 and 600 minutes");
            if (request.InStock < 0 || request.InStock > 100)
                return ServiceResult<bool>.Fail("Stock must be between 0 and 100");
            if (request.Price <= 0)
                return ServiceResult<bool>.Fail("Price must be greater than 0");

            if (IsReleaseDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than one week in the future or before August 17, 1908");
            if (!IsSubtitleLanguageValid(request.SubtitleLanguage))
                return ServiceResult<bool>.Fail("Subtitle language must be English");

            var fkResult = ValidateForeignKeys(request);
            if (!fkResult.Success)
                return fkResult;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Insert/Update Helpers
        private bool IsReleaseDateInvalid(DateTime? releaseDate)
        {
            if (!releaseDate.HasValue)
                return true;

            var today = DateTime.UtcNow.Date;
            var oneWeekFromNow = today.AddDays(7);
            var earliestAllowedDate = new DateTime(1908, 8, 17);

            return releaseDate.Value < earliestAllowedDate || releaseDate.Value > oneWeekFromNow;
        }
        private bool IsSubtitleLanguageValid(string subtitleLanguage)
        {
            return subtitleLanguage?.Trim().ToLower() == "english";
        }
        private ServiceResult<bool> ValidateForeignKeys(BluRayInsertRequest request)
        {
            if (!Context.Set<VideoFormat>().Any(v => v.Id == request.VideoFormatId))
                return ServiceResult<bool>.Fail("Selected video format does not exist.");

            if (!Context.Set<AudioFormat>().Any(a => a.Id == request.AudioFormatId))
                return ServiceResult<bool>.Fail("Selected audio format does not exist.");

            if (!Context.Set<Movie>().Any(m => m.Id == request.MovieId))
                return ServiceResult<bool>.Fail("Selected movie does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> ValidateForeignKeys(BluRayUpdateRequest request)
        {
            if (!Context.Set<VideoFormat>().Any(v => v.Id == request.VideoFormatId))
                return ServiceResult<bool>.Fail("Selected video format does not exist.");
            if (!Context.Set<AudioFormat>().Any(a => a.Id == request.AudioFormatId))
                return ServiceResult<bool>.Fail("Selected audio format does not exist.");
            if (!Context.Set<Movie>().Any(m => m.Id == request.MovieId))
                return ServiceResult<bool>.Fail("Selected movie does not exist.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region SoftDelete
        public override ServiceResult<string> SoftDelete(int id)
        {
            var entity = Context.Set<BluRay>().Find(id);
            if (entity == null)
                return ServiceResult<string>.Fail($"Blu-Ray is not found in the database");

            entity.IsDeleted = true;
            Context.SaveChanges();

            return ServiceResult<string>.Ok($"Blu-Ray: {entity.Title}, is succesfully deleted");
        }
        #endregion
    }
}