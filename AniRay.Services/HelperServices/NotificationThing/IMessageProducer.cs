using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public interface IMessageProducer
    {
        Task SendMessage<T>(string queue, T message);
    }
}
