using System.Net;
using System.Net.Http;

namespace Tenants.Web.Client.Framework
{
    public class DefaultHttpClientHandler : HttpClientHandler
    {
        public DefaultHttpClientHandler() =>
            this.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
    }
}
