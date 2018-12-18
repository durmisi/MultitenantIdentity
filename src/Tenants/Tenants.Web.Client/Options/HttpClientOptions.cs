using System;

namespace Tenants.Web.Client.Options
{
    public class HttpClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}
