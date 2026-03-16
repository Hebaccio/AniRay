using AniRay.Model.Entities;
using AniRay.Model.Requestss.BluRay;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.BluRayService
{
    public interface IBluRayService : ICRUDService<BluRayMU, BluRayME, BluRaySOU, BluRaySOE, BluRayIRU, BluRayIRE, BluRayURU, BluRayURE>
    {
    }
}
