using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.OrderRequests;
using AniRay.Services.HelperServices.OtherHelpers;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.StateMachines
{
    public class BaseOrderState
    {
        public AniRayDbContext Context { get; set; }
        public IMapper Mapper { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public BaseOrderState(AniRayDbContext context, IMapper mapper, IServiceProvider serviceProvider)
        {
            Context = context;
            Mapper = mapper;
            ServiceProvider = serviceProvider;
        }

        public virtual Order InProgress(OrderIRU order)
        {
            throw new Exception("Method Not Allowed!");
        }

        public virtual Order Cancelled(OrderIRU order)
        {
            throw new Exception("Method Not Allowed!");
        }

        public virtual Order Rejected(OrderIRU order)
        {
            throw new Exception("Method Not Allowed!");
        }

        public virtual Order Processed(OrderIRU order)
        {
            throw new Exception("Method Not Allowed!");
        }

        public BaseOrderState CreateState(string stateName)
        {
            switch (stateName)
            {
                //case "initial":
                //    return ServiceProvider.GetService<InitialProizvodiState>();
                //case "draft":
                //    return ServiceProvider.GetService<DraftProizvodiState>();
                //case "active":
                //    return ServiceProvider.GetService<ActiveProizvodiState>();
                //case "hidden":
                //    return ServiceProvider.GetService<HiddenProizvodiState>();
                default: throw new Exception("State not recognized");
            }
        }
    }
}
