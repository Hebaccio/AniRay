using AniRay.API.Controllers.BaseControllers;
using AniRay.Model.Entities;
using AniRay.Services.EntityServices.VideoFormatService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoFormatController : BasicEntityController<VideoFormat>
    {
        public VideoFormatController(IVideoFormatService service)
            : base(service)
        {
        }
        


    }
}