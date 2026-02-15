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
            query = query.Where(m => !m.IsDeleted);

            query = query.Where(br => br.MovieId == search.MovieId);
            query = query.Include(br => br.AudioFormat);
            query = query.Include(br=> br.VideoFormat);

            return query;
        }
        public override IQueryable<BluRay> AddGetByIdFilters(IQueryable<BluRay> query)
        {
            query = base.AddGetByIdFilters(query);
            query = query.Where(m => !m.IsDeleted);

            query = query.Include(br => br.AudioFormat);
            query = query.Include(br => br.VideoFormat);

            return query;
        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsert(BluRayInsertRequest request, BluRay entity)
        {
            if (IsReleaseDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than one week in the future or before August 17, 1908.");

            if (!IsSubtitleLanguageValid(request.SubtitleLanguage))
                return ServiceResult<bool>.Fail("Subtitle language must be English.");

            var foreignKeyResult = ValidateForeignKeys(request);
            if (!foreignKeyResult.Success)
                return foreignKeyResult;

            entity.IsDeleted = false;

            return ServiceResult<bool>.Ok(true);
        }

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
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdate(BluRayUpdateRequest request, BluRay entity)
        {
            if (IsReleaseDateInvalid(request.ReleaseDate))
                return ServiceResult<bool>.Fail("Release date cannot be more than one week in the future or before August 17, 1908.");

            if (!IsSubtitleLanguageValid(request.SubtitleLanguage))
                return ServiceResult<bool>.Fail("Subtitle language must be English.");

            var foreignKeyResult = ValidateForeignKeys(request);
            if (!foreignKeyResult.Success)
                return foreignKeyResult;

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
