using AniRay.Model.Requests.HelperRequests;
using AniRay.Model.Requests.UserCartRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.Services.EntityServices.UserCartService
{
    public interface IUserCartService : ICRUDService<UserCartMU, UserCartME, BaseSO, BaseSO, UserCartIRU, UserCartIRE, UserCartURU, UserCartURE>
    {
        public Task<ActionResult<bool>> AddIndividualBluRayToCart(UserCartIndividualURU request, CancellationToken cancellationToken);
        public Task<ActionResult<bool>> RemoveIndividualBluRayFromCart(int id, CancellationToken cancellationToken);
        public Task<ActionResult<bool>> IsBluRayInCart(int id, CancellationToken cancellationToken);
    }
}
