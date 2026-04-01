using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.HelperServices.OtherHelpers
{
    public class RabbitMqUnavailableException : Exception
    {
        public RabbitMqUnavailableException(string message) : base(message) { }
    }

    public class RabbitMqTimeoutException : Exception
    {
        public RabbitMqTimeoutException(string message) : base(message) { }
    }

    public class RabbitMqOtherException : Exception
    {
        public RabbitMqOtherException(string message) : base(message) { }
    }
}
