using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.BluRayRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.NotificationThing;
using AniRay.Services.HelperServices.OtherHelpers;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace AniRay.Services.EntityServices.BluRayService
{
    public class BluRayService : BaseCRUDService<BluRayMU, BluRayME, BluRaySOU, BluRaySOE, BluRay, BluRayIRU, BluRayIRE, BluRayURU, BluRayURE>, IBluRayService
    {
        private readonly ICurrentUserService _currentUser;
        private readonly BluRayNotificationService _service;

        public BluRayService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser, BluRayNotificationService service) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
            _service = service;
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
            if (request == null)
                return ServiceResult<bool>.Fail("Blu Ray cannot be null");

            var nullCheck = await BeforeInsertChecks(request, entity.ReleaseDate, cancellationToken);
            if (!nullCheck.Success)
                return nullCheck;

            entity.SubtitleLanguage = "English";
            entity.IsDeleted = false;

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> BeforeInsertChecks(BluRayIRE request, DateOnly movieReleaseDate, CancellationToken cancellationToken)
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

            //DiscCount
            result = UpsertHelper.ValidateAmount(request.DiscCount, 1, 5, nameof(request.DiscCount), false);
            if (!result.Success) return result;

            //Runtime
            result = UpsertHelper.ValidateAmount(request.Runtime, 1, 600, nameof(request.Runtime), false);
            if (!result.Success) return result;

            //Stock
            result = UpsertHelper.ValidateAmount(request.InStock, 0, 100, "Stock", false);
            if (!result.Success) return result;

            //Price
            result = UpsertHelper.ValidatePrice(request.Price, nameof(request.Price), false);
            if (!result.Success) return result;
            
            //Video Format
            result = await UpsertHelper.ValidateForeignKey<VideoFormat>(Context, request.VideoFormatId, "Video Format", cancellationToken, false);
            if (!result.Success) return result;
            
            //Audio Format
            result = await UpsertHelper.ValidateForeignKey<AudioFormat>(Context, request.AudioFormatId, "Audio Format", cancellationToken, false);
            if (!result.Success) return result;
            
            //Movie
            result = await UpsertHelper.ValidateForeignKey<Movie>(Context, request.MovieId, "Movie", cancellationToken, false);
            if (!result.Success) return result;
            
            //Release Date
            var entity = await Context.Set<Movie>().Where(m => m.Id == request.MovieId).FirstOrDefaultAsync(cancellationToken);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var sixMonthsFromNow = today.AddMonths(6);
            result = UpsertHelper.ValidateDate(request.ReleaseDate, entity.ReleaseDate, sixMonthsFromNow, "BluRay Release Date", false);
            if (!result.Success) return result;

            return ServiceResult<bool>.Ok(true);
        }
        public override async Task FinalInsertEmployeeIncludes(BluRay entity, BluRayIRE request)
        {
            await Context.Entry(entity).Reference(e => e.AudioFormat).LoadAsync();
            await Context.Entry(entity).Reference(e => e.VideoFormat).LoadAsync();
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

            var nullCheck = await BeforeUpdateChecks(request, entity.InStock, entity.Id, entity.MovieId , cancellationToken);
            if (!nullCheck.Success)
                return nullCheck;

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> BeforeUpdateChecks(BluRayURE request, int inStock, int bluRayId, int movieId, CancellationToken cancellationToken)
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

            //DiscCount
            result = UpsertHelper.ValidateAmount(request.DiscCount, 1, 5, nameof(request.DiscCount), true);
            if (!result.Success) return result;

            //Runtime
            result = UpsertHelper.ValidateAmount(request.Runtime, 1, 600, nameof(request.Runtime), true);
            if (!result.Success) return result;

            //Stock
            result = UpsertHelper.ValidateAmount(request.InStock, 0, 100, "Stock", true);
            if (!result.Success) return result;

            //Price
            result = UpsertHelper.ValidatePrice(request.Price, nameof(request.Price), true);
            if (!result.Success) return result;

            //Video Format
            result = await UpsertHelper.ValidateForeignKey<VideoFormat>(Context, request.VideoFormatId, "Video Format", cancellationToken, true);
            if (!result.Success) return result;

            //Audio Format
            result = await UpsertHelper.ValidateForeignKey<AudioFormat>(Context, request.AudioFormatId, "Audio Format", cancellationToken, true);
            if (!result.Success) return result;

            //Movie
            result = await UpsertHelper.ValidateForeignKey<Movie>(Context, request.MovieId, "Movie", cancellationToken, true);
            if (!result.Success) return result;

            //Release Date
            var entity = await Context.Set<Movie>().Where(m => m.Id == movieId).FirstOrDefaultAsync(cancellationToken);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var sixMonthsFromNow = today.AddMonths(6);
            result = UpsertHelper.ValidateDate(request.ReleaseDate, entity.ReleaseDate, sixMonthsFromNow, "BluRay Release Date", true);
            if (!result.Success) return result;

            if (request!.InStock != null && inStock == 0 && request.InStock > 0)
            {
                await TriggerCheck(Context, entity.Id);
                _ = _service.RunNotificationJob(entity.Id, "bluray_notifications_email_queue");
            }

            return ServiceResult<bool>.Ok(true);
        }

        public override async Task FinalUpdateEmployeeIncludes(BluRay entity, BluRayURE? request)
        {
            await Context.Entry(entity).Reference(e => e.AudioFormat).LoadAsync();
            await Context.Entry(entity).Reference(e => e.VideoFormat).LoadAsync();
        }
        private async Task TriggerCheck(AniRayDbContext context, int bluRayId)
        {
            var entity = await context.Set<BluRayNotificationTrigger>()
                .FirstOrDefaultAsync(t => t.BluRayId == bluRayId);

            if (entity == null)
            {
                entity = new BluRayNotificationTrigger
                {
                    BluRayId = bluRayId,
                    Trigger = true
                };

                await context.AddAsync(entity);
            }
            else
            {
                entity.Trigger = true;
                context.Update(entity);
            }

            await context.SaveChangesAsync();
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
