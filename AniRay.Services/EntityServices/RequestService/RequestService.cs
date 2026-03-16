using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.RequestService
{
    public class RequestService : BaseCRUDService<RequestUM, RequestEM, RequestUSO, RequestESO, Request, RequestUIR, RequestEIR, RequestUUR, RequestEUR>, IRequestService
    {
        private readonly ICurrentUserService _currentUser;
        public RequestService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser) 
        {
            _currentUser = currentUser;
        }

        

    }
}
