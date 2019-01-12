namespace MultitenantClient
{
    public class MultitenantClientConfiguration
    {
        public string CookieName { get; set; }

        public string Authority { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string ClientId { get; set; }
    }
}
