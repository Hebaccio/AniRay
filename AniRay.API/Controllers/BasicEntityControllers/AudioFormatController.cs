using AniRay.API.Controllers.BaseControllers;
using AniRay.Model.Entities;
using AniRay.Services.EntityServices.AudioFormatService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AudioFormatController : BasicEntityController<AudioFormat>
    {
        public AudioFormatController(IAudioFormatService service)
            : base(service)
        {
        }

        

    }
}